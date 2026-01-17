using Application.Commons;
using Application.DTOs.Users;
using Application.Exceptions;
using Application.Features.Users.Queries;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Users.Handlers;

public class GetUserByIdHandler(IUserRepository repository) : IRequestHandler<GetUserByIdQuery, ApiResponse<UserResponseDto>>
{
    private readonly IUserRepository _userRepository = repository;
    public async Task<ApiResponse<UserResponseDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, false) ??
            throw new NotFoundException("User not found");

        var userDto = new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };
        return new ApiResponse<UserResponseDto>(userDto);

    }
}

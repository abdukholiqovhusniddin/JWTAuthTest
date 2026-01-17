using Application.Commons;
using Application.DTOs.Users;
using Application.Features.Users.Queries;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Handlers;

public class GetUsersHandler(IUserRepository repository) : IRequestHandler<GetUsersQuery, ApiResponse<List<UserResponseDto>>>
{
    private readonly IUserRepository _userRepository = repository;

    public async Task<ApiResponse<List<UserResponseDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllUsers();

        var response = users.Select(user => 
            new UserResponseDto { Id = user.Id, Username = user.Username,
                Email = user.Email, Role = user.Role }).ToList();

        return new ApiResponse<List<UserResponseDto>>
        {
            Data = response,
            StatusCode = 200
        };
    }
}

using Application.Commons;
using Application.DTOs.Users;
using Application.Exceptions;
using Application.Features.Users.Commands;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Handlers;

public class CreateUserHandler(IUserRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateUserCommand, ApiResponse<UserResponseDto>>
{
    private readonly IUserRepository _userRepository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<ApiResponse<UserResponseDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var t = await _userRepository.ExistEmailAsync(request.CreateUser.Email);
        if (t) throw new ApiException("Failed to check existing email");

        var user = new User
        {
            Username = request.CreateUser.Username,
            Email = request.CreateUser.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.CreateUser.Password),
            Role = request.CreateUser.Role,
            IsActive = true
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

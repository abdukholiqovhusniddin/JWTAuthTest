using Application.Commons;
using Application.DTOs.Users;
using MediatR;

namespace Application.Features.Users.Commands;

public record CreateUserCommand(CreateUserDto CreateUser)
    : IRequest<ApiResponse<UserResponseDto>>;

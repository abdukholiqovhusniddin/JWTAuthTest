using Application.Commons;
using Application.DTOs.Users;
using MediatR;

namespace Application.Features.Users.Commands;

public record UpdateUserCommand(UpdateUserDto UpdateUser) : IRequest<ApiResponse<bool>>;
using Application.Commons;
using Application.DTOs.Users;
using MediatR;

namespace Application.Features.Users.Queries;

public record GetUserByIdQuery(Guid Id) : IRequest<ApiResponse<UserResponseDto>>;

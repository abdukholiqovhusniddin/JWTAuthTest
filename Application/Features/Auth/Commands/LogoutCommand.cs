using Application.Commons;
using MediatR;

namespace Application.Features.Auth.Commands;

public record LogoutCommand(Guid UserId, string Jti) : IRequest<ApiResponse<Unit>>;

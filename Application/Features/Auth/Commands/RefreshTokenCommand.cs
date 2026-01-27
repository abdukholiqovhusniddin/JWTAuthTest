using Application.Commons;
using Application.DTOs.Auth;
using MediatR;

namespace Application.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken, string AccessToken) : IRequest<ApiResponse<TokenResponseDto>>;

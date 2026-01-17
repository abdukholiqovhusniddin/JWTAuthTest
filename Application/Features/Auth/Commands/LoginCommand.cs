using Application.Commons;
using Application.DTOs.Auth;
using MediatR;

namespace Application.Features.Auth.Commands;

public record LoginCommand(LoginRequestDto LoginRequest) : IRequest<ApiResponse<LoginResponseDto>>;
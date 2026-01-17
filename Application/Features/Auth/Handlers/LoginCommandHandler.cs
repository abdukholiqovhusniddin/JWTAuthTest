using Application.Commons;
using Application.DTOs.Auth;
using Application.Exceptions;
using Application.Features.Auth.Commands;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Handlers;

public class LoginCommandHandler(IUserRepository repository, 
    IRefreshTokenRepository refreshToken ,IJwtService jwtService, IUnitOfWork unitOfWork)
    : IRequestHandler<LoginCommand, ApiResponse<LoginResponseDto>>
{

    private readonly IUserRepository _userRepository = repository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshToken;
    private readonly IJwtService _jwt = jwtService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ApiResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.LoginRequest.Email) ??
            throw new ApiException("Invalid email");

        if (!BCrypt.Net.BCrypt.Verify(request.LoginRequest.Password, user.PasswordHash))
            throw new ApiException("Invalid password.");

        var refreshToken = new RefreshToken
        {
            Token = _jwt.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        await _refreshTokenRepository.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ApiResponse<LoginResponseDto>
        {
            Data = new LoginResponseDto
            {
                UserId = user.Id,
                Role = user.Role,
                Tokens = new TokenResponseDto
                {
                    AccessToken = _jwt.GenerateAccessToken(user.Id, user.Role),
                    RefreshToken = refreshToken.Token
                }
            }
        };

    }
}


using Application.Commons;
using Application.DTOs.Auth;
using Application.Exceptions;
using Application.Features.Auth.Commands;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Handlers;

public class RefreshTokenCommandHandler(IRefreshTokenRepository refreshToken,
    IJwtService jwtService, IUnitOfWork unitOfWork, IJwtBlacklistService blacklist)
    : IRequestHandler<RefreshTokenCommand, ApiResponse<TokenResponseDto>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshToken;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJwtBlacklistService _blacklist = blacklist;

    public async Task<ApiResponse<TokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _refreshTokenRepository.GetValidTokenAsync(request.Refreshtocen) ??
            throw new ApiException("Invalid refresh token");

        if (token.ExpiresAt < DateTime.UtcNow)
            throw new ApiException("Refresh token has expired");

        if (!string.IsNullOrWhiteSpace(request.Jti))
        {
            await _blacklist.AddAsync(request.Jti, TimeSpan.FromMinutes(15));
        }

        token.IsRevoked = true;

        var newRefreshToken = new RefreshToken
        {
            Token = _jwtService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = token.UserId
        };

        await _refreshTokenRepository.AddAsync(newRefreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ApiResponse<TokenResponseDto>
        {
            Data = new TokenResponseDto
            {
                AccessToken = _jwtService.GenerateAccessToken(
                    token.User.Id,
                    token.User.Role),
                RefreshToken = newRefreshToken.Token
            }
        };
    }
}

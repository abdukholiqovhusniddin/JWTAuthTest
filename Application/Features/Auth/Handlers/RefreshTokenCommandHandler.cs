using Application.Commons;
using Application.DTOs.Auth;
using Application.Exceptions;
using Application.Features.Auth.Commands;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Application.Features.Auth.Handlers;

public class RefreshTokenCommandHandler(IRefreshTokenRepository refreshToken, IUserRepository userRepository,
    IJwtService jwtService, IUnitOfWork unitOfWork, IJwtBlacklistService blacklist)
    : IRequestHandler<RefreshTokenCommand, ApiResponse<TokenResponseDto>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshToken;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJwtBlacklistService _blacklist = blacklist;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<ApiResponse<TokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.AccessToken))
            throw new ApiException("Access token is required to refresh (to read claims).");
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new ApiException("Refresh token is required");
        
        var refresh = await _refreshTokenRepository.GetValidTokenAsync(request.RefreshToken)
                      ?? throw new ApiException("Invalid refresh token or Refresh token revoked");
        if (refresh.ExpiresAt <= DateTime.UtcNow)
            throw new ApiException("Refresh token has expired");
    
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userIdClaim =
            principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? principal.FindFirst("sub")?.Value; 

        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var tokenUserId))
            throw new ApiException("Invalid user ID in access token");

        if (tokenUserId != refresh.UserId)
            throw new ApiException("Access token user does not match refresh token user.");
    
        var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        if (!string.IsNullOrWhiteSpace(jti))
            await _blacklist.AddAsync(jti, TimeSpan.FromMinutes(15)); 
        else
            throw new ApiException("Access token does not match refresh token.");
    
        var role = await _userRepository.GetRoleByIdAsync(refresh.UserId);
    
        refresh.IsRevoked = true;
        var newRefresh = new RefreshToken
        {
            Token = _jwtService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = refresh.UserId
        };
        await _refreshTokenRepository.AddAsync(newRefresh);
        var newAccess = _jwtService.GenerateAccessToken(refresh.UserId, role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    
        return new ApiResponse<TokenResponseDto>
        {
            Data = new TokenResponseDto
            {
                AccessToken = newAccess,
                RefreshToken = newRefresh.Token
            }
        };
    }
}

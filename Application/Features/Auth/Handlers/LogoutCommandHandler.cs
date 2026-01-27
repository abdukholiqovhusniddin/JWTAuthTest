using Application.Commons;
using Application.Exceptions;
using Application.Features.Auth.Commands;
using Application.Interfaces;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Application.Features.Auth.Handlers;

public class LogoutCommandHandler(IJwtBlacklistService blacklist, IUnitOfWork unitOfWork,ILogger<LogoutCommandHandler> logger,
    IRefreshTokenRepository refreshToken) : IRequestHandler<LogoutCommand, ApiResponse<Unit>>
{
    private readonly IJwtBlacklistService _blacklist = blacklist;
    private readonly IRefreshTokenRepository _refreshToken = refreshToken;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<LogoutCommandHandler> _logger = logger;


    public async Task<ApiResponse<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // 1) ✅ Redis blacklist (BEST-EFFORT)
        try
        {
            await _blacklist.AddAsync(request.Jti, TimeSpan.FromMinutes(15));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Blacklist (Redis) unavailable during logout. Continuing with DB revoke.");
        }

        // 2) ✅ DB revoke (KRITIK)
        try
        {
            await _refreshToken.RevokeAllByUserIdAsync(request.UserId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new ApiResponse<Unit> { Data = Unit.Value };
        }
        catch (SqlException ex)
        {
            // SQL Server unavailable -> 503 qaytaramiz (middleware ushlab oladi)
            _logger.LogError(ex, "Database SQqaraL error during logout.");
            throw new ServiceUnavailableException("Database unavailable. Logout could not be completed. Try again.", ex);
        }
        catch (DbUpdateException ex)
        {
            // SaveChanges error -> 503
            _logger.LogError(ex, "Database update error during logout.");
            throw new ServiceUnavailableException("Database unavailable. Logout could not be completed. Try again.", ex);
        }
        catch (TimeoutException ex)
        {
            // Timeout -> 503
            _logger.LogError(ex, "Timeout during logout.");
            throw new ServiceUnavailableException("Database timeout. Try again.", ex);
        }
        
        
        /*await _blacklist.AddAsync(
            request.Jti,
            TimeSpan.FromMinutes(15)
        );

        await _refreshToken.RevokeAllByUserIdAsync(request.UserId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ApiResponse<Unit>
        {
            Data = Unit.Value
        };*/
    }
}

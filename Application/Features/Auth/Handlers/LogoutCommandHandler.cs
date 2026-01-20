using Application.Commons;
using Application.Features.Auth.Commands;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Auth.Handlers;

public class LogoutCommandHandler(IJwtBlacklistService blacklist, IUnitOfWork unitOfWork,
    IRefreshTokenRepository refreshToken) : IRequestHandler<LogoutCommand, ApiResponse<Unit>>
{
    private readonly IJwtBlacklistService _blacklist = blacklist;
    private readonly IRefreshTokenRepository _refreshToken = refreshToken;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ApiResponse<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _blacklist.AddAsync(
            request.Jti,
            TimeSpan.FromMinutes(1) // Assuming access tokens are valid for 15 minutes
        );

        await _refreshToken.RevokeAllByUserIdAsync(request.UserId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ApiResponse<Unit>
        {
            Data = Unit.Value
        };
    }
}

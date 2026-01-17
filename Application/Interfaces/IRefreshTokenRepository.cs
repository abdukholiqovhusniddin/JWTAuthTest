

using Domain.Entities;

namespace Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetValidTokenAsync(string refreshtocen);
    Task RevokeAllByUserIdAsync(Guid userId);
}



namespace Application.Interfaces;

public interface IJwtBlacklistService
{
    Task AddAsync(string jti, TimeSpan timeSpan);
    Task<bool> ExistsAsync(string jti);
}

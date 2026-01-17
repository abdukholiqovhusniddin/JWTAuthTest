
using Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;

namespace Infrastructure.Redis;

public class JwtBlacklistService(IConnectionMultiplexer redis) : IJwtBlacklistService
{
    private readonly StackExchange.Redis.IDatabase _db = redis.GetDatabase();

    public async Task AddAsync(string jti, TimeSpan ttl) =>
        await _db.StringSetAsync(jti, "blacklisted", ttl);

    public async Task<bool> ExistsAsync(string jti) =>
        await _db.KeyExistsAsync(jti);
}

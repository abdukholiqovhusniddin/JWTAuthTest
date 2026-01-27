using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;
public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    private readonly AppDbContext _context = context;

    public async Task AddAsync(RefreshToken refreshToken) =>
        await _context.RefreshTokens.AddAsync(refreshToken);

    public async Task<RefreshToken?> GetValidTokenAsync(string refreshtocen) =>
        await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshtocen);

    public async Task RevokeAllByUserIdAsync(Guid userId,  CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
            token.IsRevoked = true;
    }
}

using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;
public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;

    public async Task AddAsync(User user) =>
        await _context.Users.AddAsync(user);

    public async Task<bool> ExistEmailAsync(string email) =>
        await _context.Users.AnyAsync(g => g.Email == email);
    public async Task<List<User>> GetAllUsers() =>
        await _context.Users.AsNoTracking().ToListAsync();

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByIdAsync(Guid id, bool noTracing = false)
    {
        var query = _context.Users;
        if (!noTracing) query.AsNoTracking();
        return await query.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}

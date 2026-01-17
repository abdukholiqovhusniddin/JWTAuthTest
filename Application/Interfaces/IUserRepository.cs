


using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<bool> ExistEmailAsync(string email);
    Task<List<User>> GetAllUsers();
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id, bool noTracing = false);
    Task UpdateAsync(User user);
}

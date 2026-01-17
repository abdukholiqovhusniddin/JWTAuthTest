using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public UserRole Role { get; set; } 

    public bool IsActive { get; set; } = true;

    public ICollection<RefreshToken> RefreshTokens { get; init; } = [];
}

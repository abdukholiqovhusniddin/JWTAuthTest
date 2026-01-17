using Domain.Common;

namespace Domain.Entities;
public class RefreshToken : BaseEntity
{
    public string Token { get; init; } = null!;

    public DateTime ExpiresAt { get; init; }

    public bool IsRevoked { get; set; } = false;

    public Guid UserId { get; init; }

    public User User { get; init; } = null!;
}

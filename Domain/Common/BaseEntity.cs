namespace Domain.Common;
public abstract class BaseEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}

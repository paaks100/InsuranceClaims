namespace InsuranceClaims.Models;

public abstract class BaseEntity
{
    public Guid Id { get; init; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

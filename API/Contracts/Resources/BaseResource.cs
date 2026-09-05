namespace InsuranceClaims.API.Contracts.Resources;

public abstract record BaseResource
{
    public Guid Id { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}

namespace InsuranceClaims.API.Contracts.Resources;

public record PolicyResource : BaseResource
{
    public required string PolicyNumber { get; init; }
    public required string InsuredName { get; init; }
    public required DateTimeOffset StartDate { get; init; }
    public required DateTimeOffset EndDate { get; init; }
}

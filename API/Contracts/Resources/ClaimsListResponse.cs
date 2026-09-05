namespace InsuranceClaims.API.Contracts.Resources;

public record ClaimsListResponse
{
    public required List<ClaimResource> Claims { get; init; }
    public required List<ClaimTotalsResource> Totals { get; init; }
}

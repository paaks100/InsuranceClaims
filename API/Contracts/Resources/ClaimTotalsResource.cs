using InsuranceClaims.Models.Enums;

namespace InsuranceClaims.API.Contracts.Resources;

public record ClaimTotalsResource
{
    public CurrencyCode Currency { get; init; }
    public decimal TotalEstimatedLoss { get; init; }
    public decimal TotalApproved { get; init; }
    public decimal TotalPaid { get; init; }
    public decimal TotalOutstanding { get; init; }
}

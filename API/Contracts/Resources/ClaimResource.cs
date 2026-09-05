using InsuranceClaims.Models.Enums;

namespace InsuranceClaims.API.Contracts.Resources;

public record ClaimResource : BaseResource
{
    public required string ClaimNumber { get; init; }
    public required PolicyResource Policy { get; init; }
    public required DateTimeOffset LossDate { get; init; }
    public required DateTimeOffset DateNotified { get; init; }
    public required LossNature LossNature { get; init; }

    public CurrencyCode Currency { get; init; }
    public required decimal EstimatedLossAmount { get; init; }
    public decimal? ApprovedAmount { get; init; }
    
    public decimal TotalPaid { get; init; }
    public decimal OutstandingBalance { get; init; }
    
    public ClaimStatus Status { get; init; }
    
    public required List<PaymentResource> Payments { get; init; }
}

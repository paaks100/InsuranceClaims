using InsuranceClaims.Models.Enums;

namespace InsuranceClaims.Models;

public class ClaimEntity : BaseEntity
{
    public required string ClaimNumber { get; set; }
    
    public required Guid PolicyId { get; set; }
    public PolicyEntity? Policy { get; set; }

    public required DateTimeOffset LossDate { get; set; }
    public required DateTimeOffset DateNotified { get; set; }
    public required LossNature LossNature { get; set; }

    public CurrencyCode Currency { get; set; }
    public required decimal EstimatedLossAmount { get; set; }
    public decimal? ApprovedAmount { get; set; }

    public ICollection<PaymentEntity> Payments { get; set; } = [];
}

using InsuranceClaims.Models.Enums;

namespace InsuranceClaims.Models;

public class PaymentEntity : BaseEntity
{
    public required Guid ClaimId { get; set; }
    public ClaimEntity? Claim { get; set; }
    
    public required DateTimeOffset PaymentDate { get; set; }
    
    public required decimal AmountOriginal { get; set; }
    public required CurrencyCode CurrencyOriginal { get; set; }
    
    public decimal ExchangeRate { get; set; }
    
    public required decimal AmountInClaimCurrency { get; set; }
}

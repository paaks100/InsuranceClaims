using InsuranceClaims.Models.Enums;

namespace InsuranceClaims.API.Contracts.Resources;

public record PaymentResource : BaseResource
{
    public required DateTimeOffset PaymentDate { get; init; }
    public required decimal AmountOriginal { get; init; }
    public required CurrencyCode CurrencyOriginal { get; init; }
    public decimal ExchangeRate { get; init; }
    public required decimal AmountInClaimCurrency { get; init; }
}

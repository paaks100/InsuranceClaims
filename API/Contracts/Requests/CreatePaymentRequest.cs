using System.ComponentModel.DataAnnotations;
using InsuranceClaims.Models.Enums;

namespace InsuranceClaims.API.Contracts.Requests;

public class CreatePaymentRequest
{
    [Required]
    public DateTimeOffset PaymentDate { get; init; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal AmountOriginal { get; init; }
    
    [Required]
    [EnumDataType(typeof(CurrencyCode))]
    public CurrencyCode CurrencyOriginal { get; init; }

    [Required]
    [Range(0.0000001, double.MaxValue)]
    public decimal ExchangeRate { get; init; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal AmountInClaimCurrency { get; init; }
}

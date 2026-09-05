using System.ComponentModel.DataAnnotations;
using InsuranceClaims.Models.Enums;

namespace InsuranceClaims.API.Contracts.Requests;

public class CreateClaimRequest : IValidatableObject
{
    [Required]
    public Guid PolicyId { get; init; }
    
    [Required]
    public DateTimeOffset LossDate { get; init; }
    
    [Required]
    public DateTimeOffset DateNotified { get; init; }
    
    [Required]
    [EnumDataType(typeof(LossNature))]
    public LossNature LossNature { get; init; }
    
    [Required]
    [EnumDataType(typeof(CurrencyCode))]
    public CurrencyCode Currency { get; init; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal EstimatedLossAmount { get; init; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (LossDate > DateNotified)
            yield return new ValidationResult(
                "DateNotified cannot be earlier than LossDate",
                [nameof(LossDate), nameof(DateNotified)]
            );
    }
}

using System.ComponentModel.DataAnnotations;

namespace InsuranceClaims.API.Contracts.Requests;

public class CreatePolicyRequest : IValidatableObject
{
    [Required]
    [StringLength(50)]
    public string PolicyNumber { get; init; } = null!;

    [Required]
    [StringLength(255)]
    public string InsuredName { get; init; } = null!;

    [Required]
    public DateTimeOffset StartDate { get; init; }

    [Required] public DateTimeOffset EndDate { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate >= EndDate)
            yield return new ValidationResult(
                "EndDate must be later than StartDate",
                [nameof(StartDate), nameof(EndDate)]
            );
    }
}

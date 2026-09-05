using System.ComponentModel.DataAnnotations;

namespace InsuranceClaims.API.Contracts.Requests;

public class ApproveClaimRequest
{
    [Required]
    [Range(0, double.MaxValue)]
    public decimal ApprovedAmount { get; init; }
}

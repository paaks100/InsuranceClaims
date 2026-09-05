using InsuranceClaims.Models.Enums;

namespace InsuranceClaims.API.Contracts.Requests;

public class GetClaimsRequest
{
    public DateTimeOffset? StartDate { get; init; }
    public DateTimeOffset? EndDate { get; init; }
    public CurrencyCode? Currency { get; init; }
    public ClaimStatus? Status { get; init; }
}

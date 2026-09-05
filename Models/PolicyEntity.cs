namespace InsuranceClaims.Models;

public class PolicyEntity : BaseEntity
{
    public required string PolicyNumber { get; set; }
    public required string InsuredName { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public required DateTimeOffset EndDate { get; set; }
    public ICollection<ClaimEntity> Claims { get; set; } = [];
}

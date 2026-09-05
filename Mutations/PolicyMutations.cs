using InsuranceClaims.API.Contracts.Requests;
using InsuranceClaims.Data;
using InsuranceClaims.Models;

namespace InsuranceClaims.Mutations;

public class PolicyMutations(ApplicationDbContext db)
{
    public async Task<PolicyEntity> CreatePolicyAsync(
        CreatePolicyRequest payload,
        CancellationToken cancellationToken = default
    )
    {
        var policy = new PolicyEntity
        {
            Id = Guid.NewGuid(),
            PolicyNumber = payload.PolicyNumber,
            InsuredName = payload.InsuredName,
            StartDate = payload.StartDate,
            EndDate = payload.EndDate
        };

        db.Policies.Add(policy);
        await db.SaveChangesAsync(cancellationToken);
        return policy;
    }
}

using InsuranceClaims.Data;
using InsuranceClaims.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceClaims.Queries;

public class PolicyQueries(ApplicationDbContext db)
{
    public async Task<List<PolicyEntity>> GetAllPoliciesAsync(CancellationToken cancellationToken = default)
    {
        return await db.Policies
            .OrderBy(p => p.PolicyNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<PolicyEntity?> GetPolicyByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        return await db.Policies.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> PolicyExistsForPolicyNumberAsync(
        string policyNumber,
        CancellationToken cancellationToken = default
    )
    {
        return await db.Policies.AnyAsync(p => p.PolicyNumber == policyNumber, cancellationToken);
    }
}

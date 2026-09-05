using InsuranceClaims.API.Contracts.Requests;
using InsuranceClaims.Data;
using InsuranceClaims.Models;
using InsuranceClaims.Models.Enums;

namespace InsuranceClaims.Mutations;

public class ClaimMutations(ApplicationDbContext db)
{
    public async Task<ClaimEntity> CreateClaimAsync(
        CreateClaimRequest payload,
        CancellationToken cancellationToken = default
    )
    {
        // Generate a claim number (you can customize this)
        var claimNumber = $"CLM-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

        var claim = new ClaimEntity
        {
            Id = Guid.NewGuid(),
            ClaimNumber = claimNumber,
            PolicyId = payload.PolicyId,
            LossDate = payload.LossDate,
            DateNotified = payload.DateNotified,
            LossNature = payload.LossNature,
            Currency = payload.Currency,
            EstimatedLossAmount = payload.EstimatedLossAmount,
        };

        db.Claims.Add(claim);
        await db.SaveChangesAsync(cancellationToken);
        return claim;
    }

    public async Task<ClaimEntity> SetApprovedAmountAsync(
        ClaimEntity claim,
        ApproveClaimRequest payload,
        CancellationToken cancellationToken = default
    )
    {
        claim.ApprovedAmount = payload.ApprovedAmount;
        await db.SaveChangesAsync(cancellationToken);
        return claim;
    }
}

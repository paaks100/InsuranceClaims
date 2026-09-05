using InsuranceClaims.API.Contracts.Requests;
using InsuranceClaims.Data;
using InsuranceClaims.Helpers;
using InsuranceClaims.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceClaims.Queries;

public class ClaimQueries(ApplicationDbContext db)
{
    public async Task<ClaimEntity?> GetClaimByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await BuildBaseQuery()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<ClaimEntity>> GetClaimsAsync(
        GetClaimsRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var query = BuildBaseQuery();

        if (request.StartDate.HasValue)
            query = query.Where(c => c.DateNotified >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(c => c.DateNotified <= request.EndDate.Value);

        if (request.Currency.HasValue)
            query = query.Where(c => c.Currency == request.Currency.Value);

        var claims = await query.ToListAsync(cancellationToken);

        if (request.Status.HasValue)
            claims = claims
                .Where(c =>
                    ClaimCalculator.GetStatus(c.ApprovedAmount, ClaimCalculator.GetTotalPaid(c.Payments))
                    == request.Status
                )
                .ToList();

        return claims;
    }

    private IQueryable<ClaimEntity> BuildBaseQuery()
    {
        return db.Claims
            .Include(c => c.Policy)
            .Include(c => c.Payments);
    }
}

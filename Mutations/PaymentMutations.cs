using InsuranceClaims.API.Contracts.Requests;
using InsuranceClaims.Data;
using InsuranceClaims.Models;

namespace InsuranceClaims.Mutations;

public class PaymentMutations(ApplicationDbContext db)
{
    public async Task CreatePaymentAsync(
        ClaimEntity claim,
        CreatePaymentRequest payload,
        CancellationToken cancellationToken = default
    )
    {
        var payment = new PaymentEntity
        {
            Id = Guid.NewGuid(),
            ClaimId = claim.Id,
            PaymentDate = payload.PaymentDate,
            CurrencyOriginal = payload.CurrencyOriginal,
            AmountOriginal = payload.AmountOriginal,
            ExchangeRate = payload.ExchangeRate,
            AmountInClaimCurrency = payload.AmountInClaimCurrency
        };
        
        db.Payments.Add(payment);
        await db.SaveChangesAsync(cancellationToken);
    }
}

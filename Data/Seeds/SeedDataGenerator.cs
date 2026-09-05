using InsuranceClaims.Models;
using InsuranceClaims.Models.Enums;

namespace InsuranceClaims.Data.Seeds;

public static class SeedDataGenerator
{
    private static readonly DateTimeOffset BaseDate =
        new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    
    public static List<PolicyEntity> GeneratePolicies()
    {
        var policies = new List<PolicyEntity>();

        for (var i = 1; i <= 50; i++)
            policies.Add(new PolicyEntity
            {
                Id = Guid.Parse($"10000000-0000-0000-0000-{i:D12}"),
                PolicyNumber = $"POL-{1000 + i}",
                InsuredName = $"Insured Customer {i}",
                StartDate = BaseDate.AddMonths(-12),
                EndDate = BaseDate.AddMonths(12),
                CreatedAt = BaseDate,
                UpdatedAt = BaseDate
            });

        return policies;
    }

    public static List<ClaimEntity> GenerateClaims(List<PolicyEntity> policies)
    {
        var rand = new Random(42);

        var currencies = Enum.GetValues<CurrencyCode>();
        var lossNatures = Enum.GetValues<LossNature>();

        var claims = new List<ClaimEntity>();

        for (var i = 1; i <= 100; i++)
        {
            var policy = policies[rand.Next(policies.Count)];

            decimal estimated = rand.Next(1000, 150000);

            decimal? approved = i % 4 == 0
                ? null
                : Math.Round(estimated * (decimal)(0.65 + rand.NextDouble() * 0.35), 2);

            var lossDate = BaseDate.AddDays(-rand.Next(10, 300));
            
            claims.Add(new ClaimEntity
            {
                Id = Guid.Parse($"20000000-0000-0000-0000-{i:D12}"),
                ClaimNumber = $"CLM-{10000 + i}",
                PolicyId = policy.Id,
                LossDate = lossDate,
                DateNotified = lossDate.AddDays(rand.Next(1, 30)),
                Currency = currencies[rand.Next(currencies.Length)],
                LossNature = lossNatures[rand.Next(lossNatures.Length)],
                EstimatedLossAmount = estimated,
                ApprovedAmount = approved,
                CreatedAt = BaseDate,
                UpdatedAt = BaseDate
            });
        }

        return claims;
    }

    public static List<PaymentEntity> GeneratePayments(List<ClaimEntity> claims)
    {
        var rand = new Random(33);
        
        var currencies = Enum.GetValues<CurrencyCode>();

        var payments = new List<PaymentEntity>();

        var index = 1;

        foreach (var claim in claims)
        {
            if (!claim.ApprovedAmount.HasValue)
                continue;

            var paymentCount = rand.Next(0, 4);

            decimal totalPaid = 0;

            for (var i = 0; i < paymentCount; i++)
            {
                var payCurrency = currencies[rand.Next(currencies.Length)];

                var rate = payCurrency == claim.Currency
                    ? 1
                    : Math.Round((decimal)(0.5 + rand.NextDouble() * 12), 4);

                var amountClaimCurrency = Math.Round(
                    claim.ApprovedAmount.Value / paymentCount * (decimal)(0.7 + rand.NextDouble() * 0.3),
                    2
                );

                var original = Math.Round(amountClaimCurrency / rate, 2);

                totalPaid += amountClaimCurrency;

                payments.Add(new PaymentEntity
                {
                    Id = Guid.Parse($"30000000-0000-0000-0000-{index:D12}"),
                    ClaimId = claim.Id,
                    PaymentDate = BaseDate.AddDays(-rand.Next(1, 120)),
                    CurrencyOriginal = payCurrency,
                    AmountOriginal = original,
                    ExchangeRate = rate,
                    AmountInClaimCurrency = amountClaimCurrency,
                    CreatedAt = BaseDate,
                    UpdatedAt = BaseDate
                });

                index++;
            }
        }
        
        return payments;
    }
}

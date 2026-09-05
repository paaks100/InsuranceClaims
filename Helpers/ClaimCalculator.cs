using InsuranceClaims.Models;
using InsuranceClaims.Models.Enums;

namespace InsuranceClaims.Helpers;

public static class ClaimCalculator
{
    public static decimal GetTotalPaid(IEnumerable<PaymentEntity> payments) =>
        payments.Sum(p => p.AmountInClaimCurrency);

    public static decimal GetOutstandingBalance(decimal? approvedAmount, decimal totalPaid) =>
        (approvedAmount ?? 0) - totalPaid;

    public static ClaimStatus GetStatus(decimal? approvedAmount, decimal totalPaid)
    {
        if (approvedAmount is null)
            return ClaimStatus.Reserved;

        if (approvedAmount == 0)
            return ClaimStatus.Denied;

        return approvedAmount - totalPaid > 0
            ? ClaimStatus.SettledPaymentOutstanding
            : ClaimStatus.SettledAndPaid;
    }
}

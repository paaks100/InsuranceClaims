using AutoMapper;
using InsuranceClaims.API.Contracts.Resources;
using InsuranceClaims.Helpers;
using InsuranceClaims.Models;

namespace InsuranceClaims.Mapping.Resolvers;

public class ClaimResourceResolver : ITypeConverter<ClaimEntity, ClaimResource>
{
    public ClaimResource Convert(ClaimEntity source, ClaimResource destination, ResolutionContext context)
    {
        var totalPaid = ClaimCalculator.GetTotalPaid(source.Payments);

        return new ClaimResource
        {
            Id = source.Id,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,

            ClaimNumber = source.ClaimNumber,
            Policy = context.Mapper.Map<PolicyResource>(source.Policy),

            LossDate = source.LossDate,
            DateNotified = source.DateNotified,
            LossNature = source.LossNature,

            Currency = source.Currency,

            EstimatedLossAmount = source.EstimatedLossAmount,
            ApprovedAmount = source.ApprovedAmount,

            TotalPaid = totalPaid,

            OutstandingBalance = ClaimCalculator.GetOutstandingBalance(source.ApprovedAmount, totalPaid),

            Status = ClaimCalculator.GetStatus(source.ApprovedAmount, totalPaid),

            Payments = context.Mapper.Map<List<PaymentResource>>(source.Payments)
        };
    }
}

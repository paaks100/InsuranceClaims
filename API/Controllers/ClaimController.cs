using AutoMapper;
using InsuranceClaims.API.Contracts.Requests;
using InsuranceClaims.API.Contracts.Resources;
using InsuranceClaims.API.Responses;
using InsuranceClaims.Helpers;
using InsuranceClaims.Models;
using InsuranceClaims.Mutations;
using InsuranceClaims.Queries;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceClaims.API.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/claims")]
public class ClaimController(
    ClaimMutations claimMutations,
    ClaimQueries claimQueries,
    PaymentMutations paymentMutations,
    PolicyQueries policyQueries,
    IMapper mapper
) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(ClaimResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterClaimAsync(
        [FromBody] CreateClaimRequest request,
        CancellationToken cancellationToken
    )
    {
        var policy = await policyQueries.GetPolicyByIdAsync(request.PolicyId, cancellationToken);

        if (policy is null)
            return ErrorResponse.BadRequestResult("Policy not found");

        if (request.LossDate < policy.StartDate || request.LossDate > policy.EndDate)
            return ErrorResponse.BadRequestResult("Loss date falls outside policy coverage period.");

        var claim = await claimMutations.CreateClaimAsync(request, cancellationToken);
        var resource = mapper.Map<ClaimResource>(claim);

        return Created(string.Empty, resource);
    }

    [HttpGet("{id:guid}/retrieve")]
    [ProducesResponseType(typeof(ClaimResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RetrieveClaimByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var claim = await claimQueries.GetClaimByIdAsync(id, cancellationToken);

        if (claim is null)
            return ErrorResponse.NotFoundResult("No claim found matching the provided id.");

        var resource = mapper.Map<ClaimResource>(claim);
        return Ok(resource);
    }

    [HttpPatch("{id:guid}/approve")]
    [ProducesResponseType(typeof(ClaimResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveClaimAsync(
        Guid id,
        [FromBody] ApproveClaimRequest request,
        CancellationToken cancellationToken
    )
    {
        var claim = await claimQueries.GetClaimByIdAsync(id, cancellationToken);

        if (claim is null)
            return ErrorResponse.NotFoundResult("No claim found matching the provided id.");

        var updatedClaim = await claimMutations.SetApprovedAmountAsync(claim, request, cancellationToken);
        var resource = mapper.Map<ClaimResource>(updatedClaim);
        return Ok(resource);
    }

    [HttpPost("{id:guid}/payments/add")]
    [ProducesResponseType(typeof(ClaimResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddPaymentToClaimAsync(
        Guid id,
        CreatePaymentRequest request,
        CancellationToken cancellationToken
    )
    {
        var claim = await claimQueries.GetClaimByIdAsync(id, cancellationToken);

        if (claim is null)
            return ErrorResponse.NotFoundResult("No claim found matching the provided id.");

        if (!claim.ApprovedAmount.HasValue)
            return ErrorResponse.BadRequestResult("Claim must be approved before payment.");

        var currentPaid = ClaimCalculator.GetTotalPaid(claim.Payments);
        if (currentPaid + request.AmountInClaimCurrency > claim.ApprovedAmount)
            return ErrorResponse.BadRequestResult("Payment exceeds approved amount.");
        
        var convertedAmount = Math.Round(request.AmountOriginal * request.ExchangeRate, 2);

        if (Math.Abs(convertedAmount - request.AmountInClaimCurrency) > 0.01m)
            return ErrorResponse.BadRequestResult("AmountInClaimCurrency does not match exchange calculation.");

        await paymentMutations.CreatePaymentAsync(claim, request, cancellationToken);

        var updatedClaim = await claimQueries.GetClaimByIdAsync(id, cancellationToken);
        var resource = mapper.Map<ClaimResource>(updatedClaim);
        return Created(string.Empty, resource);
    }

    [HttpGet("list")]
    [ProducesResponseType(typeof(ClaimsListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RetrieveAllClaimsAsync(
        [FromQuery] GetClaimsRequest request,
        CancellationToken cancellationToken
    )
    {
        var claims = await claimQueries.GetClaimsAsync(request, cancellationToken);
        var claimResources = mapper.Map<List<ClaimResource>>(claims);

        var totals = BuildTotals(claims);

        return Ok(new ClaimsListResponse
        {
            Claims = claimResources,
            Totals = totals
        });
    }

    private static List<ClaimTotalsResource> BuildTotals(IEnumerable<ClaimEntity> claims)
    {
        return claims
            .GroupBy(c=>c.Currency)
            .Select(g =>
            {
                var totalApproved = g.Sum(c => c.ApprovedAmount ?? 0);
                
                var totalPaid = g.Sum(c => ClaimCalculator.GetTotalPaid(c.Payments));

                return new ClaimTotalsResource
                {
                    Currency = g.Key,
                    TotalEstimatedLoss = g.Sum(c => c.EstimatedLossAmount),
                    TotalApproved = totalApproved,
                    TotalPaid = totalPaid,
                    TotalOutstanding = totalApproved - totalPaid
                };
            })
            .ToList();
    }
}

using AutoMapper;
using InsuranceClaims.API.Contracts.Requests;
using InsuranceClaims.API.Contracts.Resources;
using InsuranceClaims.API.Responses;
using InsuranceClaims.Mutations;
using InsuranceClaims.Queries;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceClaims.API.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/policies")]
public class PolicyController(
    PolicyMutations policyMutations,
    PolicyQueries policyQueries,
    IMapper mapper
) : ControllerBase
{
    [HttpGet("list")]
    [ProducesResponseType(typeof(List<PolicyResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAllPoliciesAsync(CancellationToken cancellationToken)
    {
        var policies = await policyQueries.GetAllPoliciesAsync(cancellationToken);
        var resources = mapper.Map<List<PolicyResource>>(policies);
        return Ok(resources);
    }

    [HttpGet("{id:guid}/retrieve")]
    [ProducesResponseType(typeof(PolicyResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RetrievePolicyByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var policy = await policyQueries.GetPolicyByIdAsync(id, cancellationToken);

        if (policy is null)
            return ErrorResponse.NotFoundResult("No policy found matching the provided id.");

        var resource = mapper.Map<PolicyResource>(policy);
        return Ok(resource);
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(PolicyResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterPolicy(
        [FromBody] CreatePolicyRequest request,
        CancellationToken cancellationToken
    )
    {
        var exists = await policyQueries.PolicyExistsForPolicyNumberAsync(
            request.PolicyNumber,
            cancellationToken
        );

        if (exists)
            return ErrorResponse.ConflictResult("Policy with policy number already exists.");

        var policy = await policyMutations.CreatePolicyAsync(request, cancellationToken);
        var resource = mapper.Map<PolicyResource>(policy);

        return Created(string.Empty, resource);
    }
}

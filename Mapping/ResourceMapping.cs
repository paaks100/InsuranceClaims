using AutoMapper;
using InsuranceClaims.API.Contracts.Resources;
using InsuranceClaims.Mapping.Resolvers;
using InsuranceClaims.Models;

namespace InsuranceClaims.Mapping;

public class ResourceMapping : Profile
{
    public ResourceMapping()
    {
        CreateMap<PolicyEntity, PolicyResource>();
        
        CreateMap<PaymentEntity, PaymentResource>();

        CreateMap<ClaimEntity, ClaimResource>().ConvertUsing<ClaimResourceResolver>();
    }
}

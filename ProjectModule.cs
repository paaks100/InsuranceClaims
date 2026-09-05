using InsuranceClaims.Mutations;
using InsuranceClaims.Queries;

namespace InsuranceClaims;

public static class ProjectModule
{
    public static IServiceCollection AddProjectModule(this IServiceCollection services)
    {
        services.AddScoped<ClaimMutations>();
        services.AddScoped<PaymentMutations>();
        services.AddScoped<PolicyMutations>();
        services.AddScoped<ClaimQueries>();
        services.AddScoped<PolicyQueries>();
        
        return services;
    }
}

using InsuranceClaims.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceClaims.Data.Configurations;

public class PolicyEntityConfiguration : IEntityTypeConfiguration<PolicyEntity>
{
    public void Configure(EntityTypeBuilder<PolicyEntity> builder)
    {
        builder.ToTable("Policies");

        builder.HasIndex(p => p.PolicyNumber).IsUnique();

        builder.Property(p => p.PolicyNumber).HasMaxLength(50);
        builder.Property(p => p.InsuredName).HasMaxLength(255);
    }
}

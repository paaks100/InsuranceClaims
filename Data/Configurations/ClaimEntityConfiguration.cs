using InsuranceClaims.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceClaims.Data.Configurations;

public class ClaimEntityConfiguration : IEntityTypeConfiguration<ClaimEntity>
{
    public void Configure(EntityTypeBuilder<ClaimEntity> builder)
    {
        builder.ToTable("Claims");

        builder.HasIndex(c => c.ClaimNumber).IsUnique();

        builder.Property(c => c.ClaimNumber).HasMaxLength(50);
        builder.Property(c => c.EstimatedLossAmount).HasPrecision(18, 2);
        builder.Property(c => c.ApprovedAmount).HasPrecision(18, 2);
        builder.Property(c => c.Currency).HasConversion<string>().HasMaxLength(3);
        builder.Property(c => c.LossNature).HasConversion<string>().HasMaxLength(50);

        builder
            .HasOne(c => c.Policy)
            .WithMany(p => p.Claims)
            .HasForeignKey(c => c.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using InsuranceClaims.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceClaims.Data.Configurations;

public class PaymentEntityConfiguration : IEntityTypeConfiguration<PaymentEntity>
{
    public void Configure(EntityTypeBuilder<PaymentEntity> builder)
    {
        builder.ToTable("Payments");

        builder.HasIndex(p => p.ClaimId);

        builder.Property(p => p.CurrencyOriginal).HasConversion<string>().HasMaxLength(3);
        builder.Property(p => p.AmountOriginal).HasPrecision(18, 2);
        builder.Property(p => p.AmountInClaimCurrency).HasPrecision(18, 8);
        builder.Property(p => p.ExchangeRate).HasPrecision(18, 8);

        builder
            .HasOne(p => p.Claim)
            .WithMany(p => p.Payments)
            .HasForeignKey(p => p.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

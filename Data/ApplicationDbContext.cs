using InsuranceClaims.Data.Seeds;
using InsuranceClaims.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InsuranceClaims.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<PolicyEntity> Policies => Set<PolicyEntity>();
    public DbSet<ClaimEntity> Claims => Set<ClaimEntity>();
    public DbSet<PaymentEntity> Payments => Set<PaymentEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        var policies = SeedDataGenerator.GeneratePolicies();
        var claims = SeedDataGenerator.GenerateClaims(policies);
        var payments = SeedDataGenerator.GeneratePayments(claims);

        modelBuilder.Entity<PolicyEntity>().HasData(policies);
        modelBuilder.Entity<ClaimEntity>().HasData(claims);
        modelBuilder.Entity<PaymentEntity>().HasData(payments);

        var dateTimeOffsetConverter = new ValueConverter<DateTimeOffset, long>(
            v => v.UtcDateTime.Ticks,
            v => new DateTimeOffset(v, TimeSpan.Zero));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        foreach (var property in entityType.GetProperties())
        {
            if (property.ClrType == typeof(DateTimeOffset))
                property.SetValueConverter(dateTimeOffsetConverter);

            else if (property.ClrType == typeof(DateTimeOffset?))
                property.SetValueConverter(new ValueConverter<DateTimeOffset?, long?>(
                    v => v.HasValue ? v.Value.UtcDateTime.Ticks : null,
                    v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null));
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                    entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

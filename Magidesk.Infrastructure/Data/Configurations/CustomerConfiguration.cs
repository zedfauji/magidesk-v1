using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Customer entity.
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers", "magidesk");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .IsRequired();

        builder.Property(c => c.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Phone)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(150);

        builder.Property(c => c.Address)
            .HasMaxLength(250);

        builder.Property(c => c.City)
            .HasMaxLength(100);

        builder.Property(c => c.PostalCode)
            .HasMaxLength(20);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.LastVisitAt);

        builder.Property(c => c.TotalVisits)
            .IsRequired()
            .HasDefaultValue(0);

        // Money value object - stored as decimal with currency
        builder.OwnsOne(c => c.TotalSpent, ts =>
        {
            ts.Property(s => s.Amount)
                .HasColumnName("TotalSpentAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            ts.Property(s => s.Currency)
                .HasColumnName("TotalSpentCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("USD")
                .HasConversion(v => v, v => string.IsNullOrWhiteSpace(v) ? "USD" : v)
                .IsRequired();
        });

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(c => c.Phone)
            .IsUnique();

        builder.HasIndex(c => c.Email);
        
        builder.HasIndex(c => c.FirstName);
        builder.HasIndex(c => c.LastName);
    }
}

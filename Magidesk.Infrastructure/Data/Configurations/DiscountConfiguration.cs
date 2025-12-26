using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Discount (reference data).
/// </summary>
public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable("Discounts");

        builder.HasKey(d => d.Id);

        // Properties
        builder.Property(d => d.Id)
            .IsRequired();

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(d => d.Value)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.OwnsOne(d => d.MinimumBuy, mb =>
        {
            mb.Property(m => m.Amount)
                .HasColumnName("MinimumBuy")
                .HasPrecision(18, 2);
            mb.Property(m => m.Currency)
                .HasColumnName("MinimumBuyCurrency")
                .HasMaxLength(3);
        });

        builder.Property(d => d.MinimumQuantity);

        builder.Property(d => d.QualificationType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(d => d.ApplicationType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(d => d.AutoApply)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.CouponCode)
            .HasMaxLength(50);

        builder.Property(d => d.ExpirationDate);

        // Indexes
        builder.HasIndex(d => d.IsActive);
        
        builder.HasIndex(d => d.CouponCode)
            .IsUnique()
            .HasFilter("\"CouponCode\" IS NOT NULL");
    }
}


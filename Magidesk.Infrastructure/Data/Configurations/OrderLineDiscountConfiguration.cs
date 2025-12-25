using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for OrderLineDiscount.
/// </summary>
public class OrderLineDiscountConfiguration : IEntityTypeConfiguration<OrderLineDiscount>
{
    public void Configure(EntityTypeBuilder<OrderLineDiscount> builder)
    {
        builder.ToTable("OrderLineDiscounts");

        builder.HasKey(old => old.Id);

        // Properties
        builder.Property(old => old.Id)
            .IsRequired();

        builder.Property(old => old.OrderLineId)
            .IsRequired();

        builder.Property(old => old.DiscountId)
            .IsRequired();

        builder.Property(old => old.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(old => old.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(old => old.Value)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(old => old.MinimumQuantity);

        builder.OwnsOne(old => old.Amount, da =>
        {
            da.Property(d => d.Amount)
                .HasColumnName("DiscountAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            da.Property(d => d.Currency)
                .HasColumnName("DiscountAmountCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(old => old.AutoApply)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(old => old.AppliedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(old => old.OrderLineId);
    }
}


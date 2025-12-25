using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for OrderLineModifier.
/// </summary>
public class OrderLineModifierConfiguration : IEntityTypeConfiguration<OrderLineModifier>
{
    public void Configure(EntityTypeBuilder<OrderLineModifier> builder)
    {
        builder.ToTable("OrderLineModifiers");

        builder.HasKey(olm => olm.Id);

        // Properties
        builder.Property(olm => olm.Id)
            .IsRequired();

        builder.Property(olm => olm.OrderLineId)
            .IsRequired();

        builder.Property(olm => olm.ModifierId)
            .IsRequired();

        builder.Property(olm => olm.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(olm => olm.ModifierType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(olm => olm.ItemCount)
            .IsRequired();

        builder.OwnsOne(olm => olm.UnitPrice, up =>
        {
            up.Property(u => u.Amount)
                .HasColumnName("UnitPrice")
                .HasPrecision(18, 2)
                .IsRequired();
            up.Property(u => u.Currency)
                .HasColumnName("UnitPriceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(olm => olm.TaxRate)
            .HasPrecision(5, 4)
            .IsRequired()
            .HasDefaultValue(0m);

        builder.OwnsOne(olm => olm.SubtotalAmount, sa =>
        {
            sa.Property(s => s.Amount)
                .HasColumnName("SubtotalAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            sa.Property(s => s.Currency)
                .HasColumnName("SubtotalCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(olm => olm.TaxAmount, ta =>
        {
            ta.Property(t => t.Amount)
                .HasColumnName("TaxAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            ta.Property(t => t.Currency)
                .HasColumnName("TaxCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(olm => olm.TotalAmount, ta =>
        {
            ta.Property(t => t.Amount)
                .HasColumnName("TotalAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            ta.Property(t => t.Currency)
                .HasColumnName("TotalCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(olm => olm.ShouldPrintToKitchen)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(olm => olm.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(olm => olm.OrderLineId);
    }
}


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for OrderLine.
/// </summary>
public class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.ToTable("OrderLines");

        builder.HasKey(ol => ol.Id);

        // Properties
        builder.Property(ol => ol.Id)
            .IsRequired();

        builder.Property(ol => ol.TicketId)
            .IsRequired();

        builder.Property(ol => ol.MenuItemId)
            .IsRequired();

        builder.Property(ol => ol.MenuItemName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ol => ol.CategoryName)
            .HasMaxLength(100);

        builder.Property(ol => ol.GroupName)
            .HasMaxLength(100);

        builder.Property(ol => ol.Quantity)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(ol => ol.ItemCount)
            .IsRequired();

        builder.Property(ol => ol.ItemUnitName)
            .HasMaxLength(50);

        builder.Property(ol => ol.IsFractionalUnit)
            .IsRequired()
            .HasDefaultValue(false);

        // Money value objects
        builder.OwnsOne(ol => ol.UnitPrice, up =>
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

        builder.OwnsOne(ol => ol.SubtotalAmount, sa =>
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

        builder.OwnsOne(ol => ol.SubtotalAmountWithoutModifiers, sawm =>
        {
            sawm.Property(s => s.Amount)
                .HasColumnName("SubtotalAmountWithoutModifiers")
                .HasPrecision(18, 2)
                .IsRequired();
            sawm.Property(s => s.Currency)
                .HasColumnName("SubtotalWithoutModifiersCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(ol => ol.DiscountAmount, da =>
        {
            da.Property(d => d.Amount)
                .HasColumnName("DiscountAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            da.Property(d => d.Currency)
                .HasColumnName("DiscountCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(ol => ol.TaxRate)
            .HasPrecision(5, 4)
            .IsRequired()
            .HasDefaultValue(0m);

        builder.OwnsOne(ol => ol.TaxAmount, ta =>
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

        builder.OwnsOne(ol => ol.TaxAmountWithoutModifiers, tam =>
        {
            tam.Property(t => t.Amount)
                .HasColumnName("TaxAmountWithoutModifiers")
                .HasPrecision(18, 2)
                .IsRequired();
            tam.Property(t => t.Currency)
                .HasColumnName("TaxWithoutModifiersCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(ol => ol.TotalAmount, ta =>
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

        builder.OwnsOne(ol => ol.TotalAmountWithoutModifiers, tawm =>
        {
            tawm.Property(t => t.Amount)
                .HasColumnName("TotalAmountWithoutModifiers")
                .HasPrecision(18, 2)
                .IsRequired();
            tawm.Property(t => t.Currency)
                .HasColumnName("TotalWithoutModifiersCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(ol => ol.IsBeverage)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ol => ol.ShouldPrintToKitchen)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ol => ol.PrintedToKitchen)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ol => ol.SeatNumber);

        builder.Property(ol => ol.TreatAsSeat)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ol => ol.CreatedAt)
            .IsRequired();

        // Relationships
        // Note: Modifiers and AddOns are both OrderLineModifier entities stored in the same table
        // They are distinguished by ModifierType in domain logic
        // We configure Modifiers navigation to use its backing field
        builder.HasMany(ol => ol.Modifiers)
            .WithOne()
            .HasForeignKey(olm => olm.OrderLineId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Set the backing field for the Modifiers navigation property
        builder.Metadata.FindNavigation(nameof(OrderLine.Modifiers))?.SetField("_modifiers");
        
        // Ignore AddOns navigation - it will be populated from Modifiers by domain logic after loading
        builder.Ignore(ol => ol.AddOns);

        builder.HasMany(ol => ol.Discounts)
            .WithOne()
            .HasForeignKey(old => old.OrderLineId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ol => ol.TicketId);
        builder.HasIndex(ol => ol.MenuItemId);
    }
}


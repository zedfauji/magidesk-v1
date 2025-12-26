using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for MenuModifier entity.
/// </summary>
public class MenuModifierConfiguration : IEntityTypeConfiguration<MenuModifier>
{
    public void Configure(EntityTypeBuilder<MenuModifier> builder)
    {
        builder.ToTable("MenuModifiers", "magidesk");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .IsRequired();

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Description)
            .HasMaxLength(500);

        builder.Property(m => m.ModifierGroupId);

        builder.Property(m => m.ModifierType)
            .HasConversion<int>()
            .IsRequired();

        builder.OwnsOne(m => m.BasePrice, bp =>
        {
            bp.Property(b => b.Amount)
                .HasColumnName("BasePrice")
                .HasPrecision(18, 2)
                .IsRequired();
            bp.Property(b => b.Currency)
                .HasColumnName("BasePriceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(m => m.TaxRate)
            .HasPrecision(5, 4)
            .IsRequired()
            .HasDefaultValue(0m);

        builder.Property(m => m.ShouldPrintToKitchen)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(m => m.IsSectionWisePrice)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.SectionName)
            .HasMaxLength(50);

        builder.Property(m => m.MultiplierName)
            .HasMaxLength(50);

        builder.Property(m => m.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(m => m.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(m => m.Version)
            .IsRequired()
            .HasDefaultValue(1);

        // Indexes
        builder.HasIndex(m => m.Name);
        builder.HasIndex(m => m.ModifierGroupId);
        builder.HasIndex(m => m.IsActive);
        builder.HasIndex(m => m.DisplayOrder);
    }
}


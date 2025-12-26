using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for ModifierGroup entity.
/// </summary>
public class ModifierGroupConfiguration : IEntityTypeConfiguration<ModifierGroup>
{
    public void Configure(EntityTypeBuilder<ModifierGroup> builder)
    {
        builder.ToTable("ModifierGroups", "magidesk");

        builder.HasKey(mg => mg.Id);

        builder.Property(mg => mg.Id)
            .IsRequired();

        builder.Property(mg => mg.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(mg => mg.Description)
            .HasMaxLength(500);

        builder.Property(mg => mg.IsRequired)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(mg => mg.MinSelections)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(mg => mg.MaxSelections)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(mg => mg.AllowMultipleSelections)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(mg => mg.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(mg => mg.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(mg => mg.Version)
            .IsRequired()
            .HasDefaultValue(1);

        // Indexes
        builder.HasIndex(mg => mg.Name)
            .IsUnique();

        builder.HasIndex(mg => mg.IsActive);
        builder.HasIndex(mg => mg.DisplayOrder);
    }
}


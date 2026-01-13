using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for Equipment entity.
/// </summary>
public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("Equipment", "magidesk");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (EquipmentType)Enum.Parse(typeof(EquipmentType), v));

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (EquipmentStatus)Enum.Parse(typeof(EquipmentStatus), v));

        builder.Property(e => e.AssignedTableId);

        builder.Property(e => e.LastMaintenanceDate);

        builder.Property(e => e.NextMaintenanceDate);

        builder.Property(e => e.IsActive)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(e => e.Type);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.AssignedTableId);
        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.NextMaintenanceDate);
    }
}
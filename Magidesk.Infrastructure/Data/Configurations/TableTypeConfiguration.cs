using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for TableType entity.
/// </summary>
public class TableTypeConfiguration : IEntityTypeConfiguration<TableType>
{
    public void Configure(EntityTypeBuilder<TableType> builder)
    {
        builder.ToTable("TableTypes", "magidesk");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .IsRequired();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.HourlyRate)
            .IsRequired()
            .HasPrecision(10, 2); // Decimal precision for currency

        builder.Property(t => t.FirstHourRate)
            .HasPrecision(10, 2);

        builder.Property(t => t.MinimumMinutes)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(t => t.RoundingMinutes)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(t => t.Name)
            .IsUnique();

        builder.HasIndex(t => t.IsActive);
    }
}

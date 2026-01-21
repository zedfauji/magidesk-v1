using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for Shift entity.
/// </summary>
public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.ToTable("Shifts");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.StartTime)
            .IsRequired()
            .HasConversion(
                v => v.Ticks,
                v => new TimeSpan(v));

        builder.Property(s => s.EndTime)
            .IsRequired()
            .HasConversion(
                v => v.Ticks,
                v => new TimeSpan(v));

        builder.Property(s => s.IsActive)
            .IsRequired();

        builder.Property(s => s.Version)
            .IsRequired()
            .HasDefaultValue(1);

        // Index for active shifts
        builder.HasIndex(s => s.IsActive)
            .HasFilter("\"IsActive\" = true");
    }
}


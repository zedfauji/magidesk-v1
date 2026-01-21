using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Infrastructure.Services;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for PerformanceMetricEntity.
/// </summary>
public class PerformanceMetricEntityConfiguration : IEntityTypeConfiguration<PerformanceMetricEntity>
{
    public void Configure(EntityTypeBuilder<PerformanceMetricEntity> builder)
    {
        builder.ToTable("PerformanceMetrics", "magidesk");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Value)
            .IsRequired()
            .HasPrecision(18, 6);

        builder.Property(p => p.Timestamp)
            .IsRequired();

        builder.Property(p => p.Tags)
            .HasMaxLength(500);

        // Indexes for efficient querying
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.Timestamp);

        // Composite index for metric queries
        builder.HasIndex(p => new { p.Name, p.Timestamp })
            .HasDatabaseName("IX_PerformanceMetrics_Name_Timestamp");
    }
}
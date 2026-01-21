using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Application.Interfaces;
using Magidesk.Infrastructure.Services;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for AlertEntity.
/// </summary>
public class AlertEntityConfiguration : IEntityTypeConfiguration<AlertEntity>
{
    public void Configure(EntityTypeBuilder<AlertEntity> builder)
    {
        builder.ToTable("Alerts", "magidesk");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .IsRequired();

        builder.Property(a => a.Type)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (AlertType)Enum.Parse(typeof(AlertType), v));

        builder.Property(a => a.Severity)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (AlertSeverity)Enum.Parse(typeof(AlertSeverity), v));

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(a => a.EntityId);

        builder.Property(a => a.EntityType)
            .HasMaxLength(100);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.AcknowledgedAt);

        builder.Property(a => a.AcknowledgedBy);

        builder.Property(a => a.ResolvedAt);

        builder.Property(a => a.ResolvedBy);

        builder.Property(a => a.Resolution)
            .HasMaxLength(1000);

        builder.Property(a => a.ExpiresAt);

        builder.Property(a => a.IsActive)
            .IsRequired();

        // Indexes for efficient querying
        builder.HasIndex(a => a.Type);
        builder.HasIndex(a => a.Severity);
        builder.HasIndex(a => a.IsActive);
        builder.HasIndex(a => a.CreatedAt);
        builder.HasIndex(a => a.ExpiresAt);
        builder.HasIndex(a => a.EntityId);

        // Composite index for active alerts
        builder.HasIndex(a => new { a.IsActive, a.Severity, a.CreatedAt })
            .HasDatabaseName("IX_Alerts_Active_Severity_Created");
    }
}
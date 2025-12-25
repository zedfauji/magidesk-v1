using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for AuditEvent.
/// </summary>
public class AuditEventConfiguration : IEntityTypeConfiguration<AuditEvent>
{
    public void Configure(EntityTypeBuilder<AuditEvent> builder)
    {
        builder.ToTable("AuditEvents");

        builder.HasKey(ae => ae.Id);

        // Properties
        builder.Property(ae => ae.Id)
            .IsRequired();

        builder.Property(ae => ae.EventType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(ae => ae.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ae => ae.EntityId)
            .IsRequired();

        builder.Property(ae => ae.UserId)
            .IsRequired();

        builder.Property(ae => ae.BeforeState)
            .HasMaxLength(5000);

        builder.Property(ae => ae.AfterState)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(ae => ae.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(ae => ae.Timestamp)
            .IsRequired();

        builder.Property(ae => ae.CorrelationId);

        // Indexes
        builder.HasIndex(ae => new { ae.EntityType, ae.EntityId });
        builder.HasIndex(ae => ae.CorrelationId)
            .HasFilter("\"CorrelationId\" IS NOT NULL");
        builder.HasIndex(ae => ae.Timestamp);
        builder.HasIndex(ae => ae.UserId);
    }
}


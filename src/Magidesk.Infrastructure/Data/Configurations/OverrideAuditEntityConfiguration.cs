using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Infrastructure.Repositories;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for OverrideAuditEntity.
/// </summary>
public class OverrideAuditEntityConfiguration : IEntityTypeConfiguration<OverrideAuditEntity>
{
    public void Configure(EntityTypeBuilder<OverrideAuditEntity> builder)
    {
        builder.ToTable("OverrideAuditEntries");

        builder.HasKey(oae => oae.Id);

        // Properties
        builder.Property(oae => oae.Id)
            .IsRequired();

        builder.Property(oae => oae.SessionId)
            .IsRequired();

        builder.Property(oae => oae.OverrideType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(oae => oae.OriginalValue)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(oae => oae.NewValue)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(oae => oae.Reason)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(oae => oae.ManagerId)
            .IsRequired();

        builder.Property(oae => oae.Timestamp)
            .IsRequired();

        // Indexes for efficient querying
        builder.HasIndex(oae => oae.SessionId);
        builder.HasIndex(oae => oae.ManagerId);
        builder.HasIndex(oae => oae.Timestamp);
        builder.HasIndex(oae => new { oae.OverrideType, oae.Timestamp });
        builder.HasIndex(oae => new { oae.ManagerId, oae.Timestamp });

        // Foreign key relationships (if needed)
        // Note: We're not setting up explicit foreign keys here to keep the audit trail independent
        // This ensures audit entries remain even if related entities are deleted
    }
}
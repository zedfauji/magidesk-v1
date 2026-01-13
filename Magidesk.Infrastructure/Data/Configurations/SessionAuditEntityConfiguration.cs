using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Infrastructure.Repositories;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for SessionAuditEntity.
/// </summary>
public class SessionAuditEntityConfiguration : IEntityTypeConfiguration<SessionAuditEntity>
{
    public void Configure(EntityTypeBuilder<SessionAuditEntity> builder)
    {
        builder.ToTable("SessionAuditEntries", "magidesk");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .IsRequired();

        builder.Property(s => s.SessionId)
            .IsRequired();

        builder.Property(s => s.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Details)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.Timestamp)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        // Indexes for efficient querying
        builder.HasIndex(s => s.SessionId);
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.Action);
        builder.HasIndex(s => s.Timestamp);

        // Composite index for common queries
        builder.HasIndex(s => new { s.SessionId, s.Timestamp })
            .HasDatabaseName("IX_SessionAuditEntries_SessionId_Timestamp");
    }
}
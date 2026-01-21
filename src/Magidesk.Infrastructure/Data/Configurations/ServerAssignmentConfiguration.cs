using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for ServerAssignment entity.
/// </summary>
public class ServerAssignmentConfiguration : IEntityTypeConfiguration<ServerAssignment>
{
    public void Configure(EntityTypeBuilder<ServerAssignment> builder)
    {
        builder.ToTable("ServerAssignments", "magidesk");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .IsRequired();

        builder.Property(s => s.SessionId)
            .IsRequired();

        builder.Property(s => s.ServerId)
            .IsRequired();

        builder.Property(s => s.AssignedAt)
            .IsRequired();

        builder.Property(s => s.UnassignedAt);

        builder.Property(s => s.IsPrimary)
            .IsRequired();

        builder.Property(s => s.AllocationPercentage)
            .IsRequired()
            .HasPrecision(5, 2); // Allows values like 100.00

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(s => s.SessionId);
        builder.HasIndex(s => s.ServerId);
        builder.HasIndex(s => s.AssignedAt);
        builder.HasIndex(s => s.UnassignedAt);
        builder.HasIndex(s => s.IsPrimary);

        // Composite index for active assignments
        builder.HasIndex(s => new { s.SessionId, s.UnassignedAt })
            .HasDatabaseName("IX_ServerAssignments_SessionId_Active");

        // Composite index for server performance queries
        builder.HasIndex(s => new { s.ServerId, s.AssignedAt, s.UnassignedAt })
            .HasDatabaseName("IX_ServerAssignments_ServerId_DateRange");
    }
}
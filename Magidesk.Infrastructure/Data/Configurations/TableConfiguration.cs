using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for Table entity.
/// </summary>
public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.ToTable("Tables", "magidesk");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .IsRequired();

        builder.Property(t => t.TableNumber)
            .IsRequired();

        builder.Property(t => t.FloorId);

        builder.Property(t => t.Capacity)
            .IsRequired();

        builder.Property(t => t.X)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(t => t.Y)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (TableStatus)Enum.Parse(typeof(TableStatus), v))
            .HasDefaultValue(TableStatus.Available);

        builder.Property(t => t.Width)
            .IsRequired()
            .HasDefaultValue(100);

        builder.Property(t => t.Height)
            .IsRequired()
            .HasDefaultValue(100);

        builder.Property(t => t.Shape)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (TableShapeType)Enum.Parse(typeof(TableShapeType), v))
            .HasDefaultValue(TableShapeType.Rectangle);

        builder.Property(t => t.CurrentTicketId);

        builder.Property(t => t.TableTypeId)
            .IsRequired(false); // Nullable - tables can exist without a type initially

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(t => t.Version)
            .IsRequired()
            .HasDefaultValue(1);

        // Foreign key relationship to TableType
        builder.HasOne(t => t.TableType)
            .WithMany()
            .HasForeignKey(t => t.TableTypeId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deleting table types that are in use

        // Indexes
        builder.HasIndex(t => t.TableNumber)
            .IsUnique();

        builder.HasIndex(t => t.FloorId);

        builder.HasIndex(t => t.Status);

        builder.HasIndex(t => t.CurrentTicketId);

        builder.HasIndex(t => t.TableTypeId); // Index for foreign key lookups

        builder.HasIndex(t => t.IsActive);
    }
}


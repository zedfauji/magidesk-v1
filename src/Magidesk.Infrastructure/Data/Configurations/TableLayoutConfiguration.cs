using Magidesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Magidesk.Infrastructure.Data.Configurations;

public class TableLayoutConfiguration : IEntityTypeConfiguration<TableLayout>
{
    public void Configure(EntityTypeBuilder<TableLayout> builder)
    {
        builder.ToTable("TableLayouts", "magidesk");
        builder.HasKey(tl => tl.Id);

        builder.Property(tl => tl.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(tl => tl.CreatedAt)
            .IsRequired();

        builder.Property(tl => tl.UpdatedAt)
            .IsRequired();

        builder.Property(tl => tl.Version)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(tl => tl.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(tl => tl.Name)
            .IsUnique()
            .HasDatabaseName("IX_TableLayout_Name");

        builder.HasIndex(tl => tl.FloorId);

        // Relationships
        builder.HasOne(tl => tl.Floor)
            .WithMany(f => f.TableLayouts)
            .HasForeignKey(tl => tl.FloorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(tl => tl.Tables)
            .WithOne(t => t.Layout)
            .HasForeignKey(t => t.LayoutId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

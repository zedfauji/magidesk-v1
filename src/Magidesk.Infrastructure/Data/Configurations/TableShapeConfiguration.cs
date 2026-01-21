using Magidesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Magidesk.Infrastructure.Data.Configurations;

public class TableShapeConfiguration : IEntityTypeConfiguration<TableShape>
{
    public void Configure(EntityTypeBuilder<TableShape> builder)
    {
        builder.ToTable("TableShapes", "magidesk");
        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.BackgroundColor)
            .HasMaxLength(7)
            .HasDefaultValue("#ffffff");

        builder.Property(ts => ts.BorderColor)
            .HasMaxLength(7)
            .HasDefaultValue("#cccccc");

        builder.Property(ts => ts.BorderThickness)
            .IsRequired()
            .HasDefaultValue(2);

        builder.Property(ts => ts.CornerRadius)
            .IsRequired()
            .HasDefaultValue(8);

        builder.Property(ts => ts.Width)
            .IsRequired()
            .HasDefaultValue(100);

        builder.Property(ts => ts.Height)
            .IsRequired()
            .HasDefaultValue(100);

        builder.Property(ts => ts.CreatedAt)
            .IsRequired();

        builder.Property(ts => ts.UpdatedAt)
            .IsRequired();

        builder.Property(ts => ts.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(ts => ts.Name)
            .IsUnique()
            .HasDatabaseName("IX_TableShape_Name");
    }
}

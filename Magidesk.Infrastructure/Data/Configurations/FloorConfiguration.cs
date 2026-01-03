using Magidesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Magidesk.Infrastructure.Data.Configurations;

public class FloorConfiguration : IEntityTypeConfiguration<Floor>
{
    public void Configure(EntityTypeBuilder<Floor> builder)
    {
        builder.ToTable("Floors", "magidesk");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Description)
            .HasMaxLength(500);

        builder.Property(f => f.BackgroundColor)
            .HasMaxLength(7)
            .HasDefaultValue("#f8f8f8");

        builder.Property(f => f.Width)
            .IsRequired()
            .HasDefaultValue(2000);

        builder.Property(f => f.Height)
            .IsRequired()
            .HasDefaultValue(2000);

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.Property(f => f.UpdatedAt)
            .IsRequired();

        builder.Property(f => f.Version)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(f => f.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(f => f.Name)
            .IsUnique()
            .HasDatabaseName("IX_Floor_Name");

        // Relationships
        builder.HasMany(f => f.TableLayouts)
            .WithOne(l => l.Floor)
            .HasForeignKey(l => l.FloorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

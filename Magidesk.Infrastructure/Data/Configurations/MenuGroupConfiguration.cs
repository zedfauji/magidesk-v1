using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class MenuGroupConfiguration : IEntityTypeConfiguration<MenuGroup>
{
    public void Configure(EntityTypeBuilder<MenuGroup> builder)
    {
        builder.ToTable("MenuGroups");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CategoryId)
            .IsRequired();

        builder.Property(x => x.SortOrder)
            .HasDefaultValue(0);

        builder.Property(x => x.IsVisible)
            .HasDefaultValue(true);

        builder.Property(x => x.ButtonColor)
            .HasMaxLength(20);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        // Foreign key relationship
        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index on CategoryId for query performance
        builder.HasIndex(x => x.CategoryId);

        // Composite unique constraint on CategoryId + Name
        builder.HasIndex(x => new { x.CategoryId, x.Name })
            .IsUnique();
    }
}

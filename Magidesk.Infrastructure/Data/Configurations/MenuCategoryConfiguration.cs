using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class MenuCategoryConfiguration : IEntityTypeConfiguration<MenuCategory>
{
    public void Configure(EntityTypeBuilder<MenuCategory> builder)
    {
        builder.ToTable("MenuCategories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.SortOrder)
            .HasDefaultValue(0);

        builder.Property(x => x.IsVisible)
            .HasDefaultValue(true);

        builder.Property(x => x.IsBeverage)
            .HasDefaultValue(false);

        builder.Property(x => x.ButtonColor)
            .HasMaxLength(20);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        // Unique constraint on Name
        builder.HasIndex(x => x.Name)
            .IsUnique();
        
        // G.4: Hierarchy support - Self-referencing relationship
        builder.Property(e => e.ParentCategoryId)
            .IsRequired(false);
        
        builder.HasOne(e => e.Parent)
            .WithMany(e => e.Subcategories)
            .HasForeignKey(e => e.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete of hierarchy
        
        builder.HasIndex(e => e.ParentCategoryId)
            .HasDatabaseName("IX_MenuCategories_ParentCategoryId");
        
        builder.Property(e => e.PrinterGroupId)
            .IsRequired(false);
    }
}

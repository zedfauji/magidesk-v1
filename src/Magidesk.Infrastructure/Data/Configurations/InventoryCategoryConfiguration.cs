using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class InventoryCategoryConfiguration : IEntityTypeConfiguration<InventoryCategory>
{
    public void Configure(EntityTypeBuilder<InventoryCategory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.SortOrder)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.ParentCategoryId);

        builder.HasOne<InventoryCategory>()
            .WithMany()
            .HasForeignKey(x => x.ParentCategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("InventoryCategories");
    }
}

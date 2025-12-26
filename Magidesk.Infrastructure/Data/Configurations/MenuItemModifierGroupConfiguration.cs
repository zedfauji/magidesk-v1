using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class MenuItemModifierGroupConfiguration : IEntityTypeConfiguration<MenuItemModifierGroup>
{
    public void Configure(EntityTypeBuilder<MenuItemModifierGroup> builder)
    {
        builder.ToTable("MenuItemModifierGroups");

        builder.HasKey(x => new { x.MenuItemId, x.ModifierGroupId });

        builder.HasOne(x => x.MenuItem)
            .WithMany(x => x.ModifierGroups)
            .HasForeignKey(x => x.MenuItemId);

        builder.HasOne(x => x.ModifierGroup)
            .WithMany() // Assuming ModifierGroup doesn't need unnecessary overhead of linking back to all Items
            .HasForeignKey(x => x.ModifierGroupId);
    }
}

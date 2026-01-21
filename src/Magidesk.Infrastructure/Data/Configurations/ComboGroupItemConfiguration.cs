using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class ComboGroupItemConfiguration : IEntityTypeConfiguration<ComboGroupItem>
{
    public void Configure(EntityTypeBuilder<ComboGroupItem> builder)
    {
        builder.ToTable("ComboGroupItems");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Upcharge, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Upcharge");
            money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        // Link to MenuItem if we had navigation property, but we only have ID here for now.
        builder.HasIndex(x => x.MenuItemId);
    }
}

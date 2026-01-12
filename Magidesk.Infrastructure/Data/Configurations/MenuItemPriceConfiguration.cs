using Magidesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Magidesk.Infrastructure.Data.Configurations;

public class MenuItemPriceConfiguration : IEntityTypeConfiguration<MenuItemPrice>
{
    public void Configure(EntityTypeBuilder<MenuItemPrice> builder)
    {
        builder.ToTable("MenuItemPrices");

        builder.HasKey(x => x.Id);

        // Configure Value Object: Money
        builder.OwnsOne(x => x.Price, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("PriceAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("PriceCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        // Relationships
        builder.HasOne(x => x.MenuItem)
            .WithMany(x => x.MenuItemPrices)
            .HasForeignKey(x => x.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PriceLevel)
            .WithMany()
            .HasForeignKey(x => x.PriceLevelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

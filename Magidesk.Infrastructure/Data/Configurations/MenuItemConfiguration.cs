using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Data.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("MenuItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Barcode)
            .HasMaxLength(50);
            
        // Value Object: Money
        builder.OwnsOne(x => x.Price, p =>
        {
            p.Property(m => m.Amount)
                .HasColumnName("Price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            p.Property(m => m.Currency)
                .HasColumnName("PriceCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("USD")
                .IsRequired();
        });

        builder.Property(x => x.TaxRate)
            .HasColumnType("decimal(5,4)");

        // Dictionary property map to JSON
        builder.Property(e => e.Properties)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) 
                     ?? new System.Collections.Generic.Dictionary<string, string>());
                     
        // Many-to-Many via Join Entity
        builder.HasMany(x => x.ModifierGroups)
            .WithOne(x => x.MenuItem)
            .HasForeignKey(x => x.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Unit)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.StockQuantity)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.ReorderPoint)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class InventoryAdjustmentConfiguration : IEntityTypeConfiguration<InventoryAdjustment>
{
    public void Configure(EntityTypeBuilder<InventoryAdjustment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuantityDelta)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Reason)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasOne(x => x.InventoryItem)
            .WithMany()
            .HasForeignKey(x => x.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

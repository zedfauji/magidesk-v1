using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class KitchenOrderItemConfiguration : IEntityTypeConfiguration<KitchenOrderItem>
{
    public void Configure(EntityTypeBuilder<KitchenOrderItem> builder)
    {
        builder.ToTable("KitchenOrderItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ItemName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.DestinationId)
            .IsRequired(); // Printer Group ID

        // EF Core 8 + Npgsql supports primitive collections as native arrays
        builder.PrimitiveCollection(x => x.Modifiers); 
    }
}

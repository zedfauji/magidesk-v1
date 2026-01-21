using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class KitchenOrderConfiguration : IEntityTypeConfiguration<KitchenOrder>
{
    public void Configure(EntityTypeBuilder<KitchenOrder> builder)
    {
        builder.ToTable("KitchenOrders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ServerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.TableNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.KitchenOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

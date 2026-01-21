using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for OrderType entity.
/// </summary>
public class OrderTypeConfiguration : IEntityTypeConfiguration<OrderType>
{
    public void Configure(EntityTypeBuilder<OrderType> builder)
    {
        builder.ToTable("OrderTypes", "magidesk");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .IsRequired();

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.CloseOnPaid)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(o => o.AllowSeatBasedOrder)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(o => o.AllowToAddTipsLater)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(o => o.IsBarTab)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(o => o.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(o => o.Version)
            .IsRequired()
            .HasDefaultValue(1);

        // Properties dictionary stored as JSON
        builder.Property(o => o.Properties)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>())
            .HasColumnType("jsonb");

        // Indexes
        builder.HasIndex(o => o.Name)
            .IsUnique();

        builder.HasIndex(o => o.IsActive);
    }
}


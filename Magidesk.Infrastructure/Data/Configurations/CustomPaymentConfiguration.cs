using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using System.Text.Json;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for CustomPayment.
/// </summary>
public class CustomPaymentConfiguration : IEntityTypeConfiguration<CustomPayment>
{
    public void Configure(EntityTypeBuilder<CustomPayment> builder)
    {
        // CustomPayment-specific properties
        builder.Property(cp => cp.PaymentName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(cp => cp.ReferenceNumber)
            .HasMaxLength(100);

        // Properties dictionary stored as JSONB in PostgreSQL
        builder.Property(cp => cp.Properties)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>())
            .HasColumnType("jsonb");
    }
}


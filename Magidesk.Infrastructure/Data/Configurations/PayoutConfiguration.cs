using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Payout.
/// </summary>
public class PayoutConfiguration : IEntityTypeConfiguration<Payout>
{
    public void Configure(EntityTypeBuilder<Payout> builder)
    {
        builder.ToTable("Payouts");

        builder.HasKey(po => po.Id);

        // Properties
        builder.Property(po => po.Id)
            .IsRequired();

        builder.Property(po => po.CashSessionId)
            .IsRequired();

        builder.OwnsOne(po => po.Amount, a =>
        {
            a.Property(am => am.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2)
                .IsRequired();
            a.Property(am => am.Currency)
                .HasColumnName("AmountCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(po => po.ProcessedBy)
            .HasConversion(
                v => v.Value,
                v => new UserId(v))
            .HasColumnName("ProcessedBy")
            .IsRequired();

        builder.Property(po => po.Reason)
            .HasMaxLength(500);

        builder.Property(po => po.ProcessedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(po => po.CashSessionId);
    }
}


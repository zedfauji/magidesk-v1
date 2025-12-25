using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for GiftCertificatePayment.
/// </summary>
public class GiftCertificatePaymentConfiguration : IEntityTypeConfiguration<GiftCertificatePayment>
{
    public void Configure(EntityTypeBuilder<GiftCertificatePayment> builder)
    {
        // GiftCertificatePayment-specific properties
        builder.Property(gc => gc.GiftCertificateNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(gc => gc.OriginalAmount, oa =>
        {
            oa.Property(a => a.Amount)
                .HasColumnName("OriginalAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            oa.Property(a => a.Currency)
                .HasColumnName("OriginalAmountCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(gc => gc.RemainingBalance, rb =>
        {
            rb.Property(b => b.Amount)
                .HasColumnName("RemainingBalance")
                .HasPrecision(18, 2)
                .IsRequired();
            rb.Property(b => b.Currency)
                .HasColumnName("RemainingBalanceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });
    }
}


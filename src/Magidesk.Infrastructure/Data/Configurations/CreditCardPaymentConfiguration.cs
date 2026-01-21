using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for CreditCardPayment.
/// </summary>
public class CreditCardPaymentConfiguration : IEntityTypeConfiguration<CreditCardPayment>
{
    public void Configure(EntityTypeBuilder<CreditCardPayment> builder)
    {
        // CreditCardPayment-specific properties
        builder.Property(cc => cc.CardNumber)
            .HasMaxLength(50);

        builder.Property(cc => cc.CardHolderName)
            .HasMaxLength(255);

        builder.Property(cc => cc.AuthorizationCode)
            .HasMaxLength(50);

        builder.Property(cc => cc.ReferenceNumber)
            .HasMaxLength(100);

        builder.Property(cc => cc.CardType)
            .HasMaxLength(50);

        builder.Property(cc => cc.AuthorizationTime);

        builder.Property(cc => cc.CaptureTime);
    }
}


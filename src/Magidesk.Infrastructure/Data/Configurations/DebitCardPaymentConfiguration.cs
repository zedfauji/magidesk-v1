using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for DebitCardPayment.
/// </summary>
public class DebitCardPaymentConfiguration : IEntityTypeConfiguration<DebitCardPayment>
{
    public void Configure(EntityTypeBuilder<DebitCardPayment> builder)
    {
        // DebitCardPayment-specific properties
        builder.Property(dc => dc.CardNumber)
            .HasMaxLength(50);

        builder.Property(dc => dc.CardHolderName)
            .HasMaxLength(255);

        builder.Property(dc => dc.AuthorizationCode)
            .HasMaxLength(50);

        builder.Property(dc => dc.ReferenceNumber)
            .HasMaxLength(100);

        builder.Property(dc => dc.CardType)
            .HasMaxLength(50);

        builder.Property(dc => dc.PinNumber)
            .HasMaxLength(256); // Encrypted PIN

        builder.Property(dc => dc.AuthorizationTime);

        builder.Property(dc => dc.CaptureTime);
    }
}


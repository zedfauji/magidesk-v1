using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Payment.
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        // Configure TPH (Table Per Hierarchy) for payment types
        builder.HasDiscriminator(p => p.PaymentType)
            .HasValue<CashPayment>(PaymentType.Cash)
            .HasValue<CreditCardPayment>(PaymentType.CreditCard)
            .HasValue<DebitCardPayment>(PaymentType.DebitCard)
            .HasValue<GiftCertificatePayment>(PaymentType.GiftCertificate)
            .HasValue<CustomPayment>(PaymentType.CustomPayment);

        // Properties
        builder.Property(p => p.Id)
            .IsRequired();

        builder.Property(p => p.GlobalId)
            .HasMaxLength(255);

        builder.Property(p => p.TicketId)
            .IsRequired();

        builder.Property(p => p.TransactionType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.PaymentType)
            .HasConversion<int>()
            .IsRequired();

        // Money value objects
        builder.OwnsOne(p => p.Amount, a =>
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

        builder.OwnsOne(p => p.TipsAmount, ta =>
        {
            ta.Property(t => t.Amount)
                .HasColumnName("TipsAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            ta.Property(t => t.Currency)
                .HasColumnName("TipsCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(p => p.TipsExceedAmount, tea =>
        {
            tea.Property(t => t.Amount)
                .HasColumnName("TipsExceedAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            tea.Property(t => t.Currency)
                .HasColumnName("TipsExceedCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(p => p.TenderAmount, ta =>
        {
            ta.Property(t => t.Amount)
                .HasColumnName("TenderAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            ta.Property(t => t.Currency)
                .HasColumnName("TenderCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(p => p.ChangeAmount, ca =>
        {
            ca.Property(c => c.Amount)
                .HasColumnName("ChangeAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            ca.Property(c => c.Currency)
                .HasColumnName("ChangeCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(p => p.TransactionTime)
            .IsRequired();

        builder.OwnsOne(p => p.ProcessedBy, pb =>
        {
            pb.Property(p => p.Value)
                .HasColumnName("ProcessedBy")
                .IsRequired();
        });

        builder.Property(p => p.TerminalId)
            .IsRequired();

        builder.Property(p => p.IsCaptured)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.IsVoided)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.IsAuthorizable)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.CashSessionId);

        builder.Property(p => p.Note)
            .HasMaxLength(1000);

        // Indexes
        builder.HasIndex(p => p.TicketId);
        builder.HasIndex(p => p.CashSessionId)
            .HasFilter("\"CashSessionId\" IS NOT NULL");
        builder.HasIndex(p => p.GlobalId)
            .IsUnique()
            .HasFilter("\"GlobalId\" IS NOT NULL");
        builder.HasIndex(p => p.TransactionTime);
    }
}


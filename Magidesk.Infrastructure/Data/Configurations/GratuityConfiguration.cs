using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Gratuity.
/// </summary>
public class GratuityConfiguration : IEntityTypeConfiguration<Gratuity>
{
    public void Configure(EntityTypeBuilder<Gratuity> builder)
    {
        builder.ToTable("Gratuities");

        builder.HasKey(g => g.Id);

        // Properties
        builder.Property(g => g.Id)
            .IsRequired();

        builder.Property(g => g.TicketId)
            .IsRequired();

        builder.OwnsOne(g => g.Amount, a =>
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

        builder.Property(g => g.Paid)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(g => g.Refunded)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(g => g.TerminalId)
            .IsRequired();

        builder.OwnsOne(g => g.OwnerId, o =>
        {
            o.Property(ow => ow.Value)
                .HasColumnName("OwnerId")
                .IsRequired();
        });

        builder.Property(g => g.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(g => g.TicketId)
            .IsUnique();
    }
}


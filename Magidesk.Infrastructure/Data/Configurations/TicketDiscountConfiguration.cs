using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for TicketDiscount.
/// </summary>
public class TicketDiscountConfiguration : IEntityTypeConfiguration<TicketDiscount>
{
    public void Configure(EntityTypeBuilder<TicketDiscount> builder)
    {
        builder.ToTable("TicketDiscounts");

        builder.HasKey(td => td.Id);

        // Properties
        builder.Property(td => td.Id)
            .IsRequired();

        builder.Property(td => td.TicketId)
            .IsRequired();

        builder.Property(td => td.DiscountId)
            .IsRequired();

        builder.Property(td => td.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(td => td.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(td => td.Value)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.OwnsOne(td => td.MinimumAmount, ma =>
        {
            ma.Property(m => m.Amount)
                .HasColumnName("MinimumAmount")
                .HasPrecision(18, 2);
            ma.Property(m => m.Currency)
                .HasColumnName("MinimumAmountCurrency")
                .HasMaxLength(3);
        });

        builder.OwnsOne(td => td.Amount, da =>
        {
            da.Property(d => d.Amount)
                .HasColumnName("DiscountAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            da.Property(d => d.Currency)
                .HasColumnName("DiscountAmountCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(td => td.AppliedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(td => td.TicketId);
    }
}


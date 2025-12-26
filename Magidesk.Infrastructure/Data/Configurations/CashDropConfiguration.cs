using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for CashDrop.
/// </summary>
public class CashDropConfiguration : IEntityTypeConfiguration<CashDrop>
{
    public void Configure(EntityTypeBuilder<CashDrop> builder)
    {
        builder.ToTable("CashDrops");

        builder.HasKey(cd => cd.Id);

        // Properties
        builder.Property(cd => cd.Id)
            .IsRequired();

        builder.Property(cd => cd.CashSessionId)
            .IsRequired();

        builder.OwnsOne(cd => cd.Amount, a =>
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

        builder.Property(cd => cd.ProcessedBy)
            .HasConversion(
                v => v.Value,
                v => new UserId(v))
            .HasColumnName("ProcessedBy")
            .IsRequired();

        builder.Property(cd => cd.Reason)
            .HasMaxLength(500);

        builder.Property(cd => cd.ProcessedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(cd => cd.CashSessionId);
    }
}


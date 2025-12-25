using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for DrawerBleed.
/// </summary>
public class DrawerBleedConfiguration : IEntityTypeConfiguration<DrawerBleed>
{
    public void Configure(EntityTypeBuilder<DrawerBleed> builder)
    {
        builder.ToTable("DrawerBleeds");

        builder.HasKey(db => db.Id);

        // Properties
        builder.Property(db => db.Id)
            .IsRequired();

        builder.Property(db => db.CashSessionId)
            .IsRequired();

        builder.OwnsOne(db => db.Amount, a =>
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

        builder.OwnsOne(db => db.ProcessedBy, pb =>
        {
            pb.Property(p => p.Value)
                .HasColumnName("ProcessedBy")
                .IsRequired();
        });

        builder.Property(db => db.Reason)
            .HasMaxLength(500);

        builder.Property(db => db.ProcessedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(db => db.CashSessionId);
    }
}


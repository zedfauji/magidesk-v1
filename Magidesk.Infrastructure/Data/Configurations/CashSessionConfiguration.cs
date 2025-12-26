using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for CashSession aggregate root.
/// </summary>
public class CashSessionConfiguration : IEntityTypeConfiguration<CashSession>
{
    public void Configure(EntityTypeBuilder<CashSession> builder)
    {
        builder.ToTable("CashSessions");

        builder.HasKey(cs => cs.Id);

        // Properties
        builder.Property(cs => cs.Id)
            .IsRequired();

        builder.Property(cs => cs.UserId)
            .HasConversion(
                v => v.Value,
                v => new UserId(v))
            .HasColumnName("UserId")
            .IsRequired();

        builder.Property(cs => cs.TerminalId)
            .IsRequired();

        builder.Property(cs => cs.ShiftId)
            .IsRequired();

        builder.Property(cs => cs.OpenedAt)
            .IsRequired();

        builder.Property(cs => cs.ClosedAt);

        builder.Property(cs => cs.ClosedBy)
            .HasConversion(
                v => v != null ? v.Value : (Guid?)null,
                v => v.HasValue ? new UserId(v.Value) : null)
            .HasColumnName("ClosedBy");

        // Money value objects
        builder.OwnsOne(cs => cs.OpeningBalance, ob =>
        {
            ob.Property(o => o.Amount)
                .HasColumnName("OpeningBalance")
                .HasPrecision(18, 2)
                .IsRequired();
            ob.Property(o => o.Currency)
                .HasColumnName("OpeningBalanceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(cs => cs.ExpectedCash, ec =>
        {
            ec.Property(e => e.Amount)
                .HasColumnName("ExpectedCash")
                .HasPrecision(18, 2)
                .IsRequired();
            ec.Property(e => e.Currency)
                .HasColumnName("ExpectedCashCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(cs => cs.ActualCash, ac =>
        {
            ac.Property(a => a.Amount)
                .HasColumnName("ActualCash")
                .HasPrecision(18, 2);
            ac.Property(a => a.Currency)
                .HasColumnName("ActualCashCurrency")
                .HasMaxLength(3);
        });

        builder.OwnsOne(cs => cs.Difference, d =>
        {
            d.Property(di => di.Amount)
                .HasColumnName("Difference")
                .HasPrecision(18, 2);
            d.Property(di => di.Currency)
                .HasColumnName("DifferenceCurrency")
                .HasMaxLength(3);
        });

        builder.Property(cs => cs.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(cs => cs.Version)
            .IsRequired()
            .HasDefaultValue(1);

        // Relationships
        builder.HasMany(cs => cs.Payments)
            .WithOne()
            .HasForeignKey(p => p.CashSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(cs => cs.Payouts)
            .WithOne()
            .HasForeignKey(po => po.CashSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cs => cs.CashDrops)
            .WithOne()
            .HasForeignKey(cd => cd.CashSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cs => cs.DrawerBleeds)
            .WithOne()
            .HasForeignKey(db => db.CashSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cs => cs.TerminalTransactions)
            .WithOne()
            .HasForeignKey(tt => tt.CashSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(cs => cs.ShiftId);
        builder.HasIndex(cs => cs.Status)
            .HasFilter("\"Status\" = 0"); // Open sessions
    }
}


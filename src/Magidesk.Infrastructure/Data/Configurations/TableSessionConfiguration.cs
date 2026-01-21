using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for TableSession entity.
/// </summary>
public class TableSessionConfiguration : IEntityTypeConfiguration<TableSession>
{
    public void Configure(EntityTypeBuilder<TableSession> builder)
    {
        builder.ToTable("TableSessions", "magidesk");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .IsRequired();

        builder.Property(s => s.TableId)
            .IsRequired();

        builder.Property(s => s.TableTypeId)
            .IsRequired();

        builder.Property(s => s.CustomerId);

        builder.Property(s => s.TicketId);

        builder.Property(s => s.StartTime)
            .IsRequired();

        builder.Property(s => s.EndTime);

        builder.Property(s => s.PausedAt);

        builder.Property(s => s.TotalPausedDuration)
            .IsRequired();

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (TableSessionStatus)Enum.Parse(typeof(TableSessionStatus), v));

        builder.Property(s => s.HourlyRate)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(s => s.GuestCount)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        // Configure Money value object
        builder.OwnsOne(s => s.TotalCharge, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalChargeAmount")
                .HasPrecision(10, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("TotalChargeCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Indexes
        builder.HasIndex(s => s.TableId);
        builder.HasIndex(s => s.CustomerId);
        builder.HasIndex(s => s.TicketId);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.StartTime);
    }
}

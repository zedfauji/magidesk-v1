using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Ticket aggregate root.
/// </summary>
public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");

        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Id)
            .IsRequired();

        builder.Property(t => t.TicketNumber)
            .IsRequired();

        builder.Property(t => t.GlobalId)
            .HasMaxLength(255);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.OpenedAt);

        builder.Property(t => t.ClosedAt);

        builder.Property(t => t.ActiveDate)
            .IsRequired();

        builder.Property(t => t.DeliveryDate);

        builder.Property(t => t.Status)
            .HasConversion<int>()
            .IsRequired();

        // Value Objects - stored as owned types
        builder.OwnsOne(t => t.CreatedBy, cb =>
        {
            cb.Property(c => c.Value)
                .HasColumnName("CreatedBy")
                .IsRequired();
        });

        builder.OwnsOne(t => t.ClosedBy, cb =>
        {
            cb.Property(c => c.Value)
                .HasColumnName("ClosedBy");
        });

        builder.OwnsOne(t => t.VoidedBy, vb =>
        {
            vb.Property(v => v.Value)
                .HasColumnName("VoidedBy");
        });

        // Money value objects - stored as decimal with currency
        builder.OwnsOne(t => t.SubtotalAmount, sa =>
        {
            sa.Property(s => s.Amount)
                .HasColumnName("SubtotalAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            sa.Property(s => s.Currency)
                .HasColumnName("SubtotalCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(t => t.DiscountAmount, da =>
        {
            da.Property(d => d.Amount)
                .HasColumnName("DiscountAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            da.Property(d => d.Currency)
                .HasColumnName("DiscountCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(t => t.TaxAmount, ta =>
        {
            ta.Property(t => t.Amount)
                .HasColumnName("TaxAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            ta.Property(t => t.Currency)
                .HasColumnName("TaxCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(t => t.ServiceChargeAmount, sca =>
        {
            sca.Property(s => s.Amount)
                .HasColumnName("ServiceChargeAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            sca.Property(s => s.Currency)
                .HasColumnName("ServiceChargeCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(t => t.DeliveryChargeAmount, dca =>
        {
            dca.Property(d => d.Amount)
                .HasColumnName("DeliveryChargeAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            dca.Property(d => d.Currency)
                .HasColumnName("DeliveryChargeCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(t => t.AdjustmentAmount, aa =>
        {
            aa.Property(a => a.Amount)
                .HasColumnName("AdjustmentAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            aa.Property(a => a.Currency)
                .HasColumnName("AdjustmentCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(t => t.TotalAmount, ta =>
        {
            ta.Property(t => t.Amount)
                .HasColumnName("TotalAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            ta.Property(t => t.Currency)
                .HasColumnName("TotalCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(t => t.PaidAmount, pa =>
        {
            pa.Property(p => p.Amount)
                .HasColumnName("PaidAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            pa.Property(p => p.Currency)
                .HasColumnName("PaidCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(t => t.DueAmount, da =>
        {
            da.Property(d => d.Amount)
                .HasColumnName("DueAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            da.Property(d => d.Currency)
                .HasColumnName("DueCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(t => t.AdvanceAmount, aa =>
        {
            aa.Property(a => a.Amount)
                .HasColumnName("AdvanceAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            aa.Property(a => a.Currency)
                .HasColumnName("AdvanceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Regular properties
        builder.Property(t => t.TerminalId)
            .IsRequired();

        builder.Property(t => t.ShiftId)
            .IsRequired();

        builder.Property(t => t.OrderTypeId)
            .IsRequired();

        builder.Property(t => t.CustomerId);

        builder.Property(t => t.AssignedDriverId);

        builder.Property(t => t.TableNumbers)
            .HasConversion(
                v => v == null || v.Count == 0 ? string.Empty : string.Join(",", v),
                v => string.IsNullOrEmpty(v) ? new List<int>() : v.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList(),
                new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<IReadOnlyList<int>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())) : 0,
                    c => c != null ? c.ToList() : new List<int>()))
            .HasMaxLength(500);

        builder.Property(t => t.NumberOfGuests)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(t => t.IsTaxExempt)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.PriceIncludesTax)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.IsBarTab)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.IsReOpened)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.DeliveryAddress)
            .HasMaxLength(500);

        builder.Property(t => t.ExtraDeliveryInfo)
            .HasMaxLength(1000);

        builder.Property(t => t.CustomerWillPickup)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.Version)
            .IsRequired()
            .IsRowVersion()
            .HasDefaultValue(1);

        builder.Property(t => t.Properties)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>(),
                new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<IReadOnlyDictionary<string, string>>(
                    (c1, c2) => c1 != null && c2 != null && c1.Count == c2.Count && !c1.Except(c2).Any(),
                    c => c != null ? c.GetHashCode() : 0,
                    c => c != null ? new Dictionary<string, string>(c) : new Dictionary<string, string>()))
            .HasColumnType("jsonb");

        // Relationships
        builder.HasMany(t => t.OrderLines)
            .WithOne()
            .HasForeignKey(ol => ol.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Payments)
            .WithOne()
            .HasForeignKey(p => p.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Discounts)
            .WithOne()
            .HasForeignKey(td => td.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Gratuity)
            .WithOne()
            .HasForeignKey<Gratuity>(g => g.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => t.TicketNumber)
            .IsUnique();

        builder.HasIndex(t => t.GlobalId)
            .IsUnique()
            .HasFilter("\"GlobalId\" IS NOT NULL");

        builder.HasIndex(t => t.ShiftId);

        builder.HasIndex(t => t.Status);

        builder.HasIndex(t => t.CreatedAt);

        builder.HasIndex(t => t.ActiveDate);
    }
}


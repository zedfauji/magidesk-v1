using Microsoft.EntityFrameworkCore;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data.Configurations;

namespace Magidesk.Infrastructure.Data;

/// <summary>
/// Main database context for the application.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<Ticket> Tickets { get; set; } = null!;
    public DbSet<OrderLine> OrderLines { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<CashSession> CashSessions { get; set; } = null!;
    public DbSet<AuditEvent> AuditEvents { get; set; } = null!;
    public DbSet<Gratuity> Gratuities { get; set; } = null!;
    public DbSet<TicketDiscount> TicketDiscounts { get; set; } = null!;
    public DbSet<OrderLineDiscount> OrderLineDiscounts { get; set; } = null!;
    public DbSet<OrderLineModifier> OrderLineModifiers { get; set; } = null!;
    public DbSet<Payout> Payouts { get; set; } = null!;
    public DbSet<CashDrop> CashDrops { get; set; } = null!;
    public DbSet<DrawerBleed> DrawerBleeds { get; set; } = null!;
    public DbSet<Discount> Discounts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new TicketConfiguration());
        modelBuilder.ApplyConfiguration(new OrderLineConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        modelBuilder.ApplyConfiguration(new CashPaymentConfiguration());
        modelBuilder.ApplyConfiguration(new CreditCardPaymentConfiguration());
        modelBuilder.ApplyConfiguration(new DebitCardPaymentConfiguration());
        modelBuilder.ApplyConfiguration(new GiftCertificatePaymentConfiguration());
        modelBuilder.ApplyConfiguration(new CustomPaymentConfiguration());
        modelBuilder.ApplyConfiguration(new CashSessionConfiguration());
        modelBuilder.ApplyConfiguration(new AuditEventConfiguration());
        modelBuilder.ApplyConfiguration(new GratuityConfiguration());
        modelBuilder.ApplyConfiguration(new TicketDiscountConfiguration());
        modelBuilder.ApplyConfiguration(new OrderLineDiscountConfiguration());
        modelBuilder.ApplyConfiguration(new OrderLineModifierConfiguration());
        modelBuilder.ApplyConfiguration(new PayoutConfiguration());
        modelBuilder.ApplyConfiguration(new CashDropConfiguration());
        modelBuilder.ApplyConfiguration(new DrawerBleedConfiguration());
        modelBuilder.ApplyConfiguration(new DiscountConfiguration());
    }
}


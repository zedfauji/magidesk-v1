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
    public DbSet<Shift> Shifts { get; set; } = null!;
    public DbSet<OrderType> OrderTypes { get; set; } = null!;
    public DbSet<Table> Tables { get; set; } = null!;
    public DbSet<MenuModifier> MenuModifiers { get; set; } = null!;
    public DbSet<ModifierGroup> ModifierGroups { get; set; } = null!;
    public DbSet<KitchenOrder> KitchenOrders { get; set; } = null!;
    public DbSet<KitchenOrderItem> KitchenOrderItems { get; set; } = null!;
    public DbSet<TerminalTransaction> TerminalTransactions { get; set; } = null!;
    public DbSet<PaymentBatch> PaymentBatches { get; set; } = null!;
    public DbSet<GroupSettlement> GroupSettlements { get; set; } = null!;
    public DbSet<ComboDefinition> ComboDefinitions { get; set; } = null!;
    public DbSet<ComboGroup> ComboGroups { get; set; } = null!;
    public DbSet<ComboGroupItem> ComboGroupItems { get; set; } = null!;
    public DbSet<MenuItem> MenuItems { get; set; } = null!;
    public DbSet<MenuItemModifierGroup> MenuItemModifierGroups { get; set; } = null!;
    public DbSet<MenuCategory> MenuCategories { get; set; } = null!;
    public DbSet<MenuGroup> MenuGroups { get; set; } = null!;
    public DbSet<MerchantGatewayConfiguration> MerchantGatewayConfigurations { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<InventoryItem> InventoryItems { get; set; } = null!;
    // FractionalModifier is part of Set<MenuModifier> via Inheritance

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
        modelBuilder.ApplyConfiguration(new ShiftConfiguration());
        modelBuilder.ApplyConfiguration(new OrderTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TableConfiguration());
        modelBuilder.ApplyConfiguration(new MenuModifierConfiguration());
        modelBuilder.ApplyConfiguration(new ModifierGroupConfiguration());
        modelBuilder.ApplyConfiguration(new KitchenOrderConfiguration());
        modelBuilder.ApplyConfiguration(new KitchenOrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new TerminalTransactionConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentBatchConfiguration());
        modelBuilder.ApplyConfiguration(new GroupSettlementConfiguration());
        modelBuilder.ApplyConfiguration(new ComboDefinitionConfiguration());
        modelBuilder.ApplyConfiguration(new ComboGroupConfiguration());
        modelBuilder.ApplyConfiguration(new ComboGroupItemConfiguration());
        modelBuilder.ApplyConfiguration(new FractionalModifierConfiguration());
        modelBuilder.ApplyConfiguration(new MerchantGatewayConfigurationConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new MenuItemConfiguration());
        modelBuilder.ApplyConfiguration(new MenuItemModifierGroupConfiguration());
        modelBuilder.ApplyConfiguration(new MenuCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new MenuGroupConfiguration());
        modelBuilder.ApplyConfiguration(new InventoryItemConfiguration());
    }
}


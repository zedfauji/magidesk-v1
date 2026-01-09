using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Services.Reports;
using Magidesk.Infrastructure.Data;
using Magidesk.Infrastructure.PaymentGateways;
using Magidesk.Infrastructure.Printing;
using Magidesk.Infrastructure.Repositories;
using Magidesk.Domain.DomainServices;
using Magidesk.Domain.Services;
using Magidesk.Application.Services;
using Magidesk.Infrastructure.Security;
using Magidesk.Infrastructure.Services.Bootstrap;
using Magidesk.Infrastructure.Printing.Engines;
using Magidesk.Infrastructure.Printing.Layouts;
using Magidesk.Domain.Interfaces.Printing;
using Magidesk.Domain.Enumerations;
using Magidesk.Infrastructure.Services; // For LiquidTemplateEngine
using Magidesk.Application.Services.Printing;
using Magidesk.Infrastructure.Printing.Drivers; // For EscPosDriver, PlainTextDriver

namespace Magidesk.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for configuring dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Infrastructure layer services to the service collection.
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register Database Setup Services FIRST (needed by DbContext)
        services.AddSingleton<IDatabaseConfigurationService, DatabaseConfigurationService>();
        services.AddScoped<IDatabaseSeedingService, DatabaseSeedingService>();

        // Register DbContext Factory (prevents early database connection during DI initialization)
        services.AddSingleton<ApplicationDbContextFactory>();

        // Register DbContext as Scoped with factory pattern
        // This delays database connection until the DbContext is actually requested
        services.AddScoped<ApplicationDbContext>(serviceProvider =>
        {
            var factory = serviceProvider.GetRequiredService<ApplicationDbContextFactory>();
            return factory.CreateDbContext();
        });

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IShiftRepository, ShiftRepository>();
        services.AddScoped<ICashSessionRepository, CashSessionRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<IOrderTypeRepository, OrderTypeRepository>();
        services.AddScoped<ITableRepository, TableRepository>();
        services.AddScoped<IAuditEventRepository, AuditEventRepository>();
        services.AddScoped<ITableLayoutRepository, TableLayoutRepository>();
        services.AddScoped<IKitchenOrderRepository, KitchenOrderRepository>();
        services.AddScoped<IGroupSettlementRepository, GroupSettlementRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPaymentBatchRepository, PaymentBatchRepository>();
        services.AddScoped<IMerchantGatewayConfigurationRepository, MerchantGatewayConfigurationRepository>();
        services.AddScoped<IDiscountRepository, DiscountRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IMenuCategoryRepository, MenuCategoryRepository>();
        services.AddScoped<IMenuGroupRepository, MenuGroupRepository>();
        services.AddScoped<ISalesReportRepository, SalesReportRepository>();
        services.AddScoped<IModifierGroupRepository, ModifierGroupRepository>();
        services.AddScoped<IMenuModifierRepository, MenuModifierRepository>();
        services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
        services.AddScoped<IRestaurantConfigurationRepository, RestaurantConfigurationRepository>();
        services.AddScoped<ITerminalRepository, TerminalRepository>();
        services.AddScoped<IPrinterGroupRepository, PrinterGroupRepository>();
        services.AddScoped<IPrinterMappingRepository, PrinterMappingRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<IInventoryAdjustmentRepository, InventoryAdjustmentRepository>();
        services.AddScoped<IServerSectionRepository, ServerSectionRepository>();
        services.AddScoped<IPrintTemplateRepository, PrintTemplateRepository>();
        services.AddScoped<ITableTypeRepository, TableTypeRepository>();
        services.AddScoped<ITableSessionRepository, TableSessionRepository>();

        // Application Services
        // Legacy PricingService for backward compatibility (EndTableSessionCommandHandler)
        services.AddScoped<Magidesk.Application.Interfaces.IPricingService, SimplePricingService>();
        
        // New PricingService for time-based billing with table types (BE-A.9-01)
        services.AddScoped<Magidesk.Domain.Services.IPricingService, Magidesk.Domain.Services.PricingService>();

        // Register domain services (stateless, can be singleton or scoped)
        services.AddScoped<TaxDomainService>();
        services.AddScoped<TicketDomainService>();
        services.AddScoped<PaymentDomainService>();
        services.AddScoped<CashSessionDomainService>();
        services.AddScoped<DiscountDomainService>();
        services.AddScoped<ServiceChargeDomainService>();
        services.AddScoped<PriceCalculator>();

        // Register payment gateway (using mock for development)
        services.AddScoped<IPaymentGateway, MockPaymentGateway>();

        // Register print services
        services.AddScoped<IRawPrintService, WindowsPrintingService>();
        services.AddScoped<IKitchenPrintService, KitchenPrintService>();
        services.AddScoped<IReceiptPrintService, ReceiptPrintService>();
        services.AddScoped<ITemplateEngine, LiquidTemplateEngine>();
        services.AddScoped<IPrintContextBuilder, Magidesk.Application.Services.Printing.PrintContextBuilder>();

        // Register Security Services
        services.AddSingleton<IAesEncryptionService, Security.AesEncryptionService>();
        services.AddScoped<ISecurityService, SecurityService>();
        services.AddScoped<IGroupSettleService, GroupSettleService>();
        services.AddScoped<IMerchantBatchService, MerchantBatchService>();
        services.AddScoped<ISystemInitializationService, SystemInitializationService>();
        services.AddScoped<ISystemService, SystemService>();
        services.AddScoped<IBackupService, Services.PostgresBackupService>();

        // Printing Layout Adapters
        services.AddTransient<Thermal58mmAdapter>();
        services.AddTransient<Thermal80mmAdapter>();
        services.AddTransient<StandardPageAdapter>();

        // Factory for Layout Adapters
        services.AddScoped<Func<PrinterFormat, IPrintLayoutAdapter>>(serviceProvider => format =>
        {
            return format switch
            {
                PrinterFormat.Thermal58mm => serviceProvider.GetRequiredService<Thermal58mmAdapter>(),
                PrinterFormat.Thermal80mm => serviceProvider.GetRequiredService<Thermal80mmAdapter>(),
                PrinterFormat.StandardPage => serviceProvider.GetRequiredService<StandardPageAdapter>(),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        });

        // Layout Engine
        services.AddScoped<IPrintLayoutEngine, PrintLayoutEngine>();

        // New ADM Drivers
        services.AddTransient<EscPosDriver>();
        services.AddTransient<PlainTextDriver>();
        
        services.AddScoped<Func<PrinterFormat, IPrintDriver>>(sp => format =>
        {
            return format switch
            {
                PrinterFormat.StandardPage => sp.GetRequiredService<PlainTextDriver>(),
                _ => sp.GetRequiredService<EscPosDriver>() // Default to Thermal
            };
        });

        return services;
    }
}

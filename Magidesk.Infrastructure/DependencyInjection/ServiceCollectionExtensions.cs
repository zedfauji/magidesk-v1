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

namespace Magidesk.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for configuring dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Infrastructure layer services to the service collection.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                DatabaseConnection.GetConnectionString(),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("Magidesk.Infrastructure")));

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IShiftRepository, ShiftRepository>();
        services.AddScoped<ICashSessionRepository, CashSessionRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<IOrderTypeRepository, OrderTypeRepository>();
        services.AddScoped<ITableRepository, TableRepository>();
        services.AddScoped<IAuditEventRepository, AuditEventRepository>();
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

        // Register print services (using mock for development)
        services.AddScoped<IKitchenPrintService, MockKitchenPrintService>();
        services.AddScoped<IReceiptPrintService, MockReceiptPrintService>();

        // Register Security Services
        services.AddSingleton<IAesEncryptionService, Security.AesEncryptionService>();
        services.AddScoped<ISecurityService, SecurityService>();
        services.AddScoped<IGroupSettleService, GroupSettleService>();
        services.AddScoped<IMerchantBatchService, MerchantBatchService>();
        services.AddScoped<ISystemInitializationService, SystemInitializationService>();
        services.AddScoped<ISystemService, SystemService>();
        services.AddScoped<IBackupService, Services.PostgresBackupService>();

        return services;
    }
}

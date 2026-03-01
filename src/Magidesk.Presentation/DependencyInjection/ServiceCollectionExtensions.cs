using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Commands;
using Magidesk.Application.Services;
using Magidesk.Application.DTOs;
using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Queries;
using Magidesk.Application.Queries.Reports;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.ViewModels.Dialogs;

namespace Magidesk.Presentation.DependencyInjection;

/// <summary>
/// Extension methods for configuring Presentation layer dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Presentation layer services to the service collection.
    /// Registers ViewModels, Navigation, Dialog services, and UI helpers.
    /// </summary>
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        // ===================================================================
        // NAVIGATION SERVICES (Singleton - single instance for app lifetime)
        // ===================================================================
        services.AddSingleton<NavigationService>();
        services.AddSingleton<IDefaultViewRoutingService, DefaultViewRoutingService>();
        services.AddSingleton<OrderPageNavigationHelper>();

        // ===================================================================
        // DIALOG SERVICES (Transient - new instance per dialog)
        // ===================================================================
        services.AddTransient<IOrderEntryDialogService, OrderEntryDialogService>();
        services.AddTransient<ISwitchboardDialogService, SwitchboardDialogService>();
        services.AddTransient<IDialogService, WindowsDialogService>();
        services.AddTransient<IEnhancedDialogService, EnhancedDialogService>();

        // ===================================================================
        // UI SERVICES (Singleton - shared state across app)
        // ===================================================================
        services.AddSingleton<UserService>();
        services.AddSingleton<IUserService>(sp => sp.GetRequiredService<UserService>());
        services.AddSingleton<IUserContextService>(sp => sp.GetRequiredService<UserService>());
        services.AddSingleton<Magidesk.Application.Interfaces.ITerminalContext, TerminalContext>();
        services.AddSingleton<IFeatureFlagService, FeatureFlagService>();
        services.AddSingleton<LocalizationService>();
        services.AddSingleton<IKdsSettingsService, KdsSettingsService>();
        
        // Client-Side NoOp Publisher to satisfy dependency
        services.AddSingleton<IKitchenNotificationPublisher, NoOpKitchenNotificationPublisher>();

        // ===================================================================
        // PAGE VIEWMODELS (Transient - new instance per navigation)
        // ===================================================================
        services.AddTransient<BackOfficeViewModel>();
        services.AddTransient<MenuEditorViewModel>();
        services.AddTransient<ModifierEditorViewModel>();
        services.AddTransient<InventoryViewModel>();
        services.AddTransient<CashSessionViewModel>();
        services.AddTransient<TicketViewModel>();
        services.AddTransient<PaymentViewModel>();
        services.AddTransient<DiscountTaxViewModel>();
        services.AddTransient<PrintViewModel>();
        services.AddTransient<TicketManagementViewModel>();
        services.AddTransient<DrawerPullReportViewModel>();
        services.AddTransient<SalesReportsViewModel>();
        services.AddTransient<UserManagementViewModel>();
        services.AddTransient<RoleManagementViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<OrderTypeExplorerViewModel>();
        services.AddTransient<ShiftExplorerViewModel>();
        services.AddTransient<SwitchboardViewModel>();
        services.AddTransient<OrderTypeSelectionViewModel>();
        services.AddTransient<Magidesk.Presentation.ViewModels.ModifierSelectionViewModel>();
        services.AddTransient<SplitTicketViewModel>();
        services.AddTransient<TableMapViewModel>();
        services.AddTransient<TableExplorerViewModel>();
        services.AddTransient<DatabaseSetupViewModel>();
        services.AddTransient<TableDesignerViewModel>();
        services.AddTransient<VendorManagementViewModel>();
        services.AddTransient<PurchaseOrderViewModel>();
        services.AddTransient<SettlePageViewModel>();
        services.AddTransient<OrderPageViewModel>();
        services.AddTransient<SystemConfigViewModel>();
        services.AddTransient<KitchenDisplayViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<ShiftStartViewModel>();
        services.AddTransient<CashDropManagementViewModel>();
        services.AddTransient<VoidTicketViewModel>();
        services.AddTransient<RefundTicketViewModel>();
        services.AddTransient<OpenTicketsListViewModel>();
        services.AddTransient<PaymentProcessWaitViewModel>();
        services.AddTransient<SwipeCardViewModel>();
        services.AddTransient<AuthorizationCodeViewModel>();
        services.AddTransient<AuthorizationCaptureBatchViewModel>();
        services.AddTransient<GuestCountViewModel>();
        services.AddTransient<ManagerFunctionsViewModel>();
        services.AddTransient<GroupSettleTicketSelectionViewModel>();
        services.AddTransient<GroupSettleTicketViewModel>();
        services.AddTransient<HeldTicketsViewModel>();
        services.AddTransient<FloorManagementViewModel>();
        services.AddTransient<ExportImportManagementViewModel>();
        services.AddTransient<ServerSectionManagementViewModel>();
        services.AddTransient<DiscountManagementViewModel>();
        services.AddTransient<PrintTemplatesViewModel>();
        services.AddTransient<TemplateEditorViewModel>();
        services.AddTransient<CustomerListViewModel>();
        services.AddTransient<MemberProfileViewModel>();
        services.AddTransient<MemberCheckInViewModel>();
        services.AddTransient<CustomerSearchViewModel>();
        services.AddTransient<CategoryTreeViewModel>();
        services.AddTransient<ErrorManagementViewModel>();
        services.AddTransient<LanguageSelectionViewModel>();

        // ===================================================================
        // DIALOG VIEWMODELS (Transient - new instance per dialog)
        // ===================================================================
        services.AddTransient<ConfirmationDialogViewModel>();
        services.AddTransient<TableSelectionViewModel>();
        services.AddTransient<NotesDialogViewModel>();
        services.AddTransient<ManagerPinDialogViewModel>();
        services.AddTransient<CashEntryDialogViewModel>();
        services.AddTransient<StartSessionDialogViewModel>();
        services.AddTransient<EndSessionDialogViewModel>();
        services.AddTransient<HoldTicketDialogViewModel>();
        services.AddTransient<SplitPaymentViewModel>();
        services.AddTransient<DiscountSelectionViewModel>();
        services.AddTransient<SessionControlDialogViewModel>();
        services.AddTransient<ManagerOverrideDialogViewModel>();
        services.AddTransient<TableOperationsDialogViewModel>();
        services.AddTransient<PromotionScheduleDialogViewModel>();

        // ===================================================================
        // DIALOG VIEWS (Transient - new instance per dialog)
        // ===================================================================
        services.AddTransient<Views.Dialogs.ConfirmationDialog>();
        services.AddTransient<Views.Dialogs.ManagerPinDialog>();
        services.AddTransient<Views.Dialogs.ManagerOverrideDialog>();
        services.AddTransient<Views.Dialogs.MemberCheckInDialog>();
        services.AddTransient<Views.LanguageSelectionDialog>();

        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Magidesk.Application.DependencyInjection;
using Magidesk.Infrastructure.DependencyInjection;
using Magidesk.Presentation.Services;

namespace Magidesk.Presentation;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Microsoft.UI.Xaml.Application
{
    private Window? _window;

    public static Window? MainWindowInstance { get; private set; }

    public static IHost Host { get; private set; } = null!;

    public static IServiceProvider Services => Host.Services;

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // Application + Infrastructure composition root
                services.AddApplication();
                services.AddInfrastructure();

                // UI services
                services.AddTransient<Magidesk.Presentation.ViewModels.OrderEntryViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.BackOfficeViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.MenuEditorViewModel>();

                services.AddTransient<Magidesk.Presentation.ViewModels.ModifierEditorViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.InventoryViewModel>();

                // Build the service providers
                services.AddSingleton<NavigationService>();

                // ViewModels
                services.AddTransient<Magidesk.Presentation.ViewModels.CashSessionViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.TicketViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.PaymentViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.DiscountTaxViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.PrintViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.TicketManagementViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.DrawerPullReportViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.SalesReportsViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.UserManagementViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.SettingsViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.SwitchboardViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.OrderTypeSelectionViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.ModifierSelectionViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.SplitTicketViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.TableMapViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.SettleViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.SystemConfigViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.KitchenDisplayViewModel>();
                services.AddTransient<Magidesk.Presentation.ViewModels.LoginViewModel>();

            })
            .Build();
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        var mainWindow = (MainWindow)_window; // Define mainWindow first
        MainWindowInstance = mainWindow; // Assign MainWindowInstance after mainWindow is defined
        _window.Activate();

        mainWindow.ShowLoading("Initializing System...");

        try
        {
            var initService = Host.Services.GetRequiredService<Magidesk.Application.Interfaces.ISystemInitializationService>();
            var result = await initService.InitializeSystemAsync();

            if (result.IsSuccess)
            {
                // F-0002: Set Terminal ID in Status Bar
                if (!string.IsNullOrEmpty(result.TerminalId))
                {
                    mainWindow.SetTerminalId(result.TerminalId);
                }

                mainWindow.HideLoading();
                var navService = Host.Services.GetRequiredService<NavigationService>();
                // F-0003: Navigate to Login Page first
                navService.Navigate(typeof(Views.LoginPage));
            }
            else
            {
                // In a real scenario, we might navigate to a Settings page to fix DB config.
                // For now, we show the failure in the loading overlay.
                mainWindow.ShowLoading($"Startup Failed: {result.Message}"); 
            }
        }
        catch (System.Exception ex)
        {
             mainWindow.ShowLoading($"Critical Error: {ex.Message}");
        }
    }
}

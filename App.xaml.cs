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
            })
            .Build();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}

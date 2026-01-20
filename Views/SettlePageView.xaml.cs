using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Magidesk.Presentation.Views;

/// <summary>
/// Redesigned Settle Page for payment settlement with modern UI.
/// </summary>
public sealed partial class SettlePageView : Page
{
    public SettlePageViewModel ViewModel { get; }

    public SettlePageView()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<SettlePageViewModel>();
        DataContext = ViewModel;
        
        // Set XamlRoot for dialogs once the page is loaded
        this.Loaded += (s, e) =>
        {
            if (this.XamlRoot != null)
            {
                ViewModel.SetXamlRoot(this.XamlRoot);
                System.Diagnostics.Debug.WriteLine("SettlePageView - XamlRoot set on ViewModel");
            }
        };
        
        System.Diagnostics.Debug.WriteLine("SettlePageView constructor - ViewModel created");
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        // Extract ticket ID from navigation parameter
        if (e.Parameter is Guid ticketId)
        {
            await ViewModel.InitializeAsync(ticketId);
        }
    }

    // Quick Cash Click Handlers
    private void QuickCash1_Click(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("QuickCash1_Click called");
        if (ViewModel?.QuickCashCommand != null)
        {
            System.Diagnostics.Debug.WriteLine("QuickCash1_Click: Executing command with 1m");
            ViewModel.QuickCashCommand.Execute(1m);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("QuickCash1_Click: ViewModel or QuickCashCommand is null!");
        }
    }

    private void QuickCash5_Click(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("QuickCash5_Click called");
        if (ViewModel?.QuickCashCommand != null)
        {
            ViewModel.QuickCashCommand.Execute(5m);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("QuickCash5_Click: ViewModel or QuickCashCommand is null!");
        }
    }

    private void QuickCash10_Click(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("QuickCash10_Click called");
        if (ViewModel?.QuickCashCommand != null)
        {
            ViewModel.QuickCashCommand.Execute(10m);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("QuickCash10_Click: ViewModel or QuickCashCommand is null!");
        }
    }

    private void QuickCash20_Click(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("QuickCash20_Click called");
        if (ViewModel?.QuickCashCommand != null)
        {
            ViewModel.QuickCashCommand.Execute(20m);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("QuickCash20_Click: ViewModel or QuickCashCommand is null!");
        }
    }

    private void QuickCash50_Click(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("QuickCash50_Click called");
        if (ViewModel?.QuickCashCommand != null)
        {
            ViewModel.QuickCashCommand.Execute(50m);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("QuickCash50_Click: ViewModel or QuickCashCommand is null!");
        }
    }

    private void QuickCash100_Click(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("QuickCash100_Click called");
        if (ViewModel?.QuickCashCommand != null)
        {
            ViewModel.QuickCashCommand.Execute(100m);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("QuickCash100_Click: ViewModel or QuickCashCommand is null!");
        }
    }
}

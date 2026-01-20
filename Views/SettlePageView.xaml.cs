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
        ViewModel.QuickCashCommand.Execute(1m);
    }

    private void QuickCash5_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.QuickCashCommand.Execute(5m);
    }

    private void QuickCash10_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.QuickCashCommand.Execute(10m);
    }

    private void QuickCash20_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.QuickCashCommand.Execute(20m);
    }

    private void QuickCash50_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.QuickCashCommand.Execute(50m);
    }

    private void QuickCash100_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.QuickCashCommand.Execute(100m);
    }
}

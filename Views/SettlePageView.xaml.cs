using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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
}

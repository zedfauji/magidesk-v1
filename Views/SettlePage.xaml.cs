using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class SettlePage : Page
{
    public SettleViewModel ViewModel { get; }

    public SettlePage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<SettleViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is Guid ticketId)
        {
            await ViewModel.InitializeAsync(ticketId);
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class TicketManagementPage : Page
{
    public TicketManagementViewModel ViewModel { get; }

    public TicketManagementPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<TicketManagementViewModel>();
        DataContext = ViewModel;
    }

    protected override async void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.RefreshAsync();
    }
}

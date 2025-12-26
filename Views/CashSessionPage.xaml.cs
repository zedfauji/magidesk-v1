using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class CashSessionPage : Page
{
    public CashSessionViewModel ViewModel { get; }

    public CashSessionPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<CashSessionViewModel>();
        DataContext = ViewModel;
    }

    protected override async void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.RefreshAsync();
    }
}

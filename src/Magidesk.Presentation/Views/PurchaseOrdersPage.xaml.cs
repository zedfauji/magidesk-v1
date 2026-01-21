using CommunityToolkit.Mvvm.Input;
using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views;

public sealed partial class PurchaseOrdersPage : Page
{
    public PurchaseOrderViewModel ViewModel { get; }

    public PurchaseOrdersPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<PurchaseOrderViewModel>();
    }

    protected override async void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await (ViewModel.LoadDataCommand as IAsyncRelayCommand).ExecuteAsync(null);
    }
}

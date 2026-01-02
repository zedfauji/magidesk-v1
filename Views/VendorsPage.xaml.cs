using CommunityToolkit.Mvvm.Input;
using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views;

public sealed partial class VendorsPage : Page
{
    public VendorManagementViewModel ViewModel { get; }

    public VendorsPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<VendorManagementViewModel>();
    }

    protected override async void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await (ViewModel.LoadVendorsCommand as IAsyncRelayCommand).ExecuteAsync(null);
    }
}

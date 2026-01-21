using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Magidesk.Presentation.Views;

public sealed partial class DiscountManagementPage : Page
{
    public DiscountManagementViewModel ViewModel { get; }

    public DiscountManagementPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<DiscountManagementViewModel>();
        this.Name = "RootPage"; // For ElementName binding lookup
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.InitializeAsync();
    }
}

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection; // Ensure this is available

namespace Magidesk.Presentation.Views;

public sealed partial class KitchenDisplayPage : Page
{
    public KitchenDisplayViewModel ViewModel { get; }

    public KitchenDisplayPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<KitchenDisplayViewModel>();
        DataContext = ViewModel;
        this.Name = "RootPage";
    }
    
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.StartPolling();
    }
    
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        ViewModel.StopPolling();
    }
}

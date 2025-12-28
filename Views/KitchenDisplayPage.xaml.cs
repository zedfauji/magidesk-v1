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
        this.Name = "PageRoot"; // Needed for ElementName binding lookup in DataTemplate if using Binding
        ViewModel = App.Services.GetService<KitchenDisplayViewModel>()!;
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

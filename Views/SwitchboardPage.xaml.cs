using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class SwitchboardPage : Page
{
    public SwitchboardViewModel ViewModel { get; }

    public SwitchboardPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<SwitchboardViewModel>();
        DataContext = ViewModel;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        
        // Load open tickets when page is displayed
        _ = ViewModel.LoadTicketsAsync();
    }
}

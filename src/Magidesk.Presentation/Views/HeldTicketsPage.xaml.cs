using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

/// <summary>
/// Page for displaying and managing held tickets.
/// </summary>
public sealed partial class HeldTicketsPage : Page
{
    public HeldTicketsViewModel ViewModel { get; }

    public HeldTicketsPage()
    {
        this.InitializeComponent();
        
        // Get ViewModel from DI container
        ViewModel = App.Services.GetRequiredService<HeldTicketsViewModel>();
        DataContext = ViewModel;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        
        // Initialize and load held tickets when page is navigated to
        await ViewModel.InitializeAsync();
    }
}

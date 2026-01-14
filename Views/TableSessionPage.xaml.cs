using Magidesk.Presentation.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Magidesk.Presentation.Views;

/// <summary>
/// Page for managing active table sessions with control operations.
/// Provides session control (pause, resume, end), transfer, and guest count updates.
/// </summary>
public sealed partial class TableSessionPage : Page
{
    public TableSessionViewModel ViewModel { get; }

    public TableSessionPage()
    {
        this.InitializeComponent();
        
        // ViewModel will be injected via dependency injection
        ViewModel = App.Services.GetRequiredService<TableSessionViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        
        // Initialize the view model when navigating to the page
        await ViewModel.InitializeAsync();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        
        // Clean up resources when navigating away
        ViewModel.Dispose();
    }
}

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

/// <summary>
/// Page for viewing and managing audit logs.
/// </summary>
public sealed partial class AuditLogPage : Page
{
    public AuditLogViewModel ViewModel { get; }

    public AuditLogPage()
    {
        this.InitializeComponent();
        
        // Get ViewModel from DI container
        ViewModel = App.Services.GetRequiredService<AuditLogViewModel>();
        
        // Initialize the ViewModel
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.InitializeAsync();
    }

    /// <summary>
    /// Handles the Enter key press in the search box to trigger search.
    /// </summary>
    private async void OnSearchKeyboardAccelerator(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        args.Handled = true;
        await ViewModel.SearchCommand.ExecuteAsync(null);
    }
}

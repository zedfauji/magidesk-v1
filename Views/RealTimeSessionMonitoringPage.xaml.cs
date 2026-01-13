using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Magidesk.Presentation.Views;

/// <summary>
/// Real-time session monitoring dashboard page.
/// </summary>
public sealed partial class RealTimeSessionMonitoringPage : Page
{
    public RealTimeSessionMonitoringViewModel ViewModel { get; }

    public RealTimeSessionMonitoringPage()
    {
        this.InitializeComponent();
        
        // Get ViewModel from DI container
        ViewModel = App.Services.GetRequiredService<RealTimeSessionMonitoringViewModel>();
        this.DataContext = ViewModel;

        // Subscribe to session selection events
        ViewModel.SessionSelected += OnSessionSelected;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        
        // Initialize the monitoring dashboard
        await ViewModel.InitializeAsync();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        // Clean up when navigating away
        ViewModel.SessionSelected -= OnSessionSelected;
        ViewModel.Dispose();
        
        base.OnNavigatedFrom(e);
    }

    private async void OnSessionSelected(object? sender, SessionSelectedEventArgs e)
    {
        // Show session control dialog for the selected session
        try
        {
            var sessionControlViewModel = App.Services.GetRequiredService<ViewModels.Dialogs.SessionControlDialogViewModel>();
            
            // Initialize the dialog with session information
            sessionControlViewModel.Initialize(
                e.Session.SessionId,
                e.Session.TableName,
                e.Session.Status,
                4, // Default guest count - should be retrieved from session data
                e.Session.ElapsedTime,
                e.Session.PausedDuration,
                e.Session.CurrentCharge
            );

            var dialog = new Dialogs.SessionControlDialog(sessionControlViewModel);
            dialog.XamlRoot = this.XamlRoot;
            
            // Handle session control completion
            sessionControlViewModel.SessionControlCompleted += async (s, args) =>
            {
                // Refresh the monitoring dashboard to show updated session state
                await ViewModel.RefreshDataAsync();
            };

            await dialog.ShowAsync();
        }
        catch (System.Exception ex)
        {
            // Log error and show user-friendly message
            System.Diagnostics.Debug.WriteLine($"Error opening session control dialog: {ex.Message}");
            
            // TODO: Show error dialog to user
        }
    }
}
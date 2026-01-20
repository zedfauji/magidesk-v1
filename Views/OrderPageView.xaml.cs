using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Magidesk.Presentation.Views;

/// <summary>
/// Order Page View for the redesigned order entry interface.
/// </summary>
public sealed partial class OrderPageView : Page
{
    public OrderPageViewModel ViewModel { get; }

    public OrderPageView()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<OrderPageViewModel>();
        DataContext = ViewModel;
        
        // Set XamlRoot for dialogs once the page is loaded
        this.Loaded += (s, e) =>
        {
            if (this.XamlRoot != null)
            {
                ViewModel.SetXamlRoot(this.XamlRoot);
                System.Diagnostics.Debug.WriteLine("OrderPageView - XamlRoot set on ViewModel");
            }
        };
        
        System.Diagnostics.Debug.WriteLine("OrderPageView constructor - ViewModel created");
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        System.Diagnostics.Debug.WriteLine($"OrderPageView.OnNavigatedTo - Parameter: {e.Parameter?.GetType().Name ?? "null"}");

        try
        {
            // Initialize ViewModel with navigation parameters if provided
            if (e.Parameter is (Guid ticketId, Guid tableId))
            {
                System.Diagnostics.Debug.WriteLine($"OrderPageView.OnNavigatedTo - Initializing with ticketId: {ticketId}, tableId: {tableId}");
                await ViewModel.InitializeAsync(ticketId, tableId);
            }
            else if (e.Parameter is Guid id)
            {
                System.Diagnostics.Debug.WriteLine($"OrderPageView.OnNavigatedTo - Initializing with id: {id}");
                await ViewModel.InitializeAsync(id);
            }
            else
            {
                // If no parameter provided but ViewModel has a ticket, reload it
                // This handles the case when navigating back from SettlePageView
                if (ViewModel.HasTicket)
                {
                    System.Diagnostics.Debug.WriteLine("OrderPageView.OnNavigatedTo - Refreshing existing ticket");
                    await ViewModel.RefreshTicketAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("OrderPageView.OnNavigatedTo - Initializing without parameters");
                    await ViewModel.InitializeAsync();
                }
            }
            
            System.Diagnostics.Debug.WriteLine("OrderPageView.OnNavigatedTo - Initialization complete");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OrderPageView.OnNavigatedTo - ERROR: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"OrderPageView.OnNavigatedTo - Stack: {ex.StackTrace}");
        }
    }
}

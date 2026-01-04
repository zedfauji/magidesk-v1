using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Infrastructure.Services;
using Magidesk.Presentation.Services;

namespace Magidesk.Presentation;

public sealed partial class MainWindow : Window
{
    private readonly NavigationService _navigation;
    private readonly Microsoft.UI.Dispatching.DispatcherQueueTimer _clockTimer;

    public MainWindow()
    {
        StartupLogger.Log("MainWindow Constructor - Start");
        try {
            InitializeComponent();
            StartupLogger.Log("MainWindow - InitializeComponent Success");
        } catch (Exception ex) {
            StartupLogger.Log($"MainWindow - InitializeComponent FATAL: {ex}");
            throw;
        }

        StartupLogger.Log("MainWindow - Navigation initialization Start");
        try {
            _navigation = App.Services.GetRequiredService<NavigationService>();
            _navigation.Initialize(ContentFrame);
            StartupLogger.Log("MainWindow - Navigation initialization Success");
        } catch (Exception ex) {
            StartupLogger.Log($"MainWindow - Navigation initialization FATAL: {ex}");
            throw;
        }

        // Initialize Clock
        StartupLogger.Log("MainWindow - Clock Start");
        _clockTimer = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread().CreateTimer();
        _clockTimer.Interval = System.TimeSpan.FromSeconds(1);
        _clockTimer.Tick += (s, e) => { if (StatusClock != null) StatusClock.Text = System.DateTime.Now.ToString("HH:mm:ss"); };
        _clockTimer.Start();
        
        if (StatusClock != null) StatusClock.Text = System.DateTime.Now.ToString("HH:mm:ss");
        
        // AUTH GUARD: UI Visibility
        try 
        {
            var userService = App.Services.GetRequiredService<Magidesk.Application.Interfaces.IUserService>();
            // FEH-005: Fire-and-Forget Barrier
            userService.UserChanged += (s, u) => 
            {
                DispatcherQueue.TryEnqueue(() => 
                { 
                    try { UpdateUiAuthState(u); } catch (Exception ex) { StartupLogger.Log($"Auth UI Update Failed: {ex}"); }
                });
            };
            UpdateUiAuthState(userService.CurrentUser);
        }
        catch (Exception ex)
        {
             StartupLogger.Log($"MainWindow - UserService FATAL: {ex}");
        }

        StartupLogger.Log("MainWindow Constructor - End");
    }

    private void UpdateUiAuthState(Magidesk.Application.DTOs.UserDto? user)
    {
        if (RootNavigationView != null)
        {
            RootNavigationView.IsPaneVisible = user != null;
            RootNavigationView.IsSettingsVisible = user != null;
            
            // Optionally disable interaction or hide completely
            // RootNavigationView.Visibility = user != null ? Visibility.Visible : Visibility.Collapsed;
            // But if we collapse it, the ContentFrame inside might also hide if it's content?
            // NavigationView content property is the frame. Hiding NavigationView hides the frame.
            // So we must ONLY hide the PANE.
            
            RootNavigationView.PaneDisplayMode = user != null ? NavigationViewPaneDisplayMode.Left : NavigationViewPaneDisplayMode.LeftMinimal;
            RootNavigationView.IsPaneOpen = false;
            RootNavigationView.IsPaneToggleButtonVisible = user != null;
        }

        if (StatusUser != null)
        {
            StatusUser.Text = user != null ? $"{user.FirstName} {user.LastName}" : "Not Logged In";
        }
    }

    public void SetTerminalId(string terminalId)
    {
        if (StatusTerminalId != null) StatusTerminalId.Text = terminalId;
    }

    public void SetUser(string userName)
    {
        if (StatusUser != null) StatusUser.Text = userName;
    }

    private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (_navigation.CanGoBack)
        {
            _navigation.GoBack();
        }
    }

    private async void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        // FEH-001: Async Void Barrier
        try
        {
            if (args.InvokedItemContainer is not NavigationViewItem item)
            {
                return;
            }

            var tag = item.Tag?.ToString();
            if (tag == "home")
            {
                _navigation.Navigate(typeof(Views.SwitchboardPage));
                return;
            }

            if (tag == "tableMap")
            {
                _navigation.Navigate(typeof(Views.TableMapPage));
                return;
            }

            if (tag == "kitchenDisplay")
            {
                _navigation.Navigate(typeof(Views.KitchenDisplayPage));
                return;
            }
        }
        catch (Exception ex)
        {
            StartupLogger.Log($"FATAL NAV ERROR: {ex}");
            try 
            {
                // Attempt to retrieve dialog service to show error
                var dialogService = App.Services.GetRequiredService<Magidesk.Application.Interfaces.IDialogService>();
                await dialogService.ShowErrorAsync("Navigation Failed", $"Could not navigate to the requested page.\n\nError: {ex.Message}", ex.ToString());
            }
            catch (Exception dialogEx)
            {
                // Absolute last resort
                System.Diagnostics.Debug.WriteLine($"Double Fault in Navigation: {dialogEx}");
            }
        }
    }

    public void ShowLoading(string message)
    {
        if (LoadingStatus != null) LoadingStatus.Text = message;
        if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Visible;
    }

    public void HideLoading()
    {
        if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Collapsed;
    }
}

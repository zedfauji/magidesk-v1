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
        _clockTimer.Tick += (s, e) => 
        { 
            try 
            {
                if (StatusClock != null) StatusClock.Text = System.DateTime.Now.ToString("HH:mm:ss"); 
            }
            catch { /* Ignore updates if UI is tearing down */ }
        };
        _clockTimer.Start();
        
        if (StatusClock != null) StatusClock.Text = System.DateTime.Now.ToString("HH:mm:ss");
        
        // AUTH GUARD: UI Visibility
        try 
        {
            var userService = App.Services.GetRequiredService<Magidesk.Application.Interfaces.IUserService>();
            // TICKET-011: Proper async event handler instead of fire-and-forget
            userService.UserChanged += UserService_UserChanged;
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

            if (tag == "customerRecords")
            {
                _navigation.Navigate(typeof(Views.CustomerListPage));
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

    // F-ENTRY-003 FIX (TICKET-003): Persistent Error Banner Methods
    private string? _lastErrorDetails;

    public void ShowErrorBanner(string message, string? details = null)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            if (ErrorBanner != null)
            {
                ErrorBanner.Title = "System Error";
                ErrorBanner.Message = message;
                ErrorBanner.IsOpen = true;
                _lastErrorDetails = details;
            }
        });
    }

    public void HideErrorBanner()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            if (ErrorBanner != null)
            {
                ErrorBanner.IsOpen = false;
                _lastErrorDetails = null;
            }
        });
    }

    private async void ErrorBanner_DetailsClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_lastErrorDetails))
        {
            var dialog = new ContentDialog
            {
                Title = "Error Details",
                Content = new ScrollViewer
                {
                    Content = new TextBlock
                    {
                        Text = _lastErrorDetails,
                        TextWrapping = TextWrapping.Wrap,
                        FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Consolas")
                    },
                    MaxHeight = 400
                },
                CloseButtonText = "Close",
                XamlRoot = this.Content.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }

    // TICKET-011: Proper async event handler for UserChanged
    private async void UserService_UserChanged(object? sender, Magidesk.Application.DTOs.UserDto? user)
    {
        try
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                UpdateUiAuthState(user);
            });
        }
        catch (Exception ex)
        {
            StartupLogger.Log($"Auth UI Update Failed: {ex}");
            // TICKET-011: Show error banner for UI sync failures
            ShowErrorBanner($"UI sync failed: {ex.Message}");
        }
    }
}

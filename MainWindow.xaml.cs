using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.Services;

namespace Magidesk.Presentation;

public sealed partial class MainWindow : Window
{
    private readonly NavigationService _navigation;
    private readonly Microsoft.UI.Dispatching.DispatcherQueueTimer _clockTimer;

    public MainWindow()
    {
        InitializeComponent();

        _navigation = App.Services.GetRequiredService<NavigationService>();
        _navigation.Initialize(ContentFrame);

        // Maximize window by default (F-0002 requirement)
        // Note: For actual kiosk mode, we'd go full screen, but maximized is safer for dev.
        // this.Maximize(); // Requires PInvoke or specific WinUI3 calls, skipping for MVP stability unless User requests.

        // Initialize Clock
        _clockTimer = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread().CreateTimer();
        _clockTimer.Interval = System.TimeSpan.FromSeconds(1);
        _clockTimer.Tick += (s, e) => StatusClock.Text = System.DateTime.Now.ToString("HH:mm:ss");
        _clockTimer.Start();
        // Initial tick
        StatusClock.Text = System.DateTime.Now.ToString("HH:mm:ss");
    }

    public void SetTerminalId(string terminalId)
    {
        StatusTerminalId.Text = terminalId;
    }

    public void SetUser(string userName)
    {
        StatusUser.Text = userName;
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

    public void ShowLoading(string message)
    {
        LoadingStatus.Text = message;
        LoadingOverlay.Visibility = Visibility.Visible;
    }

    public void HideLoading()
    {
        LoadingOverlay.Visibility = Visibility.Collapsed;
    }
}

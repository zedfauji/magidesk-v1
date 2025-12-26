using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.Services;

namespace Magidesk.Presentation;

public sealed partial class MainWindow : Window
{
    private readonly NavigationService _navigation;

    public MainWindow()
    {
        InitializeComponent();

        _navigation = App.Services.GetRequiredService<NavigationService>();
        _navigation.Initialize(ContentFrame);

        // Default route
        _navigation.Navigate(typeof(Views.MainPage));
    }

    private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (_navigation.CanGoBack)
        {
            _navigation.GoBack();
        }
    }

    private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer is not NavigationViewItem item)
        {
            return;
        }

        var tag = item.Tag?.ToString();
        if (tag == "home")
        {
            _navigation.Navigate(typeof(Views.MainPage));
            return;
        }

        if (tag == "cashSession")
        {
            _navigation.Navigate(typeof(Views.CashSessionPage));
            return;
        }

        if (tag == "ticket")
        {
            _navigation.Navigate(typeof(Views.TicketPage));
            return;
        }

        if (tag == "payments")
        {
            _navigation.Navigate(typeof(Views.PaymentPage));
            return;
        }

        if (tag == "discountTax")
        {
            _navigation.Navigate(typeof(Views.DiscountTaxPage));
            return;
        }

        if (tag == "printing")
        {
            _navigation.Navigate(typeof(Views.PrintPage));
            return;
        }

        if (tag == "ticketMgmt")
        {
            _navigation.Navigate(typeof(Views.TicketManagementPage));
            return;
        }

        if (tag == "drawerPull")
        {
            _navigation.Navigate(typeof(Views.DrawerPullReportPage));
            return;
        }

        if (tag == "salesReports")
        {
            _navigation.Navigate(typeof(Views.SalesReportsPage));
            return;
        }

        if (tag == "userMgmt")
        {
            _navigation.Navigate(typeof(Views.UserManagementPage));
            return;
        }

        if (tag == "settings")
        {
            _navigation.Navigate(typeof(Views.SettingsPage));
        }
    }
}

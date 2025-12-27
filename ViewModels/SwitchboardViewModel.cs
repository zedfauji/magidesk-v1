using System.Threading.Tasks;
using System.Windows.Input;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.Views;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// ViewModel for the Main Switchboard (Big Button) view.
/// Acts as the primary launchpad for POS operations.
/// </summary>
public class SwitchboardViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;

    public ICommand NewTicketCommand { get; }
    public ICommand EditTicketCommand { get; }
    public ICommand SettleTicketCommand { get; }
    public ICommand ManagerFunctionsCommand { get; }
    public ICommand DrawerPullCommand { get; }
    public ICommand TablesCommand { get; }
    public ICommand ReportsCommand { get; }
    public ICommand SettingsCommand { get; }

    public SwitchboardViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;
        Title = "Magidesk POS";

        NewTicketCommand = new RelayCommand(NewTicket);
        EditTicketCommand = new RelayCommand(EditTicket);
        SettleTicketCommand = new RelayCommand(Settle);
        ManagerFunctionsCommand = new RelayCommand(ManagerFunctions);
        DrawerPullCommand = new RelayCommand(DrawerPull);
        TablesCommand = new RelayCommand(ShowTables);
        ReportsCommand = new RelayCommand(ShowReports);
        SettingsCommand = new RelayCommand(ShowSettings);
    }

    private void ShowTables()
    {
        _navigationService.Navigate(typeof(TableMapPage));
    }

    private void ShowReports()
    {
        _navigationService.Navigate(typeof(SalesReportsPage));
    }

    private void ShowSettings()
    {
        _navigationService.Navigate(typeof(SystemConfigPage));
    }

    private async void NewTicket()
    {
        var dialog = new OrderTypeSelectionDialog();
        await _navigationService.ShowDialogAsync(dialog);

        if (dialog.SelectedOrderType != null)
        {
             // Pass the selected OrderType to the TicketPage
             // We need to support parameter passing in TicketViewModel
            _navigationService.Navigate(typeof(TicketPage), dialog.SelectedOrderType);
        }
    }

    private void EditTicket()
    {
        // Navigate to Ticket Management (Open Tickets List)
        _navigationService.Navigate(typeof(TicketManagementPage));
    }

    private void Settle()
    {
        // TODO: Implement dedicated Settle Screen (F-0052)
        // For now, go to Ticket Management where Settle is an option
        _navigationService.Navigate(typeof(TicketManagementPage));
    }

    private void ManagerFunctions()
    {
        // Navigate to Admin/Back Office
        _navigationService.Navigate(typeof(UserManagementPage)); 
        // Or create a dedicated Manager Dashboard later
    }

    private void DrawerPull()
    {
        _navigationService.Navigate(typeof(DrawerPullReportPage));
    }
}

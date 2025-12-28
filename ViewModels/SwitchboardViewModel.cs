using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Magidesk.Application.Interfaces;
using Magidesk.Application.DTOs;
using Magidesk.Application.Queries;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.Views;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// ViewModel for the Main Switchboard (Big Button) view.
/// Acts as the primary launchpad for POS operations.
/// </summary>
public class SwitchboardViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;
    private readonly ICashSessionRepository _cashSessionRepository;
    private readonly IQueryHandler<GetOpenTicketsQuery, IEnumerable<TicketDto>> _getOpenTicketsHandler;

    private ObservableCollection<TicketDto> _openTickets = new();
    public ObservableCollection<TicketDto> OpenTickets
    {
        get => _openTickets;
        set => SetProperty(ref _openTickets, value);
    }

    private TicketDto? _selectedTicket;
    public TicketDto? SelectedTicket
    {
        get => _selectedTicket;
        set
        {
            if (SetProperty(ref _selectedTicket, value))
            {
               // Optional: Auto-trigger edit if selection changes
            }
        }
    }

    public ICommand LoadTicketsCommand { get; }
    public ICommand NewTicketCommand { get; }
    public ICommand EditTicketCommand { get; }
    public ICommand SettleTicketCommand { get; }
    public ICommand ManagerFunctionsCommand { get; }
    public ICommand DrawerPullCommand { get; }
    public ICommand TablesCommand { get; }
    public ICommand ReportsCommand { get; }
    public ICommand SettingsCommand { get; }
    public ICommand KitchenCommand { get; }
    public ICommand CashDropCommand { get; }
    public ICommand PayoutCommand { get; }
    public ICommand DrawerBleedCommand { get; }
    public ICommand OpenDrawerCommand { get; }
    public ICommand DrawerBalanceCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand ShutdownCommand { get; }

    public SwitchboardViewModel(
        NavigationService navigationService,
        ICashSessionRepository cashSessionRepository,
        IQueryHandler<GetOpenTicketsQuery, IEnumerable<TicketDto>> getOpenTicketsHandler)
    {
        _navigationService = navigationService;
        _cashSessionRepository = cashSessionRepository;
        _getOpenTicketsHandler = getOpenTicketsHandler;
        Title = "Magidesk POS";

        LoadTicketsCommand = new AsyncRelayCommand(LoadTicketsAsync);
        NewTicketCommand = new RelayCommand(NewTicket);
        EditTicketCommand = new RelayCommand(EditTicket);
        SettleTicketCommand = new RelayCommand(Settle);
        ManagerFunctionsCommand = new RelayCommand(ManagerFunctions);
        DrawerPullCommand = new RelayCommand(DrawerPull);
        TablesCommand = new RelayCommand(ShowTables);
        ReportsCommand = new RelayCommand(ShowReports);
        SettingsCommand = new RelayCommand(ShowSettings);
        KitchenCommand = new RelayCommand(ShowKitchen);
        
        LogoutCommand = new RelayCommand(Logout);
        ShutdownCommand = new RelayCommand(Shutdown);
        
        CashDropCommand = new AsyncRelayCommand(PerformCashDropAsync);
        PayoutCommand = new AsyncRelayCommand(PerformPayoutAsync);
        DrawerBleedCommand = new AsyncRelayCommand(PerformDrawerBleedAsync);
        OpenDrawerCommand = new AsyncRelayCommand(PerformOpenDrawerAsync);
        DrawerBalanceCommand = new AsyncRelayCommand(ShowDrawerBalanceAsync);
        
        // Initial load
        _ = LoadTicketsAsync();
    }

    private void Logout()
    {
        _navigationService.Navigate(typeof(LoginPage));
    }

    private void Shutdown()
    {
        App.Current.Exit();
    }

    private void ShowKitchen()
    {
        _navigationService.Navigate(typeof(Views.KitchenDisplayPage));
    }

    private void OpenNewTicket()
    {
         _navigationService.Navigate(typeof(Views.OrderEntryPage));
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
            _navigationService.Navigate(typeof(TicketPage), dialog.SelectedOrderType);
        }
    }

    private async Task LoadTicketsAsync()
    {
        try
        {
            var tickets = await _getOpenTicketsHandler.HandleAsync(new GetOpenTicketsQuery());
            OpenTickets = new ObservableCollection<TicketDto>(tickets);
        }
        catch (Exception ex)
        {
             // Log or show error
             System.Diagnostics.Debug.WriteLine($"Error loading tickets: {ex.Message}");
        }
    }

    private void EditTicket()
    {
        if (SelectedTicket != null)
        {
            // Navigate to Order Entry with Ticket ID
             _navigationService.Navigate(typeof(Views.OrderEntryPage), SelectedTicket.Id);
        }
        else
        {
             // If no ticket selected, navigate to TicketManagementPage (default existing behavior fallback)
             // or show a message "Select a ticket".
             // For now, mapping to OrderEntryPage as a default "generic" load is safer if we want to create new.
             _navigationService.Navigate(typeof(TicketManagementPage));
        }
    }

    private void Settle()
    {
        _navigationService.Navigate(typeof(TicketManagementPage));
    }

    private void ManagerFunctions()
    {
        _navigationService.Navigate(typeof(Views.BackOfficePage)); 
    }

    private void DrawerPull()
    {
        _navigationService.Navigate(typeof(DrawerPullReportPage));
    }

    private async Task PerformCashDropAsync()
    {
        await PerformDrawerOperationAsync(isPayout: false);
    }

    private async Task PerformPayoutAsync()
    {
         await PerformDrawerOperationAsync(isPayout: true);
    }
    
    private async Task PerformDrawerBleedAsync()
    {
         await PerformDrawerOperationAsync(isPayout: true, isBleed: true);
    }

    private async Task PerformOpenDrawerAsync()
    {
        // "No Sale" operation
        var terminalId = System.Guid.Parse("22222222-2222-2222-2222-222222222222"); // Harcoded MVP
        try 
        {
            var session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
            if (session != null)
            {
                 var transaction = new Magidesk.Domain.Entities.TerminalTransaction(
                     session.Id, 
                     Magidesk.Domain.Enumerations.TerminalTransactionType.NoSale, 
                     Magidesk.Domain.ValueObjects.Money.Zero(), 
                     "Manual Open Drawer");
                 
                 session.AddTransaction(transaction);
                 await _cashSessionRepository.UpdateAsync(session);
            }
        }
        catch (System.Exception ex)
        {
             System.Diagnostics.Debug.WriteLine($"Open Drawer Error: {ex.Message}");
        }
    }

    private async Task ShowDrawerBalanceAsync()
    {
        var terminalId = System.Guid.Parse("22222222-2222-2222-2222-222222222222"); // Harcoded MVP
        try 
        {
            var session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
             
            string message = "No open session found.";
            if (session != null)
            {
                message = $"Estimated Drawer Balance:\n\n{session.ExpectedCash:C}";
            }

            var dialog = new ContentDialog
            {
                Title = "Drawer Balance",
                Content = message,
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close
            };
            await _navigationService.ShowDialogAsync(dialog);
        }
        catch (System.Exception ex)
        {
             System.Diagnostics.Debug.WriteLine($"Balance Error: {ex.Message}");
        }
    }

    private async Task PerformDrawerOperationAsync(bool isPayout, bool isBleed = false)
    {
        string title;
        string message;

        if (isBleed)
        {
            title = "Drawer Bleed";
            message = "Enter amount to bleed from drawer (Internal Transfer).";
        }
        else
        {
             title = isPayout ? "Pay Out" : "Cash Drop";
             message = isPayout ? "Enter amount to remove from drawer." : "Enter amount to drop into safe.";
        }
        
        var dialog = new Magidesk.Presentation.Views.CashEntryDialog(title, message);
        var result = await _navigationService.ShowDialogAsync(dialog);

        if (result == ContentDialogResult.Primary)
        {
            var amount = new Magidesk.Domain.ValueObjects.Money(dialog.Amount);
            var reason = dialog.Reason;
            
            var userId = new Magidesk.Domain.ValueObjects.UserId(System.Guid.Parse("11111111-1111-1111-1111-111111111111"));
            var terminalId = System.Guid.Parse("22222222-2222-2222-2222-222222222222");

            try
            {
                var session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
                if (session == null) return;

                if (isBleed)
                {
                    var bleed = Magidesk.Domain.Entities.DrawerBleed.Create(session.Id, amount, userId, reason);
                    session.AddDrawerBleed(bleed);
                }
                else if (isPayout)
                {
                    var payout = Magidesk.Domain.Entities.Payout.Create(session.Id, amount, userId, reason);
                    session.AddPayout(payout);
                }
                else
                {
                    var drop = Magidesk.Domain.Entities.CashDrop.Create(session.Id, amount, userId, reason);
                    session.AddCashDrop(drop);
                }

                await _cashSessionRepository.UpdateAsync(session);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Drawer Op Error: {ex.Message}");
            }
        }
    }
}

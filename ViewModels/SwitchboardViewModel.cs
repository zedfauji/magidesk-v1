using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Queries;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.Views;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Presentation.Views.Dialogs;

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
    private readonly ICommandHandler<ClockInCommand> _clockInHandler;
    private readonly ICommandHandler<ClockOutCommand> _clockOutHandler;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly ICommandHandler<CreateTicketCommand, CreateTicketResult> _createTicketHandler;
    private readonly IUserService _userService;

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
        set => SetProperty(ref _selectedTicket, value);
    }

    public ICommand LoadTicketsCommand { get; }
    public ICommand NewTicketCommand { get; }
    public ICommand ClockInCommand { get; }
    public ICommand ClockOutCommand { get; }
    public ICommand EditTicketCommand { get; }
    public ICommand SettleCommand { get; }
    public ICommand DrawerPullCommand { get; }
    public ICommand PerformCashDropCommand { get; }
    public ICommand PerformPayoutCommand { get; }
    public ICommand PerformDrawerBleedCommand { get; }
    public ICommand PerformOpenDrawerCommand { get; }
    public ICommand ShowDrawerBalanceCommand { get; }
    
    public ICommand TablesCommand { get; }
    public ICommand ManagerFunctionsCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand ShutdownCommand { get; }

    public SwitchboardViewModel(
        NavigationService navigationService,
        ICashSessionRepository cashSessionRepository,
        IQueryHandler<GetOpenTicketsQuery, IEnumerable<TicketDto>> getOpenTicketsHandler,
        ICommandHandler<ClockInCommand> clockInHandler,
        ICommandHandler<ClockOutCommand> clockOutHandler,
        IAttendanceRepository attendanceRepository,
        ICommandHandler<CreateTicketCommand, CreateTicketResult> createTicketHandler,
        IUserService userService)
    {
        _navigationService = navigationService;
        _cashSessionRepository = cashSessionRepository;
        _getOpenTicketsHandler = getOpenTicketsHandler;
        _clockInHandler = clockInHandler;
        _clockOutHandler = clockOutHandler;
        _attendanceRepository = attendanceRepository;
        _createTicketHandler = createTicketHandler;
        _userService = userService;
        Title = "Magidesk POS";

        LoadTicketsCommand = new AsyncRelayCommand(LoadTicketsAsync);
        ClockInCommand = new AsyncRelayCommand(ClockInAsync);
        ClockOutCommand = new AsyncRelayCommand(ClockOutAsync);
        EditTicketCommand = new RelayCommand(EditTicket);
        SettleCommand = new RelayCommand(Settle);
        DrawerPullCommand = new RelayCommand(DrawerPull);
        PerformCashDropCommand = new AsyncRelayCommand(PerformCashDropAsync);
        PerformPayoutCommand = new AsyncRelayCommand(PerformPayoutAsync);
        PerformDrawerBleedCommand = new AsyncRelayCommand(PerformDrawerBleedAsync);
        PerformOpenDrawerCommand = new AsyncRelayCommand(PerformOpenDrawerAsync);
        ShowDrawerBalanceCommand = new AsyncRelayCommand(ShowDrawerBalanceAsync);
        
        TablesCommand = new RelayCommand(() => System.Diagnostics.Debug.WriteLine("Tables Not Implemented"));
        ManagerFunctionsCommand = new RelayCommand(() => System.Diagnostics.Debug.WriteLine("Manager Not Implemented"));
        LogoutCommand = new RelayCommand(() => _navigationService.GoBack());
        ShutdownCommand = new RelayCommand(() => { try { Microsoft.UI.Xaml.Application.Current.Exit(); } catch {} });

        NewTicketCommand = new AsyncRelayCommand(NewTicketAsync);
    }

    // ... 

    private async Task NewTicketAsync()
    {
        // F-0019: New Ticket Action
        // 1. Order Type Selection
        var dialog = new OrderTypeSelectionDialog();
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot; // Ensure Root
        await _navigationService.ShowDialogAsync(dialog);

        if (dialog.SelectedOrderType != null)
        {
            // F-0020: Strict Guards
            if (dialog.SelectedOrderType.RequiresTable)
            {
                // Strict: Must have table.
                // TODO: F-0082 Table Selection
                var errorDialog = new ContentDialog
                {
                    Title = "Action Required",
                    Content = $"Order Type '{dialog.SelectedOrderType.Name}' requires a Table. Table Selection is not yet linked.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindowInstance.Content.XamlRoot
                };
                await _navigationService.ShowDialogAsync(errorDialog);
                return; // Block creation
            }

            if (dialog.SelectedOrderType.RequiresCustomer)
            {
                // Strict: Must have customer.
                // TODO: F-0077 Customer Selection
                var errorDialog = new ContentDialog
                {
                    Title = "Action Required",
                    Content = $"Order Type '{dialog.SelectedOrderType.Name}' requires a Customer. Customer Selection is not yet linked.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindowInstance.Content.XamlRoot
                };
                await _navigationService.ShowDialogAsync(errorDialog);
                await _navigationService.ShowDialogAsync(errorDialog);
                 return; // Block creation
            }

            int numberOfGuests = 1;

            if (dialog.SelectedOrderType.Name.ToUpper().Contains("DINE IN"))
            {
                // F-0023: Guest Count Entry Dialog
                // Strict Parity: Prompt for guest count on new Dine In ticket.
                var guestCountVm = App.Services.GetRequiredService<ViewModels.GuestCountViewModel>();
                var guestCountDialog = new GuestCountDialog(guestCountVm);
                guestCountDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
                
                var result = await _navigationService.ShowDialogAsync(guestCountDialog);
                if (result == ContentDialogResult.Primary)
                {
                    if (guestCountVm.GuestCount > 0)
                    {
                        numberOfGuests = guestCountVm.GuestCount;
                    }
                    else
                    {
                        // Decide policy: if 0 entered, default to 1 or block?
                        // Audit says "Skip guest count: Default to 1".
                        numberOfGuests = 1; 
                    }
                }
                else
                {
                    // Cancelled dialog -> Cancel ticket creation?
                    return; 
                }
            }

            // 2. Resolve Context
            var userId = _userService.CurrentUser?.Id ?? new Magidesk.Domain.ValueObjects.UserId(Guid.Parse("11111111-1111-1111-1111-111111111111")); // Fallback/Dev
            // Terminal should ideally come from local config or session.
            // For MVP, checking open session or defaulting.
            var terminalId = Guid.Parse("22222222-2222-2222-2222-222222222222"); 
            
            // Resolve Shift (Active Session)
            var session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
            var shiftId = session?.Id ?? Guid.Parse("33333333-3333-3333-3333-333333333333"); // Fallback

            // 3. Create Ticket (Backend Command)
            var command = new CreateTicketCommand
            {
                CreatedBy = userId,
                TerminalId = terminalId,
                ShiftId = shiftId,
                OrderTypeId = dialog.SelectedOrderType.Id,
                NumberOfGuests = numberOfGuests
            };

            try 
            {
                var result = await _createTicketHandler.HandleAsync(command);
                
                // 4. Navigate to Order Entry with New Ticket ID
                _navigationService.Navigate(typeof(Views.OrderEntryPage), result.TicketId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Create Ticket Failed: {ex.Message}");
                // Ideally show error dialog
            }
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
        // F-0011: Use Open Tickets List Dialog instead of generic management page
        var dialog = new Magidesk.Presentation.Views.OpenTicketsListDialog();
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        _ = _navigationService.ShowDialogAsync(dialog);
    }

    private async void DrawerPull()
    {
        // F-0012: Use Drawer Pull Report Dialog
        var dialog = new Magidesk.Presentation.Views.DrawerPullReportDialog();
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        await _navigationService.ShowDialogAsync(dialog);
    }

    private async Task PerformCashDropAsync()
    {
        // F-0010: Use Management Dialog
        var dialog = new Magidesk.Presentation.Views.CashDropManagementDialog();
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        await _navigationService.ShowDialogAsync(dialog);
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
    private async Task ClockInAsync()
    {
         var userId = _userService.CurrentUser?.Id ?? new Magidesk.Domain.ValueObjects.UserId(System.Guid.Parse("11111111-1111-1111-1111-111111111111"));
         var command = new ClockInCommand { UserId = userId };
         try 
         {
             await _clockInHandler.HandleAsync(command);
         }
         catch (Exception ex)
         {
             System.Diagnostics.Debug.WriteLine($"Clock In Error: {ex.Message}");
         }
    }

    private async Task ClockOutAsync()
    {
         var userId = _userService.CurrentUser?.Id ?? new Magidesk.Domain.ValueObjects.UserId(System.Guid.Parse("11111111-1111-1111-1111-111111111111"));
         var command = new ClockOutCommand { UserId = userId };
         try 
         {
             await _clockOutHandler.HandleAsync(command);
         }
         catch (Exception ex)
         {
             System.Diagnostics.Debug.WriteLine($"Clock Out Error: {ex.Message}");
         }
    }
}

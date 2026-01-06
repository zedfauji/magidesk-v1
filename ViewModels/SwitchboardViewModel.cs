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
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Presentation.Views.Dialogs;
using Magidesk.Domain.Enumerations;
using System.Linq;
using Microsoft.Extensions.Logging;

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
    private readonly ITerminalContext _terminalContext;

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
    public ICommand KitchenCommand { get; }
    
    public ICommand TablesCommand { get; }
    public ICommand ManagerFunctionsCommand { get; }
    public ICommand BackOfficeCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand ShutdownCommand { get; }

    private readonly ISecurityService _securityService;
    private readonly IAesEncryptionService _encryptionService;
    private readonly ISwitchboardDialogService _switchboardDialogService;
    private readonly IOrderTypeRepository _orderTypeRepository;
    private readonly IShiftRepository _shiftRepository;
    private readonly ICommandHandler<OpenCashSessionCommand, OpenCashSessionResult> _openSessionHandler;
    private readonly ILogger<SwitchboardViewModel> _logger;

    public SwitchboardViewModel(
        NavigationService navigationService,
        ICashSessionRepository cashSessionRepository,
        IQueryHandler<GetOpenTicketsQuery, IEnumerable<TicketDto>> getOpenTicketsHandler,
        ICommandHandler<ClockInCommand> clockInHandler,
        ICommandHandler<ClockOutCommand> clockOutHandler,
        IAttendanceRepository attendanceRepository,
        ICommandHandler<CreateTicketCommand, CreateTicketResult> createTicketHandler,
        IUserService userService,
        ITerminalContext terminalContext,
        ISecurityService securityService,
        IAesEncryptionService encryptionService,
        ISwitchboardDialogService switchboardDialogService,
        IOrderTypeRepository orderTypeRepository,
        IShiftRepository shiftRepository,
        ICommandHandler<OpenCashSessionCommand, OpenCashSessionResult> openSessionHandler,
        ILogger<SwitchboardViewModel> logger)
    {
        _navigationService = navigationService;
        _cashSessionRepository = cashSessionRepository;
        _getOpenTicketsHandler = getOpenTicketsHandler;
        _clockInHandler = clockInHandler;
        _clockOutHandler = clockOutHandler;
        _attendanceRepository = attendanceRepository;
        _createTicketHandler = createTicketHandler;
        _userService = userService;
        _terminalContext = terminalContext;
        _securityService = securityService;
        _encryptionService = encryptionService;
        _switchboardDialogService = switchboardDialogService;
        _orderTypeRepository = orderTypeRepository;
        _shiftRepository = shiftRepository;
        _openSessionHandler = openSessionHandler;
        _logger = logger;
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
        
        KitchenCommand = new RelayCommand(() => _navigationService.Navigate(typeof(Views.KitchenDisplayPage)));
        
        TablesCommand = new RelayCommand(() => _navigationService.Navigate(typeof(Views.TableMapPage)));
        ManagerFunctionsCommand = new AsyncRelayCommand(ManagerFunctionsAsync);
        BackOfficeCommand = new AsyncRelayCommand(BackOfficeAsync);
        LogoutCommand = new RelayCommand(() => {
            _navigationService.Navigate(typeof(Views.LoginPage));
        });
        // TICKET-015: Proper shutdown error handling
        ShutdownCommand = new RelayCommand(() => 
        { 
            try 
            { 
                Microsoft.UI.Xaml.Application.Current.Exit(); 
            } 
            catch (Exception ex) 
            {
                // Shutdown failure is rare but should be logged
                _logger.LogError(ex, "Application shutdown failed");
                // Force exit as last resort
                Environment.Exit(0);
            }
        });

        NewTicketCommand = new AsyncRelayCommand(NewTicketAsync);

    }

    private async Task BackOfficeAsync()
    {
        // Security Gate for Admin/Backoffice
        var passwordDialog = new Views.PasswordEntryDialog();
        passwordDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        var result = await _navigationService.ShowDialogAsync(passwordDialog);

        if (result != ContentDialogResult.Primary) return;

        var pin = passwordDialog.Password;
        if (string.IsNullOrWhiteSpace(pin)) return;

        try
        {
            var encryptedPin = _encryptionService.Encrypt(pin);
            var user = await _securityService.GetUserByPinAsync(encryptedPin);

            if (user == null)
            {
                await _navigationService.ShowErrorAsync("Authentication Failed", "Invalid PIN.");
                return;
            }

            // Check Admin Permissions
            var adminPermissions = UserPermission.ManageUsers | UserPermission.ManageTableLayout | 
                                 UserPermission.ManageMenu | UserPermission.ViewReports | 
                                 UserPermission.SystemConfiguration;

            if ((user.Role.Permissions & adminPermissions) == 0)
            {
                await _navigationService.ShowErrorAsync("Access Denied", "Insufficient privileges for Back Office functions.");
                return;
            }

            // Auth Success - Navigate to Back Office hub
            _navigationService.Navigate(typeof(Views.BackOfficePage));
        }
        catch (Exception ex)
        {
            // T-008: Visible Failure
            await _navigationService.ShowErrorAsync("Access Error", $"Authentication System Error:\n{ex.Message}");
        }
    }

    private async Task ManagerFunctionsAsync()
    {
        // Manager Authentication Gate
        var passwordDialog = new Views.PasswordEntryDialog();
        passwordDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        var result = await _navigationService.ShowDialogAsync(passwordDialog);

        if (result != ContentDialogResult.Primary) return;

        var pin = passwordDialog.Password;
        if (string.IsNullOrWhiteSpace(pin)) return;

        try
        {
            var encryptedPin = _encryptionService.Encrypt(pin);
            var user = await _securityService.GetUserByPinAsync(encryptedPin);

            if (user == null)
            {
                await _navigationService.ShowErrorAsync("Authentication Failed", "Invalid PIN.");
                return;
            }

            // Check permissions (Any Manager or Admin function)
            var managerPermissions = UserPermission.VoidTicket | UserPermission.RefundPayment | 
                                   UserPermission.OpenDrawer | UserPermission.CloseBatch | 
                                   UserPermission.ApplyDiscount | UserPermission.ManageUsers | 
                                   UserPermission.ManageTableLayout | UserPermission.ManageMenu | 
                                   UserPermission.ViewReports | UserPermission.SystemConfiguration;

            if ((user.Role.Permissions & managerPermissions) == 0)
            {
                 await _navigationService.ShowErrorAsync("Access Denied", "Insufficient privileges for Manager functions.");
                 return;
            }

            // Auth Success - Navigate
            var dialog = new Views.ManagerFunctionsDialog();
            await _navigationService.ShowDialogAsync(dialog);
        }
        catch (Exception ex)
        {
            // T-008: Visible Failure
            await _navigationService.ShowErrorAsync("Access Error", $"Manager Auth System Error:\n{ex.Message}");
        }
    }

    private async Task NewTicketAsync()
    {
        // F-0019: New Ticket Action
        // 1. Order Type Selection
        var orderTypeVm = new OrderTypeSelectionViewModel(_orderTypeRepository);
        await _switchboardDialogService.ShowOrderTypeSelectionAsync(orderTypeVm);

        if (orderTypeVm.SelectedOrderType != null)
        {
            var selectedOrderType = orderTypeVm.SelectedOrderType;

            // F-0020: Strict Guards
            if (selectedOrderType.RequiresTable)
            {
                // Strict: Must have table.
                // TODO: F-0082 Table Selection
                await _navigationService.ShowErrorAsync("Action Required", $"Order Type '{selectedOrderType.Name}' requires a Table. Table Selection is not yet linked.");
                return; // Block creation
            }

            if (selectedOrderType.RequiresCustomer)
            {
                // Strict: Must have customer.
                // TODO: F-0077 Customer Selection
                await _navigationService.ShowErrorAsync("Action Required", $"Order Type '{selectedOrderType.Name}' requires a Customer. Customer Selection is not yet linked.");
                return; // Block creation
            }

            int numberOfGuests = 1;

            if (selectedOrderType.Name.ToUpper().Contains("DINE IN"))
            {
                // F-0023: Guest Count Entry Dialog
                // Strict Parity: Prompt for guest count on new Dine In ticket.
                var guestCountVm = new GuestCountViewModel();
                
                await _switchboardDialogService.ShowGuestCountAsync(guestCountVm);
                
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

            // 2. Resolve Context (no fallback IDs)
            if (_userService.CurrentUser?.Id == null)
            {
                await _navigationService.ShowErrorAsync("Action Required", "No current user is set. Please login again.");
                return;
            }

            if (_terminalContext.TerminalId == null)
            {
                await _navigationService.ShowErrorAsync("Action Required", "Terminal identity is not initialized. Please restart the application.");
                return;
            }

            var userId = _userService.CurrentUser.Id;
            var terminalId = _terminalContext.TerminalId.Value;

            // Resolve Shift (Active Session) - must exist
            var session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
            if (session == null)
            {
                // F-0060: Shift Start Dialog
                var shiftStartVm = new ShiftStartViewModel(_shiftRepository, _openSessionHandler, _userService, _terminalContext);
                
                await _switchboardDialogService.ShowShiftStartAsync(shiftStartVm);

                // Re-fetch session to confirm it started
                session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
                if (session == null)
                {
                    // If still null, something went wrong or user cancelled
                    return;
                }
            }

            var shiftId = session.Id;

            // 3. Create Ticket (Backend Command)
            var command = new CreateTicketCommand
            {
                CreatedBy = userId,
                TerminalId = terminalId,
                ShiftId = shiftId,
                OrderTypeId = selectedOrderType.Id,
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
                // T-006: Visible Failure
                await _navigationService.ShowErrorAsync("Create Ticket Failed", $"Critical Error creating ticket:\n{ex.Message}\n\nPlease check database connection.");
            }
        }
    }

    public async Task LoadTicketsAsync()
    {
        try
        {
            var tickets = await _getOpenTicketsHandler.HandleAsync(new GetOpenTicketsQuery());
            OpenTickets = new ObservableCollection<TicketDto>(tickets.OrderBy(t => t.TableNumbers.FirstOrDefault()));
        }
        catch (Exception ex)
        {
             // T-009: Visible Failure
             // This is critical; if we can't load tickets, the POS is effectively offline.
             await _navigationService.ShowErrorAsync("Connection Error", $"Failed to load open tickets:\n{ex.Message}\n\nPlease check network/database.");
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
             // If no ticket selected, navigate to MainPage (utility/debug fallback)
             _navigationService.Navigate(typeof(Views.MainPage));
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
        if (_terminalContext.TerminalId == null)
        {
            return;
        }

        var terminalId = _terminalContext.TerminalId.Value;
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
             // T-007: Visible Failure
             await _navigationService.ShowErrorAsync("Drawer Error", $"Failed to open drawer:\n{ex.Message}");
        }
    }

    private async Task ShowDrawerBalanceAsync()
    {
        if (_terminalContext.TerminalId == null)
        {
            return;
        }

        var terminalId = _terminalContext.TerminalId.Value;
        try 
        {
            var session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
             
            string message = "No open session found.";
            if (session != null)
            {
                message = $"Estimated Drawer Balance:\n\n{session.ExpectedCash:C}";
            }

            await _navigationService.ShowMessageAsync("Drawer Balance", message);
        }
        catch (System.Exception ex)
        {
             // T-007: Visible Failure
             await _navigationService.ShowErrorAsync("Balance Error", $"Failed to retrieve balance:\n{ex.Message}");
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

            if (_userService.CurrentUser?.Id == null || _terminalContext.TerminalId == null)
            {
                return;
            }

            var userId = _userService.CurrentUser.Id;
            var terminalId = _terminalContext.TerminalId.Value;

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
                // T-007: Visible Failure
                await _navigationService.ShowErrorAsync("Transaction Error", $"Drawer operation failed:\n{ex.Message}");
            }
        }
    }
    private async Task ClockInAsync()
    {
         if (_userService.CurrentUser?.Id == null)
         {
             return;
         }

         var userId = _userService.CurrentUser.Id;
         var command = new ClockInCommand { UserId = userId };
         try 
         {
             await _clockInHandler.HandleAsync(command);
         }
          catch (Exception ex)
          {
              // T-004: Visible Failure
              await _navigationService.ShowErrorAsync("Timeclock Error", $"Failed to Clock In:\n{ex.Message}");
          }
    }

    private async Task ClockOutAsync()
    {
         if (_userService.CurrentUser?.Id == null)
         {
             return;
         }

         var userId = _userService.CurrentUser.Id;
         var command = new ClockOutCommand { UserId = userId };
         try 
         {
             await _clockOutHandler.HandleAsync(command);
         }
          catch (Exception ex)
          {
              // T-004: Visible Failure
              await _navigationService.ShowErrorAsync("Timeclock Error", $"Failed to Clock Out:\n{ex.Message}");
          }
    }
}

using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using System.Net.Sockets;
using Magidesk.Presentation.Services;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using System.Net.Sockets;
using Magidesk.Presentation.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection; // Added for IServiceScopeFactory
using Magidesk.ViewModels; // For GratuitySelectionViewModel
using Magidesk.Views.Dialogs; // For GratuitySelectionDialog

namespace Magidesk.Presentation.ViewModels;

public sealed class SettleViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetTicketQuery, TicketDto?> _getTicket;
    private readonly ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult> _processPayment;
    private readonly ICommandHandler<SetTaxExemptCommand, SetTaxExemptResult> _setTaxExempt;
    private readonly ICommandHandler<LogoutCommand> _logoutHandler;
    private readonly Services.NavigationService _navigationService;
    private readonly IUserService _userService;
    private readonly ITerminalContext _terminalContext;
    private readonly ICashSessionRepository _cashSessionRepository;
    private readonly ICommandHandler<OpenCashDrawerCommand> _openCashDrawer;
    private readonly ILogger<SettleViewModel> _logger;
    private readonly Services.LocalizationService _localizationService;
    private readonly Domain.Services.IGratuityService _gratuityService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public Services.LocalizationService Localization { get; }

    private TicketDto? _ticket;
    private decimal _tenderAmount;
    private string _numpadInput = "";
    private string? _error;
    private string? _statusMessage;

    private Guid _ticketId;

    public SettleViewModel(
        IQueryHandler<GetTicketQuery, TicketDto?> getTicket,
        ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult> processPayment,
        ICommandHandler<SetTaxExemptCommand, SetTaxExemptResult> setTaxExempt,
        ICommandHandler<LogoutCommand> logoutHandler,
        Services.NavigationService navigationService,
        IUserService userService,
        ITerminalContext terminalContext,
        ICashSessionRepository cashSessionRepository,
        ICommandHandler<OpenCashDrawerCommand> openCashDrawer,
        ILogger<SettleViewModel> logger,
        Services.LocalizationService localizationService,
        Domain.Services.IGratuityService gratuityService,
        IServiceScopeFactory serviceScopeFactory)
    {
        _getTicket = getTicket;
        _processPayment = processPayment;
        _setTaxExempt = setTaxExempt;
        _logoutHandler = logoutHandler;
        _navigationService = navigationService;
        _userService = userService;
        _terminalContext = terminalContext;
        _cashSessionRepository = cashSessionRepository;
        _openCashDrawer = openCashDrawer;
        _logger = logger;
        _localizationService = localizationService;
        _gratuityService = gratuityService ?? throw new ArgumentNullException(nameof(gratuityService));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        Localization = localizationService;

        Title = "Settle Ticket";

        NumpadCommand = new RelayCommand<string>(OnNumpadInput);
        ClearNumpadCommand = new RelayCommand(OnClearNumpad);
        BackspaceCommand = new RelayCommand(OnBackspace);
        ProcessPaymentCommand = new AsyncRelayCommand<string>(ProcessPaymentAsync);
        ToggleTaxExemptCommand = new AsyncRelayCommand(ToggleTaxExemptAsync);
        CloseCommand = new RelayCommand(OnClose);
        
        NextAmountCommand = new RelayCommand(OnNextAmount);
        NoSaleCommand = new AsyncRelayCommand(OnNoSaleAsync);
        LogoutUiCommand = new AsyncRelayCommand(OnLogoutAsync);
        TestWaitCommand = new AsyncRelayCommand(TestWaitAsync);
        SwipeCardCommand = new AsyncRelayCommand(SwipeCardAsync);
        ExactAmountCommand = new RelayCommand(OnExactAmount);
        QuickCashCommand = new RelayCommand<string>(OnQuickCash);
        QuickCashCommand = new RelayCommand<string>(OnQuickCash);
        ShowGratuityDialogCommand = new AsyncRelayCommand(ShowGratuityDialogAsync);
        VoidTicketCommand = new AsyncRelayCommand(OnVoidTicketAsync);
        ReprintReceiptCommand = new AsyncRelayCommand(OnReprintReceiptAsync);
    }

    public TicketDto? Ticket
    {
        get => _ticket;
        private set
        {
            if (SetProperty(ref _ticket, value))
            {
                OnPropertyChanged(nameof(TotalAmount));
                OnPropertyChanged(nameof(PaidAmount));
                OnPropertyChanged(nameof(DueAmount));
                OnPropertyChanged(nameof(IsTaxExempt));
                OnPropertyChanged(nameof(TaxAmount));
                OnPropertyChanged(nameof(TicketNumber));
                OnPropertyChanged(nameof(TableName));
                
                // Explicitly update observable property
                CanAddGratuity = value != null;
                
                // Auto-set tender amount to due amount if input is empty
                if (string.IsNullOrEmpty(_numpadInput) && value != null)
                {
                   TenderAmount = value.DueAmount;
                }
            }
        }
    }

    public decimal TenderAmount
    {
        get => _tenderAmount;
        set => SetProperty(ref _tenderAmount, value);
    }
    
    // ... existing NumpadInput ...

    public string NumpadInput
    {
        get => _numpadInput;
        set
        {
            if (SetProperty(ref _numpadInput, value))
            {
                if (decimal.TryParse(value, out var amount))
                {
                    // Assuming input is in cents/minor units if purely digits, or standard decimal input
                    // For POS, often typing "100" means "1.00". 
                    // Let's implement standard decimal input for now (1 -> 1, 1. -> 1., 1.5 -> 1.5)
                    TenderAmount = amount;
                }
                else if (string.IsNullOrEmpty(value))
                {
                    TenderAmount = 0;
                }
            }
        }
    }

    public bool IsTaxExempt => Ticket?.IsTaxExempt ?? false;
    public decimal TotalAmount => Ticket?.TotalAmount ?? 0;
    public decimal PaidAmount => Ticket?.PaidAmount ?? 0;
    public decimal DueAmount => Ticket?.DueAmount ?? 0;
    public decimal TaxAmount => Ticket?.TaxAmount ?? 0;
    public string TicketNumber => Ticket != null ? $"Ticket #{Ticket.TicketNumber}" : "No Ticket";
    public string TableName => Ticket?.TableName ?? "No Table";
    
    public bool HasDueAmount => DueAmount > 0;
    
    private bool _canAddGratuity;
    public bool CanAddGratuity
    {
        get => _canAddGratuity;
        set => SetProperty(ref _canAddGratuity, value);
    }

    public string? Error
    {
        get => _error;
        set 
        {
            if (SetProperty(ref _error, value) && !string.IsNullOrEmpty(value))
            {
                StatusMessage = null; // Clear status if error occurs
            }
        }
    }

    public string? StatusMessage
    {
        get => _statusMessage;
        set 
        {
            if (SetProperty(ref _statusMessage, value) && !string.IsNullOrEmpty(value))
            {
                Error = null; // Clear error if status occurs
            }
        }
    }



    public RelayCommand<string> NumpadCommand { get; }
    public RelayCommand ClearNumpadCommand { get; }
    public RelayCommand BackspaceCommand { get; }
    public AsyncRelayCommand<string> ProcessPaymentCommand { get; }
    public AsyncRelayCommand ToggleTaxExemptCommand { get; }
    public RelayCommand CloseCommand { get; }
    public RelayCommand NextAmountCommand { get; }
    public AsyncRelayCommand NoSaleCommand { get; }
    public AsyncRelayCommand LogoutUiCommand { get; }
    public RelayCommand ExactAmountCommand { get; }
    public RelayCommand<string> QuickCashCommand { get; }
    public AsyncRelayCommand ShowGratuityDialogCommand { get; }
    public AsyncRelayCommand VoidTicketCommand { get; }
    public AsyncRelayCommand ReprintReceiptCommand { get; }

    private async Task OnLogoutAsync()
    {
        try
        {
            // F-0008: Logout Action
            if (_userService.CurrentUser == null) return;
            var userId = _userService.CurrentUser.Id; 
            await _logoutHandler.HandleAsync(new LogoutCommand { UserId = userId });
            _navigationService.Navigate(typeof(Magidesk.Presentation.Views.LoginPage));
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }

    private void OnNextAmount()
    {
        if (DueAmount <= 0) return;
        
        // Logic: Round up to next dollar. If 1.00, stays 1.00? Usually "Next Amount" implies next whole dollar if fractional, or next denomination.
        // Forensic Audit says: "rounds due up to next integer (Math.ceil(dd))".
        var nextAmt = Math.Ceiling(DueAmount);
        if (nextAmt == DueAmount) 
        {
            // If already exact integer, maybe add 1? Or just keep it. 
            // Floreant behavior: ceil. So 5.00 -> 5.00. 5.01 -> 6.00.
        }
        
        // Update Numpad Input to reflect this (so user sees it)
        NumpadInput = nextAmt.ToString("F2");
        TenderAmount = nextAmt;
    }

    private void OnExactAmount()
    {
        // F-0042: Exact Due Button - sets tender amount to exact due amount
        if (DueAmount <= 0) return;
        
        // Check for stale amounts by refreshing ticket data
        // For now, use current DueAmount (backend precision requirement handled by existing ticket refresh)
        
        // Set tender amount to exact due amount
        TenderAmount = DueAmount;
        NumpadInput = DueAmount.ToString("F2");
    }

    private void OnQuickCash(string? amountStr)
    {
        // F-0043: Quick Cash Buttons - adds denomination to tender amount
        if (decimal.TryParse(amountStr, out var amount))
        {
            if (amount <= 0) return;
            
            // Cumulative mode: add to existing tender amount
            var newTenderAmount = TenderAmount + amount;
            TenderAmount = newTenderAmount;
            NumpadInput = newTenderAmount.ToString("F2");
        }
    }

    private async Task OnNoSaleAsync()
    {
        // Manager Authorization Required for Open Drawer (No Sale)
        var authDialog = App.Services.GetRequiredService<Views.Dialogs.ManagerPinDialog>();
        authDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        
        var authResult = await authDialog.ShowForOperationAsync("Open Drawer");
        if (authResult == null || !authResult.Authorized)
        {
            return; 
        }

        // F-0007: NO SALE behavior (drawer kick).
        // Should trigger drawer kick command (if hardware connected) and log audit.
        IsBusy = true;
        try
        {
            await _openCashDrawer.HandleAsync(new OpenCashDrawerCommand());
            StatusMessage = "Drawer Opened (No Sale)";
        }
        catch (Exception ex) { Error = ex.Message; }
        finally { IsBusy = false; }
    }

    public async Task InitializeAsync(Guid ticketId)
    {
        _ticketId = ticketId;
        await LoadTicketAsync();
    }

    private async Task LoadTicketAsync()
    {
        IsBusy = true;
        Error = null;
        try
        {
            // Create a fresh scope to ensure we get the latest data from DB
            // Bypasses any stale tracking in the long-lived ViewModel scope
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var getTicketHandler = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetTicketQuery, TicketDto?>>();
                Ticket = await getTicketHandler.HandleAsync(new GetTicketQuery { TicketId = _ticketId });
                
                if (Ticket == null)
                {
                    Error = "Ticket not found.";
                }
            }
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnNumpadInput(string? value)
    {
        if (value == null) return;
        
        if (value == ".")
        {
            if (!NumpadInput.Contains("."))
            {
                NumpadInput += ".";
            }
        }
        else
        {
            NumpadInput += value;
        }
    }

    private void OnClearNumpad()
    {
        NumpadInput = "";
        TenderAmount = 0;
    }

    private void OnBackspace()
    {
        if (NumpadInput.Length > 0)
        {
            NumpadInput = NumpadInput[..^1];
        }
    }

    private async Task ToggleTaxExemptAsync()
    {
        if (Ticket == null) return;
        if (_userService.CurrentUser == null) { Error = "No user logged in."; return; }

        IsBusy = true;
        Error = null;
        try
        {
            var result = await _setTaxExempt.HandleAsync(new SetTaxExemptCommand
            {
                TicketId = Ticket.Id,
                IsTaxExempt = !Ticket.IsTaxExempt,
                ModifiedBy = new UserId(_userService.CurrentUser.Id)
            });

            if (result.Success)
            {
                await LoadTicketAsync();
            }
            else
            {
                Error = result.Error;
            }
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ProcessPaymentAsync(string? paymentTypeString)
    {
        if (Ticket == null || string.IsNullOrEmpty(paymentTypeString)) return;
        
        if (!Enum.TryParse<Magidesk.Domain.Enumerations.PaymentType>(paymentTypeString, out var paymentType))
        {
            Error = $"Invalid payment type: {paymentTypeString}";
            return;
        }

        // Manager Authorization Required for Settle/Payment Operations
        var authDialog = App.Services.GetRequiredService<Views.Dialogs.ManagerPinDialog>();
        authDialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        
        var authResult = await authDialog.ShowForOperationAsync("Settle Ticket");
        if (authResult == null || !authResult.Authorized)
        {
            // Authorization failed or cancelled
            return;
        }
        
        IsBusy = true;
        Error = null;
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var processPaymentHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult>>();
                var cashSessionRepository = scope.ServiceProvider.GetRequiredService<ICashSessionRepository>();

                // For card payments in Phase 4 simulation, we don't need Exact amount
                // But for Cash, we use the Tender Amount
                
                var amountToPay = TenderAmount > 0 && TenderAmount < DueAmount 
                    ? TenderAmount 
                    : DueAmount; // If tender >= due, we pay due. If tender 0 (auto), pay due. 
                
                if (TenderAmount > 0 && TenderAmount >= DueAmount)
                {
                    amountToPay = DueAmount;
                }

                // If user typed 0 or nothing, assume they want to pay Full Due
                if (TenderAmount <= 0)
                {
                    amountToPay = DueAmount;
                    TenderAmount = DueAmount; // Treat as exact change
                }
                
                // DTO amounts are pure decimals, so we use default currency (likely USD)
                var currency = "USD";

                if (_userService.CurrentUser == null || _terminalContext.TerminalId == null)
                {
                    Error = "User or Terminal context missing.";
                    return;
                }

                var userId = _userService.CurrentUser.Id;
                var terminalId = _terminalContext.TerminalId.Value;

                var command = new ProcessPaymentCommand
                {
                    TicketId = Ticket.Id,
                    PaymentType = paymentType,
                    Amount = new Money(amountToPay, currency),
                    ProcessedBy = new UserId(userId),
                    TerminalId = terminalId,
                    GlobalId = Guid.NewGuid().ToString()
                };

                if (paymentType == PaymentType.Cash)
                {
                     command.TenderAmount = new Money(TenderAmount, currency);
                     
                     // Resolve Cash Session from FRESH scope repository
                     var session = await cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
                     if (session != null)
                     {
                         command.CashSessionId = session.Id;
                     }
                     else
                     {
                         Error = "No active cash session.";
                         return;
                     }
                }
                else if (paymentType == PaymentType.CreditCard)
                {
                    // Simulate Card Data
                    command.Last4 = "1234";
                    command.CardType = "Visa";
                    command.AuthCode = "AUTH123";
                }
                else if (paymentType == PaymentType.GiftCertificate)
                {
                    // Simulate Gift Card
                    command.GiftCardNumber = "GC-SIM-123";
                }

                var result = await processPaymentHandler.HandleAsync(command);
                
                if (result.TicketIsPaid)
                {
                    // Close and Navigate back
                    OnClose();
                }
                else
                {
                    // Reload to show remaining balance (LoadTicketAsync will need fresh scope too preferably, but minimal fix first)
                    await LoadTicketAsync();
                    
                    // Clear numpad for next payment
                    OnClearNumpad();
                }
                
                if (paymentType == PaymentType.Cash && result.ChangeAmount.Amount > 0)
                {
                     // Show Change Dialog (Blocking)
                     var changeDialog = new Microsoft.UI.Xaml.Controls.ContentDialog
                     {
                         Title = "Change Due",
                         Content = $"Please return change to customer:\n\n{result.ChangeAmount}",
                         CloseButtonText = "OK",
                         XamlRoot = App.MainWindowInstance.Content.XamlRoot
                     };
                     await _navigationService.ShowDialogAsync(changeDialog);
                     
                     Error = $"Change Due: {result.ChangeAmount}"; 
                }
            }
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            
            // T-004: Anti-Silence. Show blocking dialog on financial failure.
             var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
             {
                 Title = "Payment Failed",
                 Content = $"Transaction could not be processed.\nReason: {ex.Message}",
                 CloseButtonText = "OK",
                 XamlRoot = App.MainWindowInstance.Content.XamlRoot
             };
             await _navigationService.ShowDialogAsync(dialog);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnClose()
    {
        if (Ticket != null && (Ticket.Status == TicketStatus.Paid || Ticket.Status == TicketStatus.Closed))
        {
            // F-0044: Settlement Navigation - Return to Switchboard/TableMap after successful payment
            _navigationService.Navigate(typeof(Magidesk.Presentation.Views.SwitchboardPage));
        }
        else
        {
            _navigationService.GoBack();
        }
    }

    public AsyncRelayCommand TestWaitCommand { get; }
    public AsyncRelayCommand SwipeCardCommand { get; }

    private async Task TestWaitAsync()
    {
        var dialog = new Magidesk.Views.PaymentProcessWaitDialog();
        dialog.ViewModel.SetMessage("Testing Wait Dialog (3s)...");
        
        // TICKET-014: Track background task instead of fire-and-forget
        var closeTask = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(3000);
                // Must dispatch to UI thread to close
                _navigationService.DispatcherQueue.TryEnqueue(() =>
                {
                    dialog.ViewModel.AllowClose();
                    dialog.Hide(); // ContentDialog.Hide() closes it
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TestWaitAsync background task failed");
                // Surface error to UI
                _navigationService.DispatcherQueue.TryEnqueue(async () =>
                {
                    await _navigationService.ShowErrorAsync("Test Wait Failed", 
                        $"Background task error: {ex.Message}");
                });
            }
        });

        // Show dialog and await both tasks
        await Task.WhenAll(
            _navigationService.ShowDialogAsync(dialog),
            closeTask
        );
    }

    private async Task SwipeCardAsync()
    {
        if (Ticket == null) return;
        
        var dialog = new Magidesk.Views.SwipeCardDialog();
        var result = await _navigationService.ShowDialogAsync(dialog);

        // result is ContentDialogResult which is an enum (None, Primary, Secondary)
        // Primary = Manual Entry
        // Secondary = Auth Code
        // None = Cancel or Enter key (if we hide)

        if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
        {
            // Manual Entry Requested - show Authorization Code Dialog
            var authDialog = new Magidesk.Views.AuthorizationCodeDialog();
            var authResult = await _navigationService.ShowDialogAsync(authDialog);

            if (authResult == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                // Proceed with Auth Code
                var authCode = authDialog.ViewModel.AuthCode;
                var cardType = authDialog.ViewModel.SelectedCardType;
                
                if (!string.IsNullOrWhiteSpace(authCode))
                {
                    // Process credit card payment with manual auth
                    await ProcessCreditCardPaymentAsync(cardType, authCode, "Manual Voice Auth");
                }
            }
        }
        else if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Secondary)
        {
            // Auth Code Requested - show Authorization Code Dialog
            var authDialog = new Magidesk.Views.AuthorizationCodeDialog();
            var authResult = await _navigationService.ShowDialogAsync(authDialog);

            if (authResult == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                // Proceed with Auth Code
                var authCode = authDialog.ViewModel.AuthCode;
                var cardType = authDialog.ViewModel.SelectedCardType;
                
                if (!string.IsNullOrWhiteSpace(authCode))
                {
                    // Process credit card payment with auth code
                    await ProcessCreditCardPaymentAsync(cardType, authCode, "Phone Auth");
                }
            }
        }
        else
        {
            // Check for swipe data (None result could be from successful swipe)
            if (!string.IsNullOrEmpty(dialog.ViewModel.SwipeData))
            {
                // Process swipe data - simulate successful card swipe
                await ProcessCreditCardPaymentAsync("Visa", "SWIPE" + DateTime.Now.ToString("yyyyMMddHHmmss"), "Card Swipe");
            }
        }
    }

    private async Task ProcessCreditCardPaymentAsync(string cardType, string authCode, string authMethod)
    {
        IsBusy = true;
        Error = null;
        
        try
        {
            // Show processing wait dialog
            var waitDialog = new Magidesk.Views.PaymentProcessWaitDialog();
            waitDialog.ViewModel.Message = $"Processing {cardType} payment...";
            
            // Start showing dialog (Task)
            var showDialogTask = _navigationService.ShowDialogAsync(waitDialog);
            
            // Simulate processing delay (Background)
            await Task.Delay(2000);
            
            // Close dialog (UI Thread)
            _navigationService.DispatcherQueue.TryEnqueue(() =>
            {
                waitDialog.ViewModel.AllowClose();
                waitDialog.Hide();
            });

            // Ensure dialog matches close state
            await showDialogTask;
            
            var success = true; // Simulated success
            
            if (success)
            {
                // Process the credit card payment using existing ProcessPaymentAsync
                // We need to pass the card details to simulation state or handling logic before calling
                // For now, we assume simple card payment trigger
                await ProcessPaymentAsync("CreditCard");
                
                // Store card details for audit (in a real app, this would be handled securely)
                Error = $"Card Payment Approved: {cardType} ({authMethod})";
            }
            else
            {
                Error = "Card Payment Declined";
            }
        }
        catch (Exception ex)
        {
            Error = $"Card Payment Error: {ex.Message}";
            
            var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
            {
                Title = "Card Payment Error",
                Content = $"Card transaction failed.\nReason: {ex.Message}",
                CloseButtonText = "OK",
                XamlRoot = App.MainWindowInstance.Content.XamlRoot
            };
            await _navigationService.ShowDialogAsync(dialog);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ShowGratuityDialogAsync()
    {
        if (Ticket == null)
        {
            Error = "No ticket selected.";
            return;
        }

        if (_userService.CurrentUser == null)
        {
            Error = "No user logged in.";
            return;
        }

        try
        {
            IsBusy = true;

            // Create a fresh scope for the gratuity operation to ensure fresh DbContext
            // This prevents ConcurrencyException due to stale tracked entities
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var getUsersQuery = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetUsersQuery, IEnumerable<UserDto>>>();
                var applyGratuityHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<ApplyGratuityCommand, ApplyGratuityResult>>();

                // Get available servers/staff
                var users = await getUsersQuery.HandleAsync(new GetUsersQuery());
                var serverItems = new System.Collections.ObjectModel.ObservableCollection<ServerItem>(
                    users.Select(u => new ServerItem(
                        new UserId(u.Id),
                        $"{u.FirstName} {u.LastName}",
                        u.RoleName
                    ))
                );

                // Create ViewModel with FRESH handler
                var viewModel = new GratuitySelectionViewModel(
                    _gratuityService,
                    applyGratuityHandler,
                    App.Services.GetRequiredService<IDialogService>(),
                    App.Services.GetRequiredService<ILogger<GratuitySelectionViewModel>>(),
                    Ticket.Id,
                    Ticket.TicketNumber.ToString(),
                    new Money(Ticket.SubtotalAmount),
                    new UserId(_userService.CurrentUser.Id),
                    serverItems
                );

                // Create and show dialog
                var dialog = new GratuitySelectionDialog(viewModel);
                dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;

                var result = await _navigationService.ShowDialogAsync(dialog);

                if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
                {
                    // Gratuity applied successfully - reload ticket
                    await LoadTicketAsync();
                    StatusMessage = "Gratuity added successfully.";
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing gratuity dialog for ticket {TicketId}", Ticket.Id);
            Error = $"Failed to show gratuity dialog: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OnVoidTicketAsync()
    {
        if (Ticket == null) return;
        
        try
        {
            var dialog = new Magidesk.Presentation.Views.VoidTicketDialog();
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            dialog.ViewModel.Initialize(Ticket);
            
            await dialog.ShowAsync();
            
            // Reload ticket to check status
            await LoadTicketAsync();
            
            if (Ticket != null && Ticket.Status == Domain.Enumerations.TicketStatus.Voided)
            {
                // Navigate away if voided
                 StatusMessage = "Ticket Voided.";
                 await Task.Delay(1000); // Brief delay to show message
                 OnClose();
            }
        }
        catch (Exception ex)
        {
             Error = ex.Message;
        }
    }

    private async Task OnReprintReceiptAsync()
    {
        if (Ticket == null) return;
        
        IsBusy = true;
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var printHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<PrintReceiptCommand, PrintReceiptResult>>();
                
                var result = await printHandler.HandleAsync(new PrintReceiptCommand
                {
                    TicketId = Ticket.Id,
                    ReceiptType = ReceiptType.Ticket
                });
                
                if (result.Success)
                {
                    StatusMessage = "Receipt Sent to Printer.";
                }
                else
                {
                    Error = "Failed to print receipt.";
                }
            }
        }
        catch (Exception ex)
        {
             Error = $"Print Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}

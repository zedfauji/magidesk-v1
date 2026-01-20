using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Magidesk.ViewModels;
using Magidesk.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// ViewModel for the redesigned Settle Page.
/// Manages payment settlement workflow with modern UI patterns.
/// </summary>
public partial class SettlePageViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetTicketQuery, TicketDto?> _getTicketHandler;
    private readonly ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult> _processPaymentHandler;
    private readonly ICommandHandler<SetTaxExemptCommand, SetTaxExemptResult> _setTaxExemptHandler;
    private readonly NavigationService _navigationService;
    private readonly IUserService _userService;
    private readonly ITerminalContext _terminalContext;
    private readonly ICashSessionRepository _cashSessionRepository;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IDialogService _dialogService;
    private readonly ILogger<SettlePageViewModel> _logger;

    private Guid _ticketId;
    private TicketDto? _ticket;
    private decimal _tenderAmount;
    private string _tenderAmountInput = ""; // Raw input string without formatting
    private DateTime _lastKeypadPress = DateTime.MinValue;
    private const int KEYPAD_DEBOUNCE_MS = 200; // Debounce time in milliseconds
    private Microsoft.UI.Xaml.XamlRoot? _xamlRoot; // Store XamlRoot for dialogs

    public SettlePageViewModel(
        IQueryHandler<GetTicketQuery, TicketDto?> getTicketHandler,
        ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult> processPaymentHandler,
        ICommandHandler<SetTaxExemptCommand, SetTaxExemptResult> setTaxExemptHandler,
        NavigationService navigationService,
        IUserService userService,
        ITerminalContext terminalContext,
        ICashSessionRepository cashSessionRepository,
        IServiceScopeFactory serviceScopeFactory,
        IDialogService dialogService,
        ILogger<SettlePageViewModel> logger)
    {
        _getTicketHandler = getTicketHandler ?? throw new ArgumentNullException(nameof(getTicketHandler));
        _processPaymentHandler = processPaymentHandler ?? throw new ArgumentNullException(nameof(processPaymentHandler));
        _setTaxExemptHandler = setTaxExemptHandler ?? throw new ArgumentNullException(nameof(setTaxExemptHandler));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _terminalContext = terminalContext ?? throw new ArgumentNullException(nameof(terminalContext));
        _cashSessionRepository = cashSessionRepository ?? throw new ArgumentNullException(nameof(cashSessionRepository));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Initialize collections
        PaymentMethods = new ObservableCollection<PaymentMethodViewModel>
        {
            new PaymentMethodViewModel(PaymentType.Cash, "CASH", "payments", "#107C10"),
            new PaymentMethodViewModel(PaymentType.CreditCard, "CREDIT CARD", "credit_card", "#0078D4"),
            new PaymentMethodViewModel(PaymentType.GiftCertificate, "GIFT CARD", "card_giftcard", "#8E44AD")
        };

        QuickCashAmounts = new ObservableCollection<decimal> { 1, 5, 10, 20, 50, 100 };

        // Initialize commands
        KeypadDigitCommand = new RelayCommand<string>(OnKeypadDigit);
        ClearTenderCommand = new RelayCommand(OnClearTender);
        QuickCashCommand = new RelayCommand<decimal>(OnQuickCash);
        ProcessPaymentCommand = new AsyncRelayCommand<PaymentType>(ProcessPaymentAsync);
        AddTipCommand = new AsyncRelayCommand(OnAddTipAsync);
        HoldTicketCommand = new AsyncRelayCommand(OnHoldTicketAsync);
        SplitPaymentCommand = new AsyncRelayCommand(OnSplitPaymentAsync);
        ApplyDiscountCommand = new AsyncRelayCommand(OnApplyDiscountAsync);
        PrintReceiptCommand = new AsyncRelayCommand(OnPrintReceiptAsync);
        ToggleTaxExemptCommand = new AsyncRelayCommand(OnToggleTaxExemptAsync);
        CancelSettlementCommand = new RelayCommand(OnCancelSettlement);
        NavigateBackCommand = new RelayCommand(OnNavigateBack);
        
        _logger.LogInformation("SettlePageViewModel constructor - All commands initialized");
    }

    #region Properties

    // Ticket Information
    public string TicketNumber => _ticket != null ? $"Ticket #{_ticket.TicketNumber}" : "No Ticket";
    public string TableNumber => _ticket?.TableName ?? "No Table";

    // Financial Summary
    public decimal TotalAmount => _ticket?.TotalAmount ?? 0m;
    public decimal TaxAmount => _ticket?.TaxAmount ?? 0m;

    [ObservableProperty]
    private decimal _paidAmount;

    [ObservableProperty]
    private decimal _balanceDue;

    // Tender Entry
    [ObservableProperty]
    private string _tenderAmountDisplay = "$0.00";

    // Payment Methods
    public ObservableCollection<PaymentMethodViewModel> PaymentMethods { get; }

    // Quick Cash Amounts
    public ObservableCollection<decimal> QuickCashAmounts { get; }

    // State Properties
    [ObservableProperty]
    private bool _isTaxExempt;

    [ObservableProperty]
    private bool _isProcessingPayment;

    #endregion

    #region Commands

    public RelayCommand<string> KeypadDigitCommand { get; }
    public RelayCommand ClearTenderCommand { get; }
    public RelayCommand<decimal> QuickCashCommand { get; }
    public AsyncRelayCommand<PaymentType> ProcessPaymentCommand { get; }
    public AsyncRelayCommand AddTipCommand { get; }
    public AsyncRelayCommand HoldTicketCommand { get; }
    public AsyncRelayCommand SplitPaymentCommand { get; }
    public AsyncRelayCommand ApplyDiscountCommand { get; }
    public AsyncRelayCommand PrintReceiptCommand { get; }
    public AsyncRelayCommand ToggleTaxExemptCommand { get; }
    public RelayCommand CancelSettlementCommand { get; }
    public RelayCommand NavigateBackCommand { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the ViewModel with a ticket ID.
    /// </summary>
    public async Task InitializeAsync(Guid ticketId)
    {
        _ticketId = ticketId;
        await LoadTicketAsync();
    }
    
    /// <summary>
    /// Sets the XamlRoot for dialogs. Must be called from the View after it's loaded.
    /// </summary>
    public void SetXamlRoot(Microsoft.UI.Xaml.XamlRoot xamlRoot)
    {
        _xamlRoot = xamlRoot;
    }

    #endregion

    #region Private Methods

    private async Task LoadTicketAsync()
    {
        try
        {
            IsBusy = true;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var getTicketHandler = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetTicketQuery, TicketDto?>>();
                _ticket = await getTicketHandler.HandleAsync(new GetTicketQuery { TicketId = _ticketId });

                if (_ticket != null)
                {
                    PaidAmount = _ticket.PaidAmount;
                    BalanceDue = _ticket.DueAmount;
                    IsTaxExempt = _ticket.IsTaxExempt;

                    // Notify property changes
                    OnPropertyChanged(nameof(TicketNumber));
                    OnPropertyChanged(nameof(TableNumber));
                    OnPropertyChanged(nameof(TotalAmount));
                    OnPropertyChanged(nameof(TaxAmount));

                    _logger.LogInformation("Loaded ticket {TicketId} with balance due {BalanceDue}", _ticketId, BalanceDue);
                }
                else
                {
                    _logger.LogWarning("Ticket {TicketId} not found", _ticketId);
                    await _dialogService.ShowErrorAsync(
                        "Ticket Not Found",
                        $"Ticket {_ticketId} could not be found. It may have been deleted or moved.");
                }
            }
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while loading ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync(
                "Network Error",
                "Unable to connect to the server. Please check your network connection and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync(
                "Error Loading Ticket",
                $"An error occurred while loading the ticket:\n\n{ex.Message}",
                ex.ToString());
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnKeypadDigit(string? digit)
    {
        // Debounce to prevent double-triggering from WinUI button template
        var now = DateTime.Now;
        if ((now - _lastKeypadPress).TotalMilliseconds < KEYPAD_DEBOUNCE_MS)
        {
            System.Diagnostics.Debug.WriteLine($"OnKeypadDigit: Debounced duplicate call for digit {digit}");
            return;
        }
        _lastKeypadPress = now;

        System.Diagnostics.Debug.WriteLine($"OnKeypadDigit called with digit: {digit}");
        _logger.LogInformation("OnKeypadDigit called with digit: {Digit}", digit);
        
        if (string.IsNullOrEmpty(digit))
        {
            System.Diagnostics.Debug.WriteLine("OnKeypadDigit: digit is null or empty, returning");
            return;
        }

        // Handle decimal point
        if (digit == ".")
        {
            // Only allow one decimal point
            if (!_tenderAmountInput.Contains("."))
            {
                // If input is empty, start with "0."
                if (string.IsNullOrEmpty(_tenderAmountInput))
                {
                    _tenderAmountInput = "0.";
                }
                else
                {
                    _tenderAmountInput += ".";
                }
                TenderAmountDisplay = "$" + _tenderAmountInput;
                System.Diagnostics.Debug.WriteLine($"OnKeypadDigit: Added decimal point. Input='{_tenderAmountInput}', Display='{TenderAmountDisplay}'");
            }
            return;
        }

        // Handle digits 0-9
        if (digit.Length == 1 && char.IsDigit(digit[0]))
        {
            // Append digit to raw input
            _tenderAmountInput += digit;

            // Try to parse and format
            if (decimal.TryParse(_tenderAmountInput, out var amount))
            {
                _tenderAmount = amount;
                
                // If there's a decimal point in the input, show it as-is with $ prefix
                if (_tenderAmountInput.Contains("."))
                {
                    TenderAmountDisplay = "$" + _tenderAmountInput;
                }
                else
                {
                    // No decimal point yet, format as currency
                    TenderAmountDisplay = FormatCurrency(amount);
                }
            }
            else
            {
                // Keep building the string
                TenderAmountDisplay = "$" + _tenderAmountInput;
            }
            
            System.Diagnostics.Debug.WriteLine($"OnKeypadDigit: Input='{_tenderAmountInput}', Display='{TenderAmountDisplay}', Amount={_tenderAmount}");
        }
    }

    private string FormatCurrency(decimal amount)
    {
        return amount.ToString("C2");
    }

    private void OnClearTender()
    {
        _tenderAmount = 0m;
        _tenderAmountInput = "";
        TenderAmountDisplay = "$0.00";
        
        _logger.LogDebug("Tender amount cleared");
    }

    private void OnQuickCash(decimal amount)
    {
        System.Diagnostics.Debug.WriteLine($"OnQuickCash called with amount: {amount}");
        _logger.LogInformation("OnQuickCash called with amount: {Amount}", amount);
        
        if (amount <= 0)
        {
            System.Diagnostics.Debug.WriteLine("OnQuickCash: amount is zero or negative, returning");
            return;
        }

        _tenderAmount = amount;
        _tenderAmountInput = amount.ToString("F2"); // Store as "20.00" format
        TenderAmountDisplay = FormatCurrency(amount);
        
        _logger.LogDebug("Quick cash amount set to {Amount}", amount);
    }

    private async Task ProcessPaymentAsync(PaymentType paymentType)
    {
        if (_ticket == null)
        {
            _logger.LogWarning("Cannot process payment: no ticket loaded");
            await _dialogService.ShowErrorAsync(
                "Payment Error",
                "No ticket is currently loaded. Please return to the order page and try again.");
            return;
        }

        if (_tenderAmount <= 0)
        {
            _logger.LogWarning("Cannot process payment: tender amount is zero or negative");
            await _dialogService.ShowWarningAsync(
                "Invalid Amount",
                "Please enter a tender amount greater than zero.");
            return;
        }

        if (_userService.CurrentUser == null)
        {
            _logger.LogError("Cannot process payment: no user logged in");
            await _dialogService.ShowErrorAsync(
                "Authentication Error",
                "No user is currently logged in. Please log in and try again.");
            return;
        }

        if (_terminalContext.TerminalId == null)
        {
            _logger.LogError("Cannot process payment: no terminal context");
            await _dialogService.ShowErrorAsync(
                "Terminal Error",
                "Terminal context is not available. Please restart the application.");
            return;
        }

        try
        {
            IsProcessingPayment = true;
            IsBusy = true;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var processPaymentHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult>>();
                var cashSessionRepository = scope.ServiceProvider.GetRequiredService<ICashSessionRepository>();

                var userId = _userService.CurrentUser.Id;
                var terminalId = _terminalContext.TerminalId.Value;
                var currency = "USD"; // Default currency

                // Determine amount to pay (handle partial payments)
                var amountToPay = _tenderAmount >= BalanceDue ? BalanceDue : _tenderAmount;

                var command = new ProcessPaymentCommand
                {
                    TicketId = _ticket.Id,
                    PaymentType = paymentType,
                    Amount = new Money(amountToPay, currency),
                    ProcessedBy = new UserId(userId),
                    TerminalId = terminalId,
                    GlobalId = Guid.NewGuid().ToString()
                };

                // Handle cash-specific logic
                if (paymentType == PaymentType.Cash)
                {
                    command.TenderAmount = new Money(_tenderAmount, currency);

                    // Get active cash session
                    var session = await cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
                    if (session != null)
                    {
                        command.CashSessionId = session.Id;
                    }
                    else
                    {
                        _logger.LogError("No active cash session for terminal {TerminalId}", terminalId);
                        await _dialogService.ShowErrorAsync(
                            "Session Error",
                            "No active cash session found for this terminal. Please start a cash session before processing cash payments.");
                        return;
                    }
                }
                else if (paymentType == PaymentType.CreditCard)
                {
                    // Simulate card data
                    command.Last4 = "1234";
                    command.CardType = "Visa";
                    command.AuthCode = "AUTH" + DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                else if (paymentType == PaymentType.GiftCertificate)
                {
                    // Simulate gift card
                    command.GiftCardNumber = "GC-" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
                }

                var result = await processPaymentHandler.HandleAsync(command);

                _logger.LogInformation("Payment processed: {PaymentId}, Change: {Change}, Ticket Paid: {IsPaid}",
                    result.PaymentId, result.ChangeAmount, result.TicketIsPaid);

                // Handle overpayment (change due)
                if (paymentType == PaymentType.Cash && result.ChangeAmount.Amount > 0)
                {
                    _logger.LogInformation("Change due: {Change}", result.ChangeAmount);
                    
                    // Show change dialog
                    await _dialogService.ShowMessageAsync(
                        "Change Due",
                        $"Change: {result.ChangeAmount.Amount:C2}\n\nPlease give the customer their change.");
                }

                // Reload ticket to get updated balances
                await LoadTicketAsync();

                // Clear tender for next payment if ticket not fully paid
                if (!result.TicketIsPaid)
                {
                    OnClearTender();
                }
                else
                {
                    // Ticket is fully paid - show confirmation and navigate back
                    _logger.LogInformation("Ticket {TicketId} is fully paid", _ticketId);
                    
                    await _dialogService.ShowMessageAsync(
                        "Payment Complete",
                        $"Ticket #{_ticket.TicketNumber} has been paid in full.\n\nTotal: {_ticket.TotalAmount:C2}");
                    
                    // Navigate back to main page
                    _navigationService.GoBack();
                }
            }
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            // Network connectivity error
            _logger.LogError(ex, "Network error while processing payment for ticket {TicketId}", _ticketId);
            
            var retry = await _dialogService.ShowConfirmationAsync(
                "Network Error",
                "Unable to connect to the payment server. The payment has not been processed.\n\nWould you like to retry?",
                "Retry", "Cancel");
            
            if (retry)
            {
                // Retry the payment
                await ProcessPaymentAsync(paymentType);
            }
        }
        catch (TimeoutException ex)
        {
            // Timeout error
            _logger.LogError(ex, "Timeout while processing payment for ticket {TicketId}", _ticketId);
            
            var retry = await _dialogService.ShowConfirmationAsync(
                "Timeout Error",
                "The payment request timed out. The payment may or may not have been processed.\n\nPlease verify the payment status before retrying.",
                "Retry", "Cancel");
            
            if (retry)
            {
                // Retry the payment
                await ProcessPaymentAsync(paymentType);
            }
        }
        catch (InvalidOperationException ex)
        {
            // Business logic error (e.g., invalid state)
            _logger.LogError(ex, "Invalid operation while processing payment for ticket {TicketId}", _ticketId);
            
            await _dialogService.ShowErrorAsync(
                "Payment Error",
                $"Unable to process payment: {ex.Message}\n\nPlease check the ticket status and try again.",
                ex.ToString());
        }
        catch (Exception ex)
        {
            // General error
            _logger.LogError(ex, "Failed to process payment for ticket {TicketId}", _ticketId);
            
            var retry = await _dialogService.ShowConfirmationAsync(
                "Payment Error",
                $"An error occurred while processing the payment:\n\n{ex.Message}\n\nWould you like to retry?",
                "Retry", "Cancel");
            
            if (retry)
            {
                // Retry the payment
                await ProcessPaymentAsync(paymentType);
            }
        }
        finally
        {
            IsProcessingPayment = false;
            IsBusy = false;
        }
    }

    private async Task OnAddTipAsync()
    {
        if (_ticket == null)
        {
            _logger.LogWarning("Cannot add tip: no ticket loaded");
            return;
        }

        try
        {
            _logger.LogInformation("Add tip requested for ticket {TicketId}", _ticketId);
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var gratuityService = scope.ServiceProvider.GetRequiredService<IGratuityService>();
                var applyGratuityHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<ApplyGratuityCommand, ApplyGratuityResult>>();
                var dialogService = scope.ServiceProvider.GetRequiredService<IDialogService>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<GratuitySelectionViewModel>>();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                
                // Get available servers (current user and ticket creator)
                var availableServers = new ObservableCollection<ServerItem>();
                
                // Add current user
                if (_userService.CurrentUser != null)
                {
                    availableServers.Add(new ServerItem(
                        new UserId(_userService.CurrentUser.Id),
                        _userService.CurrentUser.FullName));
                }
                
                // Add ticket creator if different
                if (_ticket.CreatedBy != _userService.CurrentUser?.Id)
                {
                    var creator = await userRepository.GetByIdAsync(_ticket.CreatedBy);
                    if (creator != null)
                    {
                        availableServers.Add(new ServerItem(
                            new UserId(creator.Id),
                            $"{creator.FirstName} {creator.LastName}"));
                    }
                }
                
                // Create ViewModel for gratuity selection dialog
                var viewModel = new GratuitySelectionViewModel(
                    gratuityService,
                    applyGratuityHandler,
                    dialogService,
                    logger,
                    _ticket.Id,
                    $"#{_ticket.TicketNumber}",
                    new Money(_ticket.SubtotalAmount, "USD"),
                    new UserId(_userService.CurrentUser!.Id),
                    availableServers);
                
                // Create Dialog
                var dialog = new GratuitySelectionDialog(viewModel);
                
                // Use NavigationService to show dialog (handles XamlRoot automatically)
                await _navigationService.ShowDialogAsync(dialog);

                // Reload ticket to get updated totals (gratuity is applied within the dialog)
                await LoadTicketAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add tip to ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to add tip: {ex.Message}");
        }
    }

    private async Task OnHoldTicketAsync()
    {
        if (_ticket == null)
        {
            _logger.LogWarning("Cannot hold ticket: no ticket loaded");
            return;
        }

        try
        {
            // Confirm with user before holding
            var confirmed = await _dialogService.ShowConfirmationAsync(
                "Hold Ticket",
                $"Hold ticket #{_ticket.TicketNumber}?\n\nYou can resume this ticket later from the held tickets list.",
                "Hold", "Cancel");
            
            if (confirmed)
            {
                _logger.LogInformation("Holding ticket {TicketId}", _ticketId);
                _navigationService.GoBack();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to hold ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", "Failed to hold ticket.", ex.Message);
        }
    }

    private async Task OnSplitPaymentAsync()
    {
        if (_ticket == null)
        {
            _logger.LogWarning("Cannot split payment: no ticket loaded");
            return;
        }

        try
        {
            _logger.LogInformation("Split payment requested for ticket {TicketId}", _ticketId);
            
            // TODO: Implement split payment dialog integration
            // The SplitPaymentViewModel requires proper initialization and dialog workflow
            await _dialogService.ShowMessageAsync(
                "Split Payment",
                "Split payment feature is available but requires dialog integration.\n\nPlease use the command handler directly or implement the full dialog workflow.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process split payment for ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to process split payment: {ex.Message}");
        }
    }

    private async Task OnApplyDiscountAsync()
    {
        if (_ticket == null)
        {
            _logger.LogWarning("Cannot apply discount: no ticket loaded");
            return;
        }

        try
        {
            _logger.LogInformation("Apply discount requested for ticket {TicketId}", _ticketId);
            
            // TODO: Implement discount selection dialog integration
            // The DiscountSelectionViewModel requires proper initialization and manager authorization workflow
            await _dialogService.ShowMessageAsync(
                "Apply Discount",
                "Discount selection feature is available but requires dialog integration.\n\nPlease use the command handler directly or implement the full dialog workflow.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply discount to ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to apply discount: {ex.Message}");
        }
    }

    private async Task OnPrintReceiptAsync()
    {
        if (_ticket == null)
        {
            _logger.LogWarning("Cannot print receipt: no ticket loaded");
            return;
        }

        try
        {
            _logger.LogInformation("Print receipt requested for ticket {TicketId}", _ticketId);
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var printReceiptHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<PrintReceiptCommand, PrintReceiptResult>>();
                
                var command = new PrintReceiptCommand
                {
                    TicketId = _ticketId
                };
                
                var result = await printReceiptHandler.HandleAsync(command);
                
                if (result.Success)
                {
                    _logger.LogInformation("Receipt printed for ticket {TicketId}", _ticketId);
                    
                    await _dialogService.ShowMessageAsync(
                        "Receipt Printed",
                        $"Receipt has been printed.\n\nTicket #{_ticket.TicketNumber}\nTotal: {_ticket.TotalAmount:C2}");
                }
                else
                {
                    _logger.LogError("Failed to print receipt for ticket {TicketId}", _ticketId);
                    await _dialogService.ShowErrorAsync(
                        "Print Error",
                        "Failed to print receipt. Please check the printer and try again.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print receipt for ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to print receipt: {ex.Message}");
        }
    }

    private async Task OnToggleTaxExemptAsync()
    {
        if (_ticket == null)
        {
            _logger.LogWarning("Cannot toggle tax exempt: no ticket loaded");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "No ticket is currently loaded. Please return to the order page and try again.");
            return;
        }

        if (_userService.CurrentUser == null)
        {
            _logger.LogError("Cannot toggle tax exempt: no user logged in");
            await _dialogService.ShowErrorAsync(
                "Authentication Error",
                "No user is currently logged in. Please log in and try again.");
            return;
        }

        try
        {
            IsBusy = true;

            var newTaxExemptStatus = !IsTaxExempt;

            var command = new SetTaxExemptCommand
            {
                TicketId = _ticket.Id,
                IsTaxExempt = newTaxExemptStatus,
                ModifiedBy = new UserId(_userService.CurrentUser.Id)
            };

            var result = await _setTaxExemptHandler.HandleAsync(command);

            if (result.Success)
            {
                _logger.LogInformation("Tax exempt status toggled to {Status} for ticket {TicketId}",
                    newTaxExemptStatus, _ticketId);

                // Reload ticket to get recalculated totals
                await LoadTicketAsync();
            }
            else
            {
                _logger.LogError("Failed to toggle tax exempt: {Error}", result.Error);
                await _dialogService.ShowErrorAsync(
                    "Tax Exempt Error",
                    $"Unable to change tax exempt status:\n\n{result.Error}");
            }
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while toggling tax exempt for ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync(
                "Network Error",
                "Unable to connect to the server. Please check your network connection and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to toggle tax exempt for ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync(
                "Error",
                $"An error occurred while changing tax exempt status:\n\n{ex.Message}",
                ex.ToString());
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnCancelSettlement()
    {
        // Navigate back without processing any payments
        // Ticket state is preserved (no modifications made)
        _logger.LogInformation("Settlement cancelled for ticket {TicketId}", _ticketId);
        _navigationService.GoBack();
    }

    private void OnNavigateBack()
    {
        // Navigate back to order page
        _logger.LogInformation("Navigating back from settle page for ticket {TicketId}", _ticketId);
        _navigationService.GoBack();
    }

    #endregion
}

/// <summary>
/// Represents a payment method option in the UI.
/// </summary>
public class PaymentMethodViewModel
{
    public PaymentMethodViewModel(PaymentType type, string displayName, string iconName, string backgroundColor)
    {
        Type = type;
        DisplayName = displayName;
        IconName = iconName;
        BackgroundColor = backgroundColor;
        IsEnabled = true;
    }

    public PaymentType Type { get; }
    public string DisplayName { get; }
    public string IconName { get; }
    public string BackgroundColor { get; }
    public bool IsEnabled { get; }
}

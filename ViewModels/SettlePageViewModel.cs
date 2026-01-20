using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
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
    private readonly ILogger<SettlePageViewModel> _logger;

    private Guid _ticketId;
    private TicketDto? _ticket;
    private decimal _tenderAmount;
    private string _tenderAmountDisplay = "$0.00";

    public SettlePageViewModel(
        IQueryHandler<GetTicketQuery, TicketDto?> getTicketHandler,
        ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult> processPaymentHandler,
        ICommandHandler<SetTaxExemptCommand, SetTaxExemptResult> setTaxExemptHandler,
        NavigationService navigationService,
        IUserService userService,
        ITerminalContext terminalContext,
        ICashSessionRepository cashSessionRepository,
        IServiceScopeFactory serviceScopeFactory,
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
    public string TenderAmountDisplay
    {
        get => _tenderAmountDisplay;
        private set => SetProperty(ref _tenderAmountDisplay, value);
    }

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
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load ticket {TicketId}", _ticketId);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnKeypadDigit(string? digit)
    {
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
            if (!_tenderAmountDisplay.Contains("."))
            {
                // If display is "$0.00", start fresh with "0."
                if (_tenderAmountDisplay == "$0.00")
                {
                    _tenderAmountDisplay = "0.";
                }
                else
                {
                    // Remove currency formatting and append decimal
                    var numericValue = _tenderAmountDisplay.Replace("$", "").Replace(",", "");
                    _tenderAmountDisplay = numericValue + ".";
                }
                TenderAmountDisplay = _tenderAmountDisplay;
            }
            return;
        }

        // Handle digits 0-9
        if (digit.Length == 1 && char.IsDigit(digit[0]))
        {
            // If display is "$0.00", start fresh
            if (_tenderAmountDisplay == "$0.00")
            {
                _tenderAmountDisplay = digit;
            }
            else
            {
                // Remove currency formatting and append digit
                var numericValue = _tenderAmountDisplay.Replace("$", "").Replace(",", "");
                _tenderAmountDisplay = numericValue + digit;
            }

            // Update display with currency formatting
            if (decimal.TryParse(_tenderAmountDisplay, out var amount))
            {
                _tenderAmount = amount;
                TenderAmountDisplay = FormatCurrency(amount);
            }
            else
            {
                // Keep building the string (e.g., "0.5" before it becomes "0.50")
                TenderAmountDisplay = _tenderAmountDisplay;
            }
        }
    }

    private string FormatCurrency(decimal amount)
    {
        return amount.ToString("C2");
    }

    private void OnClearTender()
    {
        _tenderAmount = 0m;
        _tenderAmountDisplay = "$0.00";
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
        _tenderAmountDisplay = FormatCurrency(amount);
        TenderAmountDisplay = _tenderAmountDisplay;
        
        _logger.LogDebug("Quick cash amount set to {Amount}", amount);
    }

    private async Task ProcessPaymentAsync(PaymentType paymentType)
    {
        if (_ticket == null)
        {
            _logger.LogWarning("Cannot process payment: no ticket loaded");
            return;
        }

        if (_tenderAmount <= 0)
        {
            _logger.LogWarning("Cannot process payment: tender amount is zero or negative");
            return;
        }

        if (_userService.CurrentUser == null)
        {
            _logger.LogError("Cannot process payment: no user logged in");
            return;
        }

        if (_terminalContext.TerminalId == null)
        {
            _logger.LogError("Cannot process payment: no terminal context");
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
                    // Change will be displayed in UI
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
                    // Ticket is fully paid - navigate back
                    _logger.LogInformation("Ticket {TicketId} is fully paid", _ticketId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process payment for ticket {TicketId}", _ticketId);
        }
        finally
        {
            IsProcessingPayment = false;
            IsBusy = false;
        }
    }

    private async Task OnAddTipAsync()
    {
        // TODO: Show tip entry dialog
        // This will be implemented when the tip dialog is created
        _logger.LogInformation("Add tip requested for ticket {TicketId}", _ticketId);
        await Task.CompletedTask;
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
            // Save ticket state and navigate back to order page
            _logger.LogInformation("Holding ticket {TicketId}", _ticketId);
            _navigationService.GoBack();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to hold ticket {TicketId}", _ticketId);
        }

        await Task.CompletedTask;
    }

    private async Task OnSplitPaymentAsync()
    {
        // TODO: Show split payment dialog
        // This will be implemented when the split payment dialog is created
        _logger.LogInformation("Split payment requested for ticket {TicketId}", _ticketId);
        await Task.CompletedTask;
    }

    private async Task OnApplyDiscountAsync()
    {
        // TODO: Show discount dialog
        // This will be implemented when the discount dialog is created
        _logger.LogInformation("Apply discount requested for ticket {TicketId}", _ticketId);
        await Task.CompletedTask;
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
            // TODO: Implement receipt printing
            // This will be implemented when the receipt printing service is available
            _logger.LogInformation("Print receipt requested for ticket {TicketId}", _ticketId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print receipt for ticket {TicketId}", _ticketId);
        }

        await Task.CompletedTask;
    }

    private async Task OnToggleTaxExemptAsync()
    {
        if (_ticket == null)
        {
            _logger.LogWarning("Cannot toggle tax exempt: no ticket loaded");
            return;
        }

        if (_userService.CurrentUser == null)
        {
            _logger.LogError("Cannot toggle tax exempt: no user logged in");
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
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to toggle tax exempt for ticket {TicketId}", _ticketId);
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

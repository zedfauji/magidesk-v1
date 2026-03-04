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
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.Views.Dialogs;
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
    private readonly IUserContextService _userContextService;
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
        IUserContextService userContextService,
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
        _userContextService = userContextService ?? throw new ArgumentNullException(nameof(userContextService));
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
}

using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using System.Net.Sockets;
using Magidesk.Presentation.Services;

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

    private TicketDto? _ticket;
    private decimal _tenderAmount;
    private string _numpadInput = "";
    private string? _error;

    private Guid _ticketId;

    public SettleViewModel(
        IQueryHandler<GetTicketQuery, TicketDto?> getTicket,
        ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult> processPayment,
        ICommandHandler<SetTaxExemptCommand, SetTaxExemptResult> setTaxExempt,
        ICommandHandler<LogoutCommand> logoutHandler,
        Services.NavigationService navigationService,
        IUserService userService,
        ITerminalContext terminalContext,
        ICashSessionRepository cashSessionRepository)
    {
        _getTicket = getTicket;
        _processPayment = processPayment;
        _setTaxExempt = setTaxExempt;
        _logoutHandler = logoutHandler;
        _navigationService = navigationService;
        _userService = userService;
        _terminalContext = terminalContext;
        _cashSessionRepository = cashSessionRepository;

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
        QuickCashCommand = new RelayCommand<decimal>(OnQuickCash);
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
    
    public bool HasDueAmount => DueAmount > 0;

    public string? Error
    {
        get => _error;
        set => SetProperty(ref _error, value);
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
    public RelayCommand<decimal> QuickCashCommand { get; }

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

    private void OnQuickCash(decimal amount)
    {
        // F-0043: Quick Cash Buttons - adds denomination to tender amount
        if (amount <= 0) return;
        
        // Cumulative mode: add to existing tender amount
        var newTenderAmount = TenderAmount + amount;
        TenderAmount = newTenderAmount;
        NumpadInput = newTenderAmount.ToString("F2");
    }

    private async Task OnNoSaleAsync()
    {
        // F-0007: NO SALE behavior (drawer kick).
        // Should trigger drawer kick command (if hardware connected) and log audit.
        IsBusy = true;
        try
        {
            // TODO: Call backend "OpenDrawerCommand" or similar.
            // For now, simulated delay.
            await Task.Delay(100);
            Error = "Drawer Opened (No Sale)";
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
            Ticket = await _getTicket.HandleAsync(new GetTicketQuery { TicketId = _ticketId });
            if (Ticket == null)
            {
                Error = "Ticket not found.";
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
        
        IsBusy = true;
        Error = null;
        try
        {
            var result = await _setTaxExempt.HandleAsync(new SetTaxExemptCommand
            {
                TicketId = Ticket.Id,
                IsTaxExempt = !Ticket.IsTaxExempt,
                ModifiedBy = _userService.CurrentUser != null ? new UserId(_userService.CurrentUser.Id) : new UserId(Guid.Empty) 
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
        
        if (!Enum.TryParse<PaymentType>(paymentTypeString, out var paymentType))
        {
            Error = $"Invalid payment type: {paymentTypeString}";
            return;
        }
        
        IsBusy = true;
        Error = null;
        try
        {
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
                 
                 // Resolve Cash Session
                 var session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
                 if (session != null)
                 {
                     command.CashSessionId = session.Id;
                 }
                 else
                 {
                     // Strict: Cannot pay cash without open drawer?
                     // Audit F-0007: "Must have valid cash session"
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

            var result = await _processPayment.HandleAsync(command);
            
            if (result.TicketIsPaid)
            {
                // Close and Navigate back
                OnClose();
            }
            else
            {
                // Reload to show remaining balance
                await LoadTicketAsync();
                
                // Clear numpad for next payment
                OnClearNumpad();
            }
            
            if (paymentType == PaymentType.Cash && result.ChangeAmount.Amount > 0)
            {
                // TODO: Show Change Dialog
                Error = $"Change Due: {result.ChangeAmount}"; 
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

    private void OnClose()
    {
        _navigationService.GoBack();
    }

    public AsyncRelayCommand TestWaitCommand { get; }
    public AsyncRelayCommand SwipeCardCommand { get; }

    private async Task TestWaitAsync()
    {
        var dialog = new Magidesk.Views.PaymentProcessWaitDialog();
        dialog.ViewModel.SetMessage("Testing Wait Dialog (3s)...");
        
        // Show dialog without awaiting it to allow background task simulation? 
        // No, ShowDialogAsync awaits until closed. 
        // We need a way to close it programmatically.
        // Since we can't easily reference the dialog instance from VM after showing, 
        // we usually run the background task BEFORE showing, or pass a unified controller.
        // FOR THIS TEST: We will fire-and-forget the closer logic before showing.
        
        var closeTask = Task.Run(async () =>
        {
            await Task.Delay(3000);
            // Must dispatch to UI thread to close
            _navigationService.DispatcherQueue.TryEnqueue(() =>
            {
                dialog.ViewModel.AllowClose();
                dialog.Hide(); // ContentDialog.Hide() closes it
            });
        });

        await _navigationService.ShowDialogAsync(dialog);
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
        }
        finally
        {
            IsBusy = false;
        }
    }
}

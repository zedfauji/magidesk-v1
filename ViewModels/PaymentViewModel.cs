using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Services;

namespace Magidesk.Presentation.ViewModels;

public sealed class PaymentViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetTicketQuery, TicketDto?> _getTicket;
    private readonly ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult> _processPayment;
    private readonly NavigationService _navigationService;
    private readonly IErrorService _errorService;

    private TicketDto? _ticket;
    private string _ticketIdText = string.Empty;

    private string _processedByText = Guid.Empty.ToString();
    private string _terminalIdText = Guid.Empty.ToString();
    private string _cashSessionIdText = string.Empty;

    private string _amountText = "0";
    private string _tenderText = "0";
    private string _tipsText = "0";
    private string? _note;

    private string? _error;

    public PaymentViewModel(
        IQueryHandler<GetTicketQuery, TicketDto?> getTicket,
        ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult> processPayment,
        NavigationService navigationService,
        IErrorService errorService)
    {
        _getTicket = getTicket;
        _processPayment = processPayment;
        _navigationService = navigationService;
        _errorService = errorService;

        Title = "Payment";

        LoadTicketCommand = new AsyncRelayCommand(LoadTicketAsync);
        CashPayCommand = new AsyncRelayCommand(CashPayAsync);
        GoBackCommand = new RelayCommand(GoBack);
    }

    public TicketDto? Ticket
    {
        get => _ticket;
        private set
        {
            if (SetProperty(ref _ticket, value))
            {
                OnPropertyChanged(nameof(HasTicket));
                OnPropertyChanged(nameof(TicketHeaderText));
                OnPropertyChanged(nameof(TotalsText));
                OnPropertyChanged(nameof(Payments));
                OnPropertyChanged(nameof(DueAmountText));
            }
        }
    }

    public bool HasTicket => Ticket != null;

    public string TicketIdText
    {
        get => _ticketIdText;
        set => SetProperty(ref _ticketIdText, value);
    }

    public string TicketHeaderText => Ticket == null
        ? "No ticket loaded"
        : $"Ticket #{Ticket.TicketNumber} ({Ticket.Status})\nId: {Ticket.Id}";

    public string TotalsText => Ticket == null
        ? string.Empty
        : $"Total: {Ticket.TotalAmount}  Paid: {Ticket.PaidAmount}  Due: {Ticket.DueAmount}";

    public string DueAmountText => Ticket == null ? string.Empty : Ticket.DueAmount.ToString();

    public IReadOnlyList<PaymentDto> Payments => Ticket?.Payments ?? new List<PaymentDto>();

    public string ProcessedByText
    {
        get => _processedByText;
        set => SetProperty(ref _processedByText, value);
    }

    public string TerminalIdText
    {
        get => _terminalIdText;
        set => SetProperty(ref _terminalIdText, value);
    }

    public string CashSessionIdText
    {
        get => _cashSessionIdText;
        set => SetProperty(ref _cashSessionIdText, value);
    }

    public string AmountText
    {
        get => _amountText;
        set => SetProperty(ref _amountText, value);
    }

    public string TenderText
    {
        get => _tenderText;
        set => SetProperty(ref _tenderText, value);
    }

    public string TipsText
    {
        get => _tipsText;
        set => SetProperty(ref _tipsText, value);
    }

    public string? Note
    {
        get => _note;
        set => SetProperty(ref _note, value);
    }

    public string? Error
    {
        get => _error;
        private set
        {
            if (SetProperty(ref _error, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(Error);

    public AsyncRelayCommand LoadTicketCommand { get; }
    public AsyncRelayCommand CashPayCommand { get; }
    public RelayCommand GoBackCommand { get; }

    private void GoBack()
    {
        if (_navigationService.CanGoBack)
        {
            _navigationService.GoBack();
        }
    }

    private async Task LoadTicketAsync()
    {
        Error = null;
        IsBusy = true;
        try
        {
            // T004: Null dependency checks
            if (_getTicket == null)
            {
                await _errorService?.ShowFatalAsync("Service Missing", "GetTicket handler is not available.");
                return;
            }

            if (!Guid.TryParse(TicketIdText, out var ticketId)) { Error = "Invalid TicketId."; return; }

            Ticket = await _getTicket.HandleAsync(new GetTicketQuery { TicketId = ticketId });
            if (Ticket == null)
            {
                // T011: Replace property-only error with ErrorService dialog
                await _errorService.ShowErrorAsync("Ticket Not Found", "The specified ticket could not be found.", ex.ToString());
                Error = "Ticket not found.";
                return;
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

    private async Task CashPayAsync()
    {
        Error = null;
        IsBusy = true;
        try
        {
            // T004: Null dependency checks
            if (Ticket == null) { Error = "Load a ticket first."; return; }
            if (Ticket.DueAmount <= 0) return; // Nothing to pay

            if (_processPayment == null)
            {
                await _errorService?.ShowFatalAsync("Service Missing", "Process payment handler is not available.");
                return;
            }

            if (_navigationService == null)
            {
                await _errorService?.ShowFatalAsync("Service Missing", "Navigation service is not available.");
                return;
            }

            if (!Guid.TryParse(ProcessedByText, out var processedBy)) { Error = "Invalid ProcessedBy."; return; }
            if (!Guid.TryParse(TerminalIdText, out var terminalId)) { Error = "Invalid TerminalId."; return; }
            if (!decimal.TryParse(AmountText, out var amount)) { Error = "Invalid Amount."; return; }
            if (!decimal.TryParse(TenderText, out var tender)) { Error = "Invalid Tender."; return; }
            if (!decimal.TryParse(TipsText, out var tips)) { Error = "Invalid Tips."; return; }

            Guid? cashSessionId = null;
            if (!string.IsNullOrWhiteSpace(CashSessionIdText))
            {
                if (!Guid.TryParse(CashSessionIdText, out var csid)) { Error = "Invalid CashSessionId."; return; }
                cashSessionId = csid;
            }

            await _processPayment.HandleAsync(new ProcessPaymentCommand
            {
                TicketId = Ticket.Id,
                PaymentType = PaymentType.Cash,
                Amount = new Money(amount),
                TenderAmount = new Money(tender),
                TipsAmount = tips > 0 ? new Money(tips) : null,
                ProcessedBy = new UserId(processedBy),
                TerminalId = terminalId,
                CashSessionId = cashSessionId,
                Note = Note
            });

            await LoadTicketAsync();
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
}

using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Presentation.ViewModels;

public sealed class TicketManagementViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetOpenTicketsQuery, IEnumerable<TicketDto>> _getOpenTickets;
    private readonly ICommandHandler<VoidTicketCommand> _voidTicket;
    private readonly ICommandHandler<RefundTicketCommand, RefundTicketResult> _refundTicket;
    private readonly ICommandHandler<SplitTicketCommand, SplitTicketResult> _splitTicket;

    private List<TicketDto> _openTickets = new();
    private TicketDto? _selectedTicket;

    private string _voidedByText = Guid.Empty.ToString();

    private string _refundProcessedByText = Guid.Empty.ToString();
    private string _refundTerminalIdText = Guid.Empty.ToString();
    private string? _refundReason;

    private string _splitByText = Guid.Empty.ToString();
    private string _splitTerminalIdText = Guid.Empty.ToString();
    private string _splitShiftIdText = Guid.Empty.ToString();
    private string _splitOrderTypeIdText = Guid.Empty.ToString();

    private string? _error;
    private string? _lastResult;

    public TicketManagementViewModel(
        IQueryHandler<GetOpenTicketsQuery, IEnumerable<TicketDto>> getOpenTickets,
        ICommandHandler<VoidTicketCommand> voidTicket,
        ICommandHandler<RefundTicketCommand, RefundTicketResult> refundTicket,
        ICommandHandler<SplitTicketCommand, SplitTicketResult> splitTicket)
    {
        _getOpenTickets = getOpenTickets;
        _voidTicket = voidTicket;
        _refundTicket = refundTicket;
        _splitTicket = splitTicket;

        Title = "Ticket Management";

        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        VoidSelectedCommand = new AsyncRelayCommand(VoidSelectedAsync);
        RefundSelectedCommand = new AsyncRelayCommand(RefundSelectedAsync);
        SplitSelectedCommand = new AsyncRelayCommand(SplitSelectedAsync);
    }

    public IReadOnlyList<TicketDto> OpenTickets
    {
        get => _openTickets;
        private set => SetProperty(ref _openTickets, value.ToList());
    }

    public TicketDto? SelectedTicket
    {
        get => _selectedTicket;
        set
        {
            if (SetProperty(ref _selectedTicket, value))
            {
                OnPropertyChanged(nameof(SelectedSummaryText));
            }
        }
    }

    public string SelectedSummaryText => SelectedTicket == null
        ? "No ticket selected"
        : $"Ticket #{SelectedTicket.TicketNumber} ({SelectedTicket.Status})\n" +
          $"Total: {SelectedTicket.TotalAmount}  Paid: {SelectedTicket.PaidAmount}  Due: {SelectedTicket.DueAmount}\n" +
          $"Lines: {SelectedTicket.OrderLines.Count}  Payments: {SelectedTicket.Payments.Count}";

    public string VoidedByText
    {
        get => _voidedByText;
        set => SetProperty(ref _voidedByText, value);
    }

    public string RefundProcessedByText
    {
        get => _refundProcessedByText;
        set => SetProperty(ref _refundProcessedByText, value);
    }

    public string RefundTerminalIdText
    {
        get => _refundTerminalIdText;
        set => SetProperty(ref _refundTerminalIdText, value);
    }

    public string? RefundReason
    {
        get => _refundReason;
        set => SetProperty(ref _refundReason, value);
    }

    public string SplitByText
    {
        get => _splitByText;
        set => SetProperty(ref _splitByText, value);
    }

    public string SplitTerminalIdText
    {
        get => _splitTerminalIdText;
        set => SetProperty(ref _splitTerminalIdText, value);
    }

    public string SplitShiftIdText
    {
        get => _splitShiftIdText;
        set => SetProperty(ref _splitShiftIdText, value);
    }

    public string SplitOrderTypeIdText
    {
        get => _splitOrderTypeIdText;
        set => SetProperty(ref _splitOrderTypeIdText, value);
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

    public string? LastResult
    {
        get => _lastResult;
        private set => SetProperty(ref _lastResult, value);
    }

    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand VoidSelectedCommand { get; }
    public AsyncRelayCommand RefundSelectedCommand { get; }
    public AsyncRelayCommand SplitSelectedCommand { get; }

    public async Task RefreshAsync()
    {
        Error = null;
        LastResult = null;
        IsBusy = true;

        try
        {
            var tickets = await _getOpenTickets.HandleAsync(new GetOpenTicketsQuery());
            OpenTickets = tickets.ToList();

            if (SelectedTicket != null)
            {
                SelectedTicket = OpenTickets.FirstOrDefault(t => t.Id == SelectedTicket.Id);
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

    private async Task VoidSelectedAsync()
    {
        Error = null;
        LastResult = null;
        IsBusy = true;

        try
        {
            if (SelectedTicket == null) { Error = "Select a ticket first."; return; }
            if (!Guid.TryParse(VoidedByText, out var voidedBy)) { Error = "Invalid VoidedBy."; return; }

            await _voidTicket.HandleAsync(new VoidTicketCommand
            {
                TicketId = SelectedTicket.Id,
                VoidedBy = new UserId(voidedBy)
            });

            LastResult = $"Voided ticket #{SelectedTicket.TicketNumber}.";
            await RefreshAsync();
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

    private async Task RefundSelectedAsync()
    {
        Error = null;
        LastResult = null;
        IsBusy = true;

        try
        {
            if (SelectedTicket == null) { Error = "Select a ticket first."; return; }
            if (!Guid.TryParse(RefundProcessedByText, out var processedBy)) { Error = "Invalid ProcessedBy."; return; }
            if (!Guid.TryParse(RefundTerminalIdText, out var terminalId)) { Error = "Invalid TerminalId."; return; }

            var result = await _refundTicket.HandleAsync(new RefundTicketCommand
            {
                TicketId = SelectedTicket.Id,
                ProcessedBy = new UserId(processedBy),
                TerminalId = terminalId,
                Reason = RefundReason
            });

            if (!result.Success)
            {
                Error = result.ErrorMessage ?? "Refund failed.";
                return;
            }

            LastResult = $"Refunded ticket #{SelectedTicket.TicketNumber}. Refund payments: {result.RefundPaymentsCreated}.";
            await RefreshAsync();
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

    private async Task SplitSelectedAsync()
    {
        Error = null;
        LastResult = null;
        IsBusy = true;

        try
        {
            if (SelectedTicket == null) { Error = "Select a ticket first."; return; }
            if (!Guid.TryParse(SplitByText, out var splitBy)) { Error = "Invalid SplitBy."; return; }
            if (!Guid.TryParse(SplitTerminalIdText, out var terminalId)) { Error = "Invalid TerminalId."; return; }
            if (!Guid.TryParse(SplitShiftIdText, out var shiftId)) { Error = "Invalid ShiftId."; return; }
            if (!Guid.TryParse(SplitOrderTypeIdText, out var orderTypeId)) { Error = "Invalid OrderTypeId."; return; }

            // Basic split: move the first order line if any
            var lineToMove = SelectedTicket.OrderLines.FirstOrDefault();
            if (lineToMove == null) { Error = "Ticket has no order lines to split."; return; }

            var result = await _splitTicket.HandleAsync(new SplitTicketCommand
            {
                OriginalTicketId = SelectedTicket.Id,
                SplitBy = new UserId(splitBy),
                TerminalId = terminalId,
                ShiftId = shiftId,
                OrderTypeId = orderTypeId,
                OrderLineIdsToSplit = new List<Guid> { lineToMove.Id }
            });

            if (!result.Success)
            {
                Error = result.ErrorMessage ?? "Split failed.";
                return;
            }

            LastResult = $"Split created ticket #{result.NewTicketNumber}. Lines moved: {result.OrderLinesMoved}.";
            await RefreshAsync();
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

using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;

namespace Magidesk.Presentation.ViewModels;

public sealed class PrintViewModel : ViewModelBase
{
    private readonly ICommandHandler<PrintToKitchenCommand, PrintToKitchenResult> _printToKitchen;
    private readonly ICommandHandler<PrintReceiptCommand, PrintReceiptResult> _printReceipt;

    private string _ticketIdText = string.Empty;
    private string _orderLineIdText = string.Empty;
    private string _paymentIdText = string.Empty;
    private ReceiptType _selectedReceiptType = ReceiptType.Ticket;

    private string? _lastResult;
    private string? _error;

    public PrintViewModel(
        ICommandHandler<PrintToKitchenCommand, PrintToKitchenResult> printToKitchen,
        ICommandHandler<PrintReceiptCommand, PrintReceiptResult> printReceipt)
    {
        _printToKitchen = printToKitchen;
        _printReceipt = printReceipt;

        Title = "Printing";

        PrintKitchenCommand = new AsyncRelayCommand(PrintKitchenAsync);
        PrintReceiptCommand = new AsyncRelayCommand(PrintReceiptAsync);
    }

    public string TicketIdText
    {
        get => _ticketIdText;
        set => SetProperty(ref _ticketIdText, value);
    }

    public string OrderLineIdText
    {
        get => _orderLineIdText;
        set => SetProperty(ref _orderLineIdText, value);
    }

    public string PaymentIdText
    {
        get => _paymentIdText;
        set => SetProperty(ref _paymentIdText, value);
    }

    public ReceiptType SelectedReceiptType
    {
        get => _selectedReceiptType;
        set => SetProperty(ref _selectedReceiptType, value);
    }

    public IEnumerable<ReceiptType> ReceiptTypes => Enum.GetValues<ReceiptType>();

    public string? LastResult
    {
        get => _lastResult;
        private set => SetProperty(ref _lastResult, value);
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

    public AsyncRelayCommand PrintKitchenCommand { get; }
    public AsyncRelayCommand PrintReceiptCommand { get; }

    private async Task PrintKitchenAsync()
    {
        Error = null;
        LastResult = null;
        IsBusy = true;

        try
        {
            if (!Guid.TryParse(TicketIdText, out var ticketId))
            {
                Error = "Invalid TicketId.";
                return;
            }

            Guid? orderLineId = null;
            if (!string.IsNullOrWhiteSpace(OrderLineIdText))
            {
                if (!Guid.TryParse(OrderLineIdText, out var olid))
                {
                    Error = "Invalid OrderLineId.";
                    return;
                }

                orderLineId = olid;
            }

            var result = await _printToKitchen.HandleAsync(new PrintToKitchenCommand
            {
                TicketId = ticketId,
                OrderLineId = orderLineId
            });

            LastResult = result.Success
                ? $"Kitchen print OK. Lines printed: {result.OrderLinesPrinted}" 
                : "Kitchen print did not print any lines.";
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

    private async Task PrintReceiptAsync()
    {
        Error = null;
        LastResult = null;
        IsBusy = true;

        try
        {
            if (!Guid.TryParse(TicketIdText, out var ticketId))
            {
                Error = "Invalid TicketId.";
                return;
            }

            Guid? paymentId = null;
            if (!string.IsNullOrWhiteSpace(PaymentIdText))
            {
                if (!Guid.TryParse(PaymentIdText, out var pid))
                {
                    Error = "Invalid PaymentId.";
                    return;
                }

                paymentId = pid;
            }

            var result = await _printReceipt.HandleAsync(new PrintReceiptCommand
            {
                TicketId = ticketId,
                PaymentId = paymentId,
                ReceiptType = SelectedReceiptType
            });

            LastResult = result.Success ? "Receipt print OK." : "Receipt print failed.";
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

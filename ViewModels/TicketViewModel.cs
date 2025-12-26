using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Presentation.ViewModels;

public sealed class TicketViewModel : ViewModelBase
{
    private readonly ICommandHandler<CreateTicketCommand, CreateTicketResult> _createTicket;
    private readonly ICommandHandler<AddOrderLineCommand, AddOrderLineResult> _addOrderLine;
    private readonly IQueryHandler<GetTicketQuery, TicketDto?> _getTicket;

    private TicketDto? _ticket;
    private string _createdByText = Guid.Empty.ToString();
    private string _terminalIdText = Guid.Empty.ToString();
    private string _shiftIdText = Guid.Empty.ToString();
    private string _orderTypeIdText = Guid.Empty.ToString();
    private string _ticketIdText = string.Empty;

    private string _menuItemIdText = Guid.Empty.ToString();
    private string _menuItemName = "Test Item";
    private string _quantityText = "1";
    private string _unitPriceText = "10";
    private string _taxRateText = "0";

    private string? _error;

    public TicketViewModel(
        ICommandHandler<CreateTicketCommand, CreateTicketResult> createTicket,
        ICommandHandler<AddOrderLineCommand, AddOrderLineResult> addOrderLine,
        IQueryHandler<GetTicketQuery, TicketDto?> getTicket)
    {
        _createTicket = createTicket;
        _addOrderLine = addOrderLine;
        _getTicket = getTicket;

        Title = "Ticket";

        CreateTicketCommand = new AsyncRelayCommand(CreateTicketAsync);
        LoadTicketCommand = new AsyncRelayCommand(LoadTicketAsync);
        AddOrderLineUiCommand = new AsyncRelayCommand(AddOrderLineAsync);
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
                OnPropertyChanged(nameof(OrderLines));
            }
        }
    }

    public bool HasTicket => Ticket != null;

    public string TicketHeaderText => Ticket == null
        ? "No ticket loaded"
        : $"Ticket #{Ticket.TicketNumber} ({Ticket.Status})\nId: {Ticket.Id}";

    public string TotalsText => Ticket == null
        ? string.Empty
        : $"Subtotal: {Ticket.SubtotalAmount}  Tax: {Ticket.TaxAmount}  Total: {Ticket.TotalAmount}  Due: {Ticket.DueAmount}";

    public IReadOnlyList<OrderLineDto> OrderLines => Ticket?.OrderLines ?? new List<OrderLineDto>();

    public string CreatedByText
    {
        get => _createdByText;
        set => SetProperty(ref _createdByText, value);
    }

    public string TerminalIdText
    {
        get => _terminalIdText;
        set => SetProperty(ref _terminalIdText, value);
    }

    public string ShiftIdText
    {
        get => _shiftIdText;
        set => SetProperty(ref _shiftIdText, value);
    }

    public string OrderTypeIdText
    {
        get => _orderTypeIdText;
        set => SetProperty(ref _orderTypeIdText, value);
    }

    public string TicketIdText
    {
        get => _ticketIdText;
        set => SetProperty(ref _ticketIdText, value);
    }

    public string MenuItemIdText
    {
        get => _menuItemIdText;
        set => SetProperty(ref _menuItemIdText, value);
    }

    public string MenuItemName
    {
        get => _menuItemName;
        set => SetProperty(ref _menuItemName, value);
    }

    public string QuantityText
    {
        get => _quantityText;
        set => SetProperty(ref _quantityText, value);
    }

    public string UnitPriceText
    {
        get => _unitPriceText;
        set => SetProperty(ref _unitPriceText, value);
    }

    public string TaxRateText
    {
        get => _taxRateText;
        set => SetProperty(ref _taxRateText, value);
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

    public AsyncRelayCommand CreateTicketCommand { get; }
    public AsyncRelayCommand LoadTicketCommand { get; }
    public AsyncRelayCommand AddOrderLineUiCommand { get; }

    private async Task CreateTicketAsync()
    {
        Error = null;
        IsBusy = true;
        try
        {
            if (!Guid.TryParse(CreatedByText, out var createdBy)) { Error = "Invalid CreatedBy."; return; }
            if (!Guid.TryParse(TerminalIdText, out var terminalId)) { Error = "Invalid TerminalId."; return; }
            if (!Guid.TryParse(ShiftIdText, out var shiftId)) { Error = "Invalid ShiftId."; return; }
            if (!Guid.TryParse(OrderTypeIdText, out var orderTypeId)) { Error = "Invalid OrderTypeId."; return; }

            var result = await _createTicket.HandleAsync(new CreateTicketCommand
            {
                CreatedBy = new UserId(createdBy),
                TerminalId = terminalId,
                ShiftId = shiftId,
                OrderTypeId = orderTypeId
            });

            TicketIdText = result.TicketId.ToString();
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

    private async Task LoadTicketAsync()
    {
        Error = null;
        IsBusy = true;
        try
        {
            if (!Guid.TryParse(TicketIdText, out var ticketId)) { Error = "Invalid TicketId."; return; }

            Ticket = await _getTicket.HandleAsync(new GetTicketQuery { TicketId = ticketId });

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

    private async Task AddOrderLineAsync()
    {
        Error = null;
        IsBusy = true;
        try
        {
            if (Ticket == null) { Error = "Load or create a ticket first."; return; }
            if (!Guid.TryParse(MenuItemIdText, out var menuItemId)) { Error = "Invalid MenuItemId."; return; }
            if (!decimal.TryParse(QuantityText, out var qty)) { Error = "Invalid Quantity."; return; }
            if (!decimal.TryParse(UnitPriceText, out var unitPrice)) { Error = "Invalid Unit Price."; return; }
            if (!decimal.TryParse(TaxRateText, out var taxRate)) { Error = "Invalid Tax Rate."; return; }

            await _addOrderLine.HandleAsync(new AddOrderLineCommand
            {
                TicketId = Ticket.Id,
                MenuItemId = menuItemId,
                MenuItemName = MenuItemName,
                Quantity = qty,
                UnitPrice = new Money(unitPrice),
                TaxRate = taxRate
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

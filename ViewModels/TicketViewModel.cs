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
    private readonly ICommandHandler<PrintToKitchenCommand, PrintToKitchenResult> _printToKitchen;
    private readonly IQueryHandler<GetTicketQuery, TicketDto?> _getTicket;
    private readonly IMenuRepository _menuRepository;
    private readonly Services.NavigationService _navigationService;

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
        ICommandHandler<PrintToKitchenCommand, PrintToKitchenResult> printToKitchen,
        IQueryHandler<GetTicketQuery, TicketDto?> getTicket,
        IMenuRepository menuRepository,
        Services.NavigationService navigationService)
    {
        _createTicket = createTicket;
        _addOrderLine = addOrderLine;
        _printToKitchen = printToKitchen;
        _getTicket = getTicket;
        _menuRepository = menuRepository;
        _navigationService = navigationService;

        Title = "Ticket";

        CreateTicketCommand = new AsyncRelayCommand(CreateTicketAsync);
        LoadTicketCommand = new AsyncRelayCommand(LoadTicketAsync);
        AddOrderLineUiCommand = new AsyncRelayCommand(AddOrderLineAsync);
        SendToKitchenCommand = new AsyncRelayCommand(SendToKitchenAsync);
        SplitTicketUiCommand = new AsyncRelayCommand(SplitTicketAsync);
        MoveTableUiCommand = new RelayCommand(MoveTable);
        SettleUiCommand = new RelayCommand(Settle);
    }

    private void MoveTable()
    {
        if (Ticket == null)
        {
            Error = "No active ticket to move.";
            return;
        }

        _navigationService.Navigate(typeof(Magidesk.Presentation.Views.TableMapPage), Ticket.Id);
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
                OnPropertyChanged(nameof(HasUnsentItems));
            }
        }
    }

    public bool HasTicket => Ticket != null;
    
    public bool HasUnsentItems => Ticket?.OrderLines.Any(ol => ol.ShouldPrintToKitchen && !ol.PrintedToKitchen) ?? false;

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
    public AsyncRelayCommand SendToKitchenCommand { get; }
    public AsyncRelayCommand SplitTicketUiCommand { get; }
    public RelayCommand MoveTableUiCommand { get; }
    public RelayCommand SettleUiCommand { get; }

    private void Settle()
    {
        if (Ticket == null)
        {
            Error = "No active ticket to settle.";
            return;
        }

        _navigationService.Navigate(typeof(Magidesk.Presentation.Views.SettlePage), Ticket.Id);
    }

    private async Task SplitTicketAsync()
    {
        Error = null;
        if (Ticket == null) return;
        
        IsBusy = true;
        try 
        {
             var dialog = new Magidesk.Presentation.Views.SplitTicketDialog(Ticket);
             var result = await _navigationService.ShowDialogAsync(dialog);
             
             if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
             {
                 // Split was executed successfully inside the dialog's closing event (via ViewModel)
                 // We just need to reload the current ticket (which should now have fewer items)
                 await LoadTicketAsync();
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

    private async Task SendToKitchenAsync()
    {
        Error = null;
        IsBusy = true;
        try
        {
            if (Ticket == null) return;
            
            var result = await _printToKitchen.HandleAsync(new PrintToKitchenCommand 
            { 
               TicketId = Ticket.Id 
            });

            if (result.Success)
            {
               await LoadTicketAsync();
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

            // Fetch Menu Item to check for modifiers
            var menuItem = await _menuRepository.GetByIdAsync(menuItemId);
            if (menuItem == null) { Error = "Menu Item not found."; return; }

            List<Magidesk.Domain.Entities.MenuModifier> selectedModifiers = new();

            // Check for active modifier groups
            if (menuItem.ModifierGroups.Any(mg => mg.ModifierGroup.IsActive))
            {
                 // We need to run this on UI thread, which we are.
                 // Instantiate Dialog
                 var dialog = new Magidesk.Presentation.Views.ModifierSelectionDialog(menuItem);
                 
                 // Show Dialog
                 var result = await _navigationService.ShowDialogAsync(dialog);

                 if (result != Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
                 {
                     // User cancelled (clicked Cancel or clicked outside if explicit cancel allowed)
                     return; 
                 }
                 
                 selectedModifiers = dialog.ViewModel.GetSelectedModifiers();
            }

            await _addOrderLine.HandleAsync(new AddOrderLineCommand
            {
                TicketId = Ticket.Id,
                MenuItemId = menuItemId,
                MenuItemName = MenuItemName,
                Quantity = qty,
                UnitPrice = new Money(unitPrice),
                TaxRate = taxRate,
                Modifiers = selectedModifiers
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

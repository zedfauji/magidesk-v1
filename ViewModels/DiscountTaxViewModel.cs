using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.ValueObjects;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;

public sealed class DiscountTaxViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetTicketQuery, TicketDto?> _getTicket;
    private readonly ICommandHandler<SetServiceChargeCommand, SetServiceChargeResult> _setServiceCharge;
    private readonly ICommandHandler<SetDeliveryChargeCommand, SetDeliveryChargeResult> _setDeliveryCharge;
    private readonly ICommandHandler<SetAdjustmentCommand, SetAdjustmentResult> _setAdjustment;
    private readonly ICommandHandler<SetAdvancePaymentCommand, SetAdvancePaymentResult> _setAdvance;

    private TicketDto? _ticket;
    private string _ticketIdText = string.Empty;
    private string _processedByText = Guid.Empty.ToString();

    private string _serviceChargeText = "0";
    private string _deliveryChargeText = "0";
    private string _adjustmentText = "0";
    private string _advanceText = "0";
    private string? _adjustmentReason;

    private string? _error;

    public DiscountTaxViewModel(
        IQueryHandler<GetTicketQuery, TicketDto?> getTicket,
        ICommandHandler<SetServiceChargeCommand, SetServiceChargeResult> setServiceCharge,
        ICommandHandler<SetDeliveryChargeCommand, SetDeliveryChargeResult> setDeliveryCharge,
        ICommandHandler<SetAdjustmentCommand, SetAdjustmentResult> setAdjustment,
        ICommandHandler<SetAdvancePaymentCommand, SetAdvancePaymentResult> setAdvance)
    {
        _getTicket = getTicket;
        _setServiceCharge = setServiceCharge;
        _setDeliveryCharge = setDeliveryCharge;
        _setAdjustment = setAdjustment;
        _setAdvance = setAdvance;

        Title = "Discount & Tax";

        LoadTicketCommand = new AsyncRelayCommand(LoadTicketAsync);
        SetServiceChargeUiCommand = new AsyncRelayCommand(SetServiceChargeAsync);
        SetDeliveryChargeUiCommand = new AsyncRelayCommand(SetDeliveryChargeAsync);
        SetAdjustmentUiCommand = new AsyncRelayCommand(SetAdjustmentAsync);
        SetAdvanceUiCommand = new AsyncRelayCommand(SetAdvanceAsync);
    }

    public TicketDto? Ticket
    {
        get => _ticket;
        private set
        {
            if (SetProperty(ref _ticket, value))
            {
                OnPropertyChanged(nameof(HasTicket));
                OnPropertyChanged(nameof(HeaderText));
                OnPropertyChanged(nameof(TotalsText));
            }
        }
    }

    public bool HasTicket => Ticket != null;

    public string TicketIdText
    {
        get => _ticketIdText;
        set => SetProperty(ref _ticketIdText, value);
    }

    public string ProcessedByText
    {
        get => _processedByText;
        set => SetProperty(ref _processedByText, value);
    }

    public string HeaderText => Ticket == null
        ? "No ticket loaded"
        : $"Ticket #{Ticket.TicketNumber} ({Ticket.Status})\nId: {Ticket.Id}";

    public string TotalsText => Ticket == null
        ? string.Empty
        : $"Subtotal: {Ticket.SubtotalAmount}  Discount: {Ticket.DiscountAmount}  Tax: {Ticket.TaxAmount}\n" +
          $"Svc: {Ticket.ServiceChargeAmount}  Delivery: {Ticket.DeliveryChargeAmount}  Adj: {Ticket.AdjustmentAmount}  Advance: {Ticket.AdvanceAmount}\n" +
          $"Total: {Ticket.TotalAmount}  Paid: {Ticket.PaidAmount}  Due: {Ticket.DueAmount}";

    public string ServiceChargeText
    {
        get => _serviceChargeText;
        set => SetProperty(ref _serviceChargeText, value);
    }

    public string DeliveryChargeText
    {
        get => _deliveryChargeText;
        set => SetProperty(ref _deliveryChargeText, value);
    }

    public string AdjustmentText
    {
        get => _adjustmentText;
        set => SetProperty(ref _adjustmentText, value);
    }

    public string AdvanceText
    {
        get => _advanceText;
        set => SetProperty(ref _advanceText, value);
    }

    public string? AdjustmentReason
    {
        get => _adjustmentReason;
        set => SetProperty(ref _adjustmentReason, value);
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
    public AsyncRelayCommand SetServiceChargeUiCommand { get; }
    public AsyncRelayCommand SetDeliveryChargeUiCommand { get; }
    public AsyncRelayCommand SetAdjustmentUiCommand { get; }
    public AsyncRelayCommand SetAdvanceUiCommand { get; }

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

    private async Task SetServiceChargeAsync()
    {
        await SetMoneyAsync(ServiceChargeText, async money =>
        {
            var result = await _setServiceCharge.HandleAsync(new SetServiceChargeCommand
            {
                TicketId = RequireTicketId(),
                Amount = money,
                ProcessedBy = new UserId(RequireProcessedBy())
            });
            if (!result.Success) throw new Exception(result.ErrorMessage);
        });
    }

    private async Task SetDeliveryChargeAsync()
    {
        await SetMoneyAsync(DeliveryChargeText, async money =>
        {
            var result = await _setDeliveryCharge.HandleAsync(new SetDeliveryChargeCommand
            {
                TicketId = RequireTicketId(),
                Amount = money,
                ProcessedBy = new UserId(RequireProcessedBy())
            });
            if (!result.Success) throw new Exception(result.ErrorMessage);
        });
    }

    private async Task SetAdjustmentAsync()
    {
        await SetMoneyAsync(AdjustmentText, async money =>
        {
            var result = await _setAdjustment.HandleAsync(new SetAdjustmentCommand
            {
                TicketId = RequireTicketId(),
                Amount = money,
                ProcessedBy = new UserId(RequireProcessedBy()),
                Reason = AdjustmentReason
            });
            if (!result.Success) throw new Exception(result.ErrorMessage);
        });
    }

    private async Task SetAdvanceAsync()
    {
        await SetMoneyAsync(AdvanceText, async money =>
        {
            var result = await _setAdvance.HandleAsync(new SetAdvancePaymentCommand
            {
                TicketId = RequireTicketId(),
                Amount = money,
                ProcessedBy = new UserId(RequireProcessedBy())
            });
            if (!result.Success) throw new Exception(result.ErrorMessage);
        });
    }

    private async Task SetMoneyAsync(string amountText, Func<Money, Task> action)
    {
        Error = null;
        IsBusy = true;
        try
        {
            if (!decimal.TryParse(amountText, out var amount)) { Error = "Invalid amount."; return; }
            await action(new Money(amount));
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

    private Guid RequireTicketId()
    {
        if (Ticket == null) throw new Exception("Load a ticket first.");
        return Ticket.Id;
    }

    private Guid RequireProcessedBy()
    {
        if (!Guid.TryParse(ProcessedByText, out var userId)) throw new Exception("Invalid ProcessedBy.");
        return userId;
    }
}

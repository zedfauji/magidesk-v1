using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.ViewModels;

public class SplitTicketViewModel : ViewModelBase
{
    private readonly ICommandHandler<SplitTicketCommand, SplitTicketResult> _splitTicketHandler;
    private readonly ICommandHandler<SplitBySeatCommand, SplitBySeatResult> _splitBySeatHandler;
    private readonly IUserService _userService;
    private readonly ITerminalContext _terminalContext;
    private readonly ICashSessionRepository _cashSessionRepository;

    private TicketDto _originalTicket;
    public TicketDto OriginalTicket
    {
        get => _originalTicket;
        set => SetProperty(ref _originalTicket, value);
    }

    private ObservableCollection<OrderLineDto> _originalItems;
    public ObservableCollection<OrderLineDto> OriginalItems
    {
        get => _originalItems;
        set => SetProperty(ref _originalItems, value);
    }

    private ObservableCollection<OrderLineDto> _splitItems;
    public ObservableCollection<OrderLineDto> SplitItems
    {
        get => _splitItems;
        set => SetProperty(ref _splitItems, value);
    }

    // Seat mode properties
    private bool _splitByItemsMode = true;
    public bool SplitByItemsMode
    {
        get => _splitByItemsMode;
        set 
        { 
            if (SetProperty(ref _splitByItemsMode, value))
            {
                if (value)
                {
                    SplitEvenlyMode = false;
                    SplitByAmountMode = false;
                    SplitBySeatMode = false;
                }
                UpdateSeatGroups();
            }
        }
    }

    private bool _splitEvenlyMode;
    public bool SplitEvenlyMode
    {
        get => _splitEvenlyMode;
        set
        {
            if (SetProperty(ref _splitEvenlyMode, value))
            {
                if (value)
                {
                    SplitByItemsMode = false;
                    SplitByAmountMode = false;
                    SplitBySeatMode = false;
                }
            }
        }
    }

    private bool _splitByAmountMode;
    public bool SplitByAmountMode
    {
        get => _splitByAmountMode;
        set
        {
            if (SetProperty(ref _splitByAmountMode, value))
            {
                if (value)
                {
                    SplitByItemsMode = false;
                    SplitEvenlyMode = false;
                    SplitBySeatMode = false;
                }
            }
        }
    }

    private bool _splitBySeatMode;
    public bool SplitBySeatMode
    {
        get => _splitBySeatMode;
        set 
        { 
            if (SetProperty(ref _splitBySeatMode, value))
            {
                if (value)
                {
                    SplitByItemsMode = false;
                    SplitEvenlyMode = false;
                    SplitByAmountMode = false;
                }
                UpdateSeatGroups();
            }
        }
    }

    private decimal _splitAmountInput;
    public decimal SplitAmountInput
    {
        get => _splitAmountInput;
        set => SetProperty(ref _splitAmountInput, value);
    }

    private ObservableCollection<decimal> _splitAmounts;
    public ObservableCollection<decimal> SplitAmounts
    {
        get => _splitAmounts;
        set => SetProperty(ref _splitAmounts, value);
    }

    public decimal SplitAmountsTotal => SplitAmounts?.Sum() ?? 0m;
    public decimal SplitAmountsRemainder => TotalAmountOriginal - SplitAmountsTotal;

    private ObservableCollection<SeatGroupDto> _seatGroups;
    public ObservableCollection<SeatGroupDto> SeatGroups
    {
        get => _seatGroups;
        set => SetProperty(ref _seatGroups, value);
    }

    private bool _hasUnassignedItems;
    public bool HasUnassignedItems
    {
        get => _hasUnassignedItems;
        set => SetProperty(ref _hasUnassignedItems, value);
    }

    private bool _hasSingleSeat;
    public bool HasSingleSeat
    {
        get => _hasSingleSeat;
        set => SetProperty(ref _hasSingleSeat, value);
    }

    public decimal TotalAmountOriginal => OriginalItems?.Sum(i => i.TotalAmount) ?? 0;
    public decimal TotalAmountSplit => SplitItems?.Sum(i => i.TotalAmount) ?? 0;

    private string _errorMessage;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private bool _hasError;
    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    public ICommand MoveToSplitCommand { get; }
    public ICommand MoveToOriginalCommand { get; }
    public ICommand ConfirmSplitCommand { get; }

    public ICommand AddSplitAmountCommand { get; }
    public ICommand RemoveSplitAmountCommand { get; }

    public SplitTicketViewModel(
        ICommandHandler<SplitTicketCommand, SplitTicketResult> splitTicketHandler,
        ICommandHandler<SplitBySeatCommand, SplitBySeatResult> splitBySeatHandler,
        IUserService userService,
        ITerminalContext terminalContext,
        ICashSessionRepository cashSessionRepository)
    {
        _splitTicketHandler = splitTicketHandler;
        _splitBySeatHandler = splitBySeatHandler;
        _userService = userService;
        _terminalContext = terminalContext;
        _cashSessionRepository = cashSessionRepository;

        SplitItems = new ObservableCollection<OrderLineDto>();
        OriginalItems = new ObservableCollection<OrderLineDto>();
        SeatGroups = new ObservableCollection<SeatGroupDto>();
        SplitAmounts = new ObservableCollection<decimal>();

        MoveToSplitCommand = new RelayCommand<object?>(MoveToSplit);
        MoveToOriginalCommand = new RelayCommand<object?>(MoveToOriginal);
        ConfirmSplitCommand = new AsyncRelayCommand<object?>(ConfirmSplitAsync);

        AddSplitAmountCommand = new RelayCommand(AddSplitAmount);
        RemoveSplitAmountCommand = new RelayCommand<object?>(RemoveSplitAmount);
    }

    public void Initialize(TicketDto ticket)
    {
        OriginalTicket = ticket;
        // Clone the list so we can manipulate it in UI without affecting the source until confirmed
        OriginalItems = new ObservableCollection<OrderLineDto>(ticket.OrderLines);
        SplitItems.Clear();
        SplitAmounts.Clear();
        SplitAmountInput = 0m;
        ErrorMessage = string.Empty;
        HasError = false;
        
        UpdateSeatGroups();
        RaiseTotalsChanged();
    }

    private void AddSplitAmount()
    {
        HasError = false;
        ErrorMessage = string.Empty;

        if (SplitAmountInput <= 0)
        {
            HasError = true;
            ErrorMessage = "Amount must be greater than 0.";
            return;
        }

        if (SplitAmountInput > SplitAmountsRemainder)
        {
            HasError = true;
            ErrorMessage = "Amount exceeds remaining balance.";
            return;
        }

        SplitAmounts.Add(decimal.Round(SplitAmountInput, 2));
        SplitAmountInput = 0m;
        OnPropertyChanged(nameof(SplitAmountsTotal));
        OnPropertyChanged(nameof(SplitAmountsRemainder));
    }

    private void RemoveSplitAmount(object? parameter)
    {
        if (parameter is decimal amount)
        {
            SplitAmounts.Remove(amount);
            OnPropertyChanged(nameof(SplitAmountsTotal));
            OnPropertyChanged(nameof(SplitAmountsRemainder));
        }
    }

    private void UpdateSeatGroups()
    {
        if (!SplitBySeatMode)
        {
            SeatGroups.Clear();
            return;
        }

        var seatGroups = OriginalItems
            .GroupBy(item => item.SeatNumber ?? 0)
            .OrderBy(g => g.Key)
            .Select(g => new SeatGroupDto
            {
                SeatNumber = g.Key,
                SeatDisplay = g.Key == 0 ? "Unassigned" : $"Seat {g.Key}",
                Items = new ObservableCollection<OrderLineDto>(g),
                TotalAmount = g.Sum(item => item.TotalAmount)
            })
            .ToList();

        SeatGroups = new ObservableCollection<SeatGroupDto>(seatGroups);

        HasUnassignedItems = seatGroups.Any(g => g.SeatNumber == 0);
        HasSingleSeat = seatGroups.Count <= 1;
    }

    private void MoveToSplit(object? parameter)
    {
        if (parameter is IList<object> selectedItems)
        {
            var itemsToMove = selectedItems.Cast<OrderLineDto>().ToList();
            foreach (var item in itemsToMove)
            {
                OriginalItems.Remove(item);
                SplitItems.Add(item);
            }
            RaiseTotalsChanged();
            UpdateSeatGroups();
        }
    }

    private void MoveToOriginal(object? parameter)
    {
        if (parameter is IList<object> selectedItems)
        {
            var itemsToMove = selectedItems.Cast<OrderLineDto>().ToList();
            foreach (var item in itemsToMove)
            {
                SplitItems.Remove(item);
                OriginalItems.Add(item);
            }
            RaiseTotalsChanged();
            UpdateSeatGroups();
        }
    }

    private void RaiseTotalsChanged()
    {
        OnPropertyChanged(nameof(TotalAmountOriginal));
        OnPropertyChanged(nameof(TotalAmountSplit));
    }

    private async Task ConfirmSplitAsync(object? parameter)
    {
        ErrorMessage = string.Empty;
        HasError = false;

        if (SplitBySeatMode)
        {
            await ConfirmSplitBySeatAsync(parameter);
        }
        else if (SplitByAmountMode)
        {
            await ConfirmSplitByAmountAsync(parameter);
        }
        else if (SplitEvenlyMode)
        {
            // F-0048: Split Evenly is a financial split
            ErrorMessage = "Split Evenly is a Payment function. Please use the Settle screen.";
            HasError = true;
        }
        else
        {
            await ConfirmSplitByItemsAsync(parameter);
        }
    }

    private Task ConfirmSplitByAmountAsync(object? parameter)
    {
        if (SplitAmounts.Count == 0)
        {
            ErrorMessage = "Enter at least one amount to split.";
            HasError = true;
            return Task.CompletedTask;
        }

        if (SplitAmounts.Any(a => a <= 0))
        {
            ErrorMessage = "All split amounts must be greater than 0.";
            HasError = true;
            return Task.CompletedTask;
        }

        if (SplitAmountsTotal > TotalAmountOriginal)
        {
            ErrorMessage = "Split amounts exceed the ticket total.";
            HasError = true;
            return Task.CompletedTask;
        }

        // F-0049: Split by Amount is a financial split (Split Tender)
        ErrorMessage = "Split by Amount is a Payment function. Please use the Settle screen.";
        HasError = true;
        return Task.CompletedTask;
    }

    private async Task ConfirmSplitByItemsAsync(object? parameter)
    {
        if (SplitItems.Count == 0)
        {
            ErrorMessage = "No items selected for the new ticket.";
            HasError = true;
            return;
        }

        if (OriginalItems.Count == 0)
        {
            ErrorMessage = "Cannot move all items. Access 'Transfer Ticket' instead if moving everything (Not yet implemented).";
            HasError = true;
            return;
        }

        try
        {
            var currentUser = _userService.CurrentUser;
            if (currentUser == null)
            {
                ErrorMessage = "No user logged in.";
                HasError = true;
                return;
            }

            if (_terminalContext.TerminalId == null)
            {
               ErrorMessage = "Terminal not initialized.";
               HasError = true;
               return;
            }

            var terminalId = _terminalContext.TerminalId.Value;
            // Resolve Shift (Active Session)
            var session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
            if (session == null)
            {
                ErrorMessage = "No active cash session found. Please start a shift.";
                HasError = true;
                return;
            }

            var command = new SplitTicketCommand
            {
                OriginalTicketId = OriginalTicket.Id,
                OrderLineIdsToSplit = SplitItems.Select(x => x.Id).ToList(),
                SplitBy = new UserId(currentUser.Id),
                TerminalId = terminalId,
                ShiftId = session.Id,
                OrderTypeId = OriginalTicket.OrderTypeId,
                GlobalId = Guid.NewGuid().ToString()
            };

            var result = await _splitTicketHandler.HandleAsync(command);

            if (result.Success)
            {
                if (parameter is ContentDialog dialog)
                {
                    dialog.Hide();
                }
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Unknown error during split.";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    private async Task ConfirmSplitBySeatAsync(object? parameter)
    {
        if (HasSingleSeat)
        {
            ErrorMessage = "Cannot split: only one seat or all items unassigned.";
            HasError = true;
            return;
        }

        try
        {
            var currentUser = _userService.CurrentUser;
            if (currentUser == null)
            {
                ErrorMessage = "No user logged in.";
                HasError = true;
                return;
            }

            if (_terminalContext.TerminalId == null)
            {
               ErrorMessage = "Terminal not initialized.";
               HasError = true;
               return;
            }

            var terminalId = _terminalContext.TerminalId.Value;
            // Resolve Shift (Active Session)
            var session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
            if (session == null)
            {
                ErrorMessage = "No active cash session found. Please start a shift.";
                HasError = true;
                return;
            }

            var command = new SplitBySeatCommand
            {
                OriginalTicketId = OriginalTicket.Id,
                ProcessedBy = new UserId(currentUser.Id),
                TerminalId = terminalId,
                GlobalId = Guid.NewGuid().ToString()
            };

            var result = await _splitBySeatHandler.HandleAsync(command);

            if (result.Success)
            {
                if (parameter is ContentDialog dialog)
                {
                    dialog.Hide();
                }
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Unknown error during split by seat.";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }
}

public class SeatGroupDto
{
    public int SeatNumber { get; set; }
    public string SeatDisplay { get; set; } = string.Empty;
    public ObservableCollection<OrderLineDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
}

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
    private readonly IUserService _userService;

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

    public SplitTicketViewModel(
        ICommandHandler<SplitTicketCommand, SplitTicketResult> splitTicketHandler,
        IUserService userService)
    {
        _splitTicketHandler = splitTicketHandler;
        _userService = userService;

        SplitItems = new ObservableCollection<OrderLineDto>();
        OriginalItems = new ObservableCollection<OrderLineDto>();

        MoveToSplitCommand = new RelayCommand<object?>(MoveToSplit);
        MoveToOriginalCommand = new RelayCommand<object?>(MoveToOriginal);
        ConfirmSplitCommand = new AsyncRelayCommand<object?>(ConfirmSplitAsync);
    }

    public void Initialize(TicketDto ticket)
    {
        OriginalTicket = ticket;
        // Clone the list so we can manipulate it in UI without affecting the source until confirmed
        OriginalItems = new ObservableCollection<OrderLineDto>(ticket.OrderLines);
        SplitItems.Clear();
        ErrorMessage = string.Empty;
        HasError = false;
        
        RaiseTotalsChanged();
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

            var command = new SplitTicketCommand
            {
                OriginalTicketId = OriginalTicket.Id,
                OrderLineIdsToSplit = SplitItems.Select(x => x.Id).ToList(),
                SplitBy = new UserId(currentUser.Id),
                TerminalId = Guid.Empty, // TODO: Get actual Terminal ID
                ShiftId = Guid.Empty,    // TODO: Get actual Shift ID
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
}

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Magidesk.Presentation.ViewModels;

public class SplitTicketViewModel : ViewModelBase
{
    private readonly ICommandHandler<SplitTicketCommand, SplitTicketResult> _splitTicket;
    private TicketDto _originalTicket = null!;
    
    public ObservableCollection<OrderLineDto> OriginalOrderLines { get; } = new();
    public ObservableCollection<OrderLineDto> NewOrderLines { get; } = new();
    
    private OrderLineDto? _selectedOriginalLine;
    public OrderLineDto? SelectedOriginalLine
    {
        get => _selectedOriginalLine;
        set
        {
            if (SetProperty(ref _selectedOriginalLine, value))
            {
                 MoveRightCommand.RaiseCanExecuteChanged();
            }
        }
    }

    private OrderLineDto? _selectedNewLine;
    public OrderLineDto? SelectedNewLine
    {
        get => _selectedNewLine;
        set
        {
            if (SetProperty(ref _selectedNewLine, value))
            {
                MoveLeftCommand.RaiseCanExecuteChanged();
            }
        }
    }
    
    public RelayCommand MoveRightCommand { get; }
    public RelayCommand MoveLeftCommand { get; }

    public SplitTicketViewModel(
        ICommandHandler<SplitTicketCommand, SplitTicketResult> splitTicket)
    {
        _splitTicket = splitTicket;
        
        MoveRightCommand = new RelayCommand(MoveRight, () => SelectedOriginalLine != null);
        MoveLeftCommand = new RelayCommand(MoveLeft, () => SelectedNewLine != null);
    }

    public void Initialize(TicketDto ticket)
    {
        _originalTicket = ticket;
        OriginalOrderLines.Clear();
        NewOrderLines.Clear();

        foreach (var line in ticket.OrderLines)
        {
            OriginalOrderLines.Add(line);
        }
    }

    private void MoveRight()
    {
        if (SelectedOriginalLine == null) return;
        
        var item = SelectedOriginalLine;
        OriginalOrderLines.Remove(item);
        NewOrderLines.Add(item);
        
        SelectedOriginalLine = null;
    }

    private void MoveLeft()
    {
        if (SelectedNewLine == null) return;
        
        var item = SelectedNewLine;
        NewOrderLines.Remove(item);
        OriginalOrderLines.Add(item);
        
        SelectedNewLine = null;
    }
    
    public async Task<SplitTicketResult> ExecuteSplitAsync(UserId splitBy)
    {
        if (NewOrderLines.Count == 0)
        {
             return new SplitTicketResult { Success = false, ErrorMessage = "No items selected to split." };
        }

        var command = new SplitTicketCommand
        {
            OriginalTicketId = _originalTicket.Id,
            OrderLineIdsToSplit = NewOrderLines.Select(x => x.Id).ToList(),
            SplitBy = splitBy,
            TerminalId = Guid.Parse("00000000-0000-0000-0000-000000000001"), // TODO: proper terminal context
            ShiftId = Guid.Parse("00000000-0000-0000-0000-000000000001"),    // TODO: proper shift context
            OrderTypeId = _originalTicket.OrderTypeId
        };

        return await _splitTicket.HandleAsync(command);
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Magidesk.Presentation.ViewModels.Dialogs;

public partial class MergeTicketsViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetOpenTicketsQuery, IEnumerable<TicketDto>> _getOpenTicketsHandler;
    private readonly Guid _sourceTicketId;

    private ObservableCollection<TicketDto> _availableTickets = new();
    public ObservableCollection<TicketDto> AvailableTickets
    {
        get => _availableTickets;
        set => SetProperty(ref _availableTickets, value);
    }

    private TicketDto? _selectedTargetTicket;
    public TicketDto? SelectedTargetTicket
    {
        get => _selectedTargetTicket;
        set => SetProperty(ref _selectedTargetTicket, value);
    }

    public Action? CloseAction { get; set; }
    public bool IsConfirmed { get; private set; }

    public MergeTicketsViewModel(
        IQueryHandler<GetOpenTicketsQuery, IEnumerable<TicketDto>> getOpenTicketsHandler,
        Guid sourceTicketId)
    {
        _getOpenTicketsHandler = getOpenTicketsHandler;
        _sourceTicketId = sourceTicketId;
        _ = LoadTicketsAsync();
    }

    private async Task LoadTicketsAsync()
    {
        IsBusy = true;
        try
        {
            var tickets = await _getOpenTicketsHandler.HandleAsync(new GetOpenTicketsQuery());
            
            // Filter out source ticket
            var validTargets = tickets.Where(t => t.Id != _sourceTicketId).ToList();
            
            AvailableTickets = new ObservableCollection<TicketDto>(validTargets);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Confirm()
    {
        if (SelectedTargetTicket != null)
        {
            IsConfirmed = true;
            CloseAction?.Invoke();
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        IsConfirmed = false;
        CloseAction?.Invoke();
    }
}

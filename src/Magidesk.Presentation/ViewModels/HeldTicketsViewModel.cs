using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// ViewModel for managing held tickets.
/// Displays all tickets that are currently on hold and allows releasing them.
/// </summary>
public partial class HeldTicketsViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetHeldTicketsQuery, IEnumerable<HeldTicketDto>> _getHeldTickets;
    private readonly ICommandHandler<ReleaseHeldTicketCommand> _releaseTicketHandler;
    private readonly IUserService _userService;
    private readonly Services.NavigationService _navigationService;
    private readonly ILogger<HeldTicketsViewModel> _logger;

    [ObservableProperty]
    private ObservableCollection<HeldTicketDto> _heldTickets = new();

    [ObservableProperty]
    private HeldTicketDto? _selectedTicket;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private string? _statusMessage;

    public HeldTicketsViewModel(
        IQueryHandler<GetHeldTicketsQuery, IEnumerable<HeldTicketDto>> getHeldTickets,
        ICommandHandler<ReleaseHeldTicketCommand> releaseTicketHandler,
        IUserService userService,
        Services.NavigationService navigationService,
        ILogger<HeldTicketsViewModel> logger)
    {
        _getHeldTickets = getHeldTickets;
        _releaseTicketHandler = releaseTicketHandler;
        _userService = userService;
        _navigationService = navigationService;
        _logger = logger;

        Title = "Held Tickets";

        ReleaseTicketCommand = new AsyncRelayCommand<HeldTicketDto>(ReleaseTicketAsync, CanReleaseTicket);
        ViewTicketDetailsCommand = new RelayCommand<HeldTicketDto>(ViewTicketDetails, ticket => ticket != null);
    }

    public AsyncRelayCommand<HeldTicketDto> ReleaseTicketCommand { get; }
    public RelayCommand<HeldTicketDto> ViewTicketDetailsCommand { get; }

    /// <summary>
    /// Initializes the view model by loading held tickets.
    /// </summary>
    public async Task InitializeAsync()
    {
        await RefreshAsync();
    }

    /// <summary>
    /// Refreshes the list of held tickets.
    /// </summary>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var tickets = await _getHeldTickets.HandleAsync(new GetHeldTicketsQuery());
            
            HeldTickets.Clear();
            foreach (var ticket in tickets.OrderByDescending(t => t.HeldAt))
            {
                HeldTickets.Add(ticket);
            }

            StatusMessage = $"Loaded {HeldTickets.Count} held ticket(s)";
            _logger.LogInformation("Loaded {Count} held tickets", HeldTickets.Count);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load held tickets: {ex.Message}";
            _logger.LogError(ex, "Error loading held tickets");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Determines if a ticket can be released.
    /// </summary>
    private bool CanReleaseTicket(HeldTicketDto? ticket)
    {
        return ticket != null && _userService.CurrentUser != null;
    }

    /// <summary>
    /// Releases a held ticket, making it available again.
    /// </summary>
    private async Task ReleaseTicketAsync(HeldTicketDto? ticket)
    {
        if (ticket == null || _userService.CurrentUser == null)
        {
            return;
        }

        IsLoading = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var command = new ReleaseHeldTicketCommand(
                ticket.Id,
                new UserId(_userService.CurrentUser.Id)
            );

            await _releaseTicketHandler.HandleAsync(command);

            StatusMessage = $"Ticket #{ticket.TicketNumber} released successfully";
            _logger.LogInformation("Released held ticket {TicketId}", ticket.Id);

            // Refresh the list to remove the released ticket
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to release ticket: {ex.Message}";
            _logger.LogError(ex, "Error releasing ticket {TicketId}", ticket.Id);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Navigates to the ticket details view.
    /// </summary>
    private void ViewTicketDetails(HeldTicketDto? ticket)
    {
        if (ticket == null)
        {
            return;
        }

        try
        {
            // Navigate to settle page to view ticket details
            _navigationService.Navigate(
                typeof(Magidesk.Presentation.Views.SettlePage),
                ticket.Id
            );
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to view ticket details: {ex.Message}";
            _logger.LogError(ex, "Error navigating to ticket details for {TicketId}", ticket.Id);
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.Views;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.Views;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using System.Collections.ObjectModel;

using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for session management operations.
/// Handles table session lifecycle: start, pause, resume, and end.
/// </summary>
public partial class OrderPageViewModel
{
    private async Task OnToggleSessionAsync()
    {
        try
        {
            _logger.LogInformation("Toggle session requested - Current state: {State}", CurrentSessionState);

            switch (CurrentSessionState)
            {
                case SessionState.NotStarted:
                    await StartTableSessionAsync();
                    break;
                case SessionState.Active:
                    await PauseTableSessionAsync();
                    break;
                case SessionState.Paused:
                    await ResumeTableSessionAsync();
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to toggle session");
            await _dialogService.ShowErrorAsync("Error", $"Failed to toggle session: {ex.Message}");
        }
    }

    private async Task OnStartSessionAsync()
    {
        // Navigate to dashboard or show dialog to start session
        // For now, guide user to dashboard as session management is there
        await _dialogService.ShowMessageAsync(
            "Start Session",
            "Please navigate to the Dashboard to start a new shift/session.");

        // Ideally navigate there:
        // _navigationService.Navigate(typeof(Views.DashboardPage));
    }

    private async Task StartTableSessionAsync()
    {
        try
        {
            _logger.LogInformation("Start table session requested - _tableId: {TableId}, _ticketId: {TicketId}", _tableId, _ticketId);

            if (!_tableId.HasValue)
            {
                _logger.LogWarning("Cannot start session: no table selected");
                await _dialogService.ShowWarningAsync(
                    "No Table Selected",
                    "Please select a table before starting a session.");
                return;
            }

            if (!_ticketId.HasValue)
            {
                _logger.LogWarning("Cannot start session: no ticket");
                await _dialogService.ShowWarningAsync(
                    "No Ticket",
                    "Please create an order before starting a session.");
                return;
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tableRepository = scope.ServiceProvider.GetRequiredService<ITableRepository>();
                var tableTypeRepository = scope.ServiceProvider.GetRequiredService<ITableTypeRepository>();

                // Get table information
                var table = await tableRepository.GetByIdAsync(_tableId.Value);
                if (table == null)
                {
                    _logger.LogError("Table {TableId} not found", _tableId);
                    await _dialogService.ShowErrorAsync(
                        "Table Not Found",
                        "The selected table could not be found.");
                    return;
                }

                // Check if table has a type assigned
                if (!table.TableTypeId.HasValue)
                {
                    _logger.LogError("Table {TableId} has no table type assigned", _tableId);
                    await _dialogService.ShowErrorAsync(
                        "Configuration Error",
                        "The selected table does not have a Table Type assigned. Please configure it in Settings.");
                    return;
                }

                // Get table type for hourly rate
                var tableType = await tableTypeRepository.GetByIdAsync(table.TableTypeId.Value);
                if (tableType == null)
                {
                    _logger.LogError("Table type {TableTypeId} not found", table.TableTypeId);
                    await _dialogService.ShowErrorAsync(
                        "Configuration Error",
                        "Table type configuration is missing.");
                    return;
                }

                var command = new StartTableSessionCommand(
                    TableId: _tableId.Value,
                    TableTypeId: table.TableTypeId.Value,
                    GuestCount: GuestCount > 0 ? GuestCount : 1,
                    CustomerId: null,
                    TicketId: _ticketId.Value,
                    CreateTicket: false,
                    UserId: _userContextService.GetCurrentUserId() != Guid.Empty ? _userContextService.GetCurrentUserId() : null,
                    TerminalId: _terminalContext.TerminalId,
                    ShiftId: null,
                    OrderTypeId: null
                );

                var result = await _startTableSessionHandler.HandleAsync(command);

                _logger.LogInformation("Table session {SessionId} started for table {TableId}",
                    result.SessionId, _tableId);

                // Reload ticket to get updated session information
                await LoadTicketAsync();

                await _dialogService.ShowMessageAsync(
                    "Session Started",
                    $"Table session has been started.\n\nHourly Rate: {result.HourlyRate:C2}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start table session");
            await _dialogService.ShowErrorAsync("Error", $"Failed to start session: {ex.Message}");
        }
    }

    private async Task PauseTableSessionAsync()
    {
        try
        {
            _logger.LogInformation("Pause table session requested");

            if (_ticket?.SessionId == null)
            {
                _logger.LogWarning("Cannot pause session: no active session");
                return;
            }

            var command = new PauseTableSessionCommand(_ticket.SessionId.Value);
            var result = await _pauseTableSessionHandler.HandleAsync(command);

            _logger.LogInformation("Table session {SessionId} paused at {PausedAt}",
                result.SessionId, result.PausedAt);

            // Reload ticket to get updated session status
            await LoadTicketAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to pause table session");
            await _dialogService.ShowErrorAsync("Error", $"Failed to pause session: {ex.Message}");
        }
    }

    private async Task ResumeTableSessionAsync()
    {
        try
        {
            _logger.LogInformation("Resume table session requested");

            if (_ticket?.SessionId == null)
            {
                _logger.LogWarning("Cannot resume session: no paused session");
                return;
            }

            var command = new ResumeTableSessionCommand(_ticket.SessionId.Value);
            var result = await _resumeTableSessionHandler.HandleAsync(command);

            _logger.LogInformation("Table session {SessionId} resumed at {ResumedAt}, total paused: {TotalPaused}",
                result.SessionId, result.ResumedAt, result.TotalPausedDuration);

            // Reload ticket to get updated session status
            await LoadTicketAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resume table session");
            await _dialogService.ShowErrorAsync("Error", $"Failed to resume session: {ex.Message}");
        }
    }

    private async Task OnEndSessionAsync()
    {
        try
        {
            _logger.LogInformation("End table session requested");

            if (_ticket?.SessionId == null)
            {
                _logger.LogWarning("Cannot end session: no active session");
                await _dialogService.ShowWarningAsync(
                    "No Active Session",
                    "There is no active session to end.");
                return;
            }

            // Confirm session end
            var confirmed = await _dialogService.ShowConfirmationAsync(
                "End Session",
                $"End the current table session?\n\nSession Duration: {SessionDurationDisplay}\n\nThis will add the session charges to the order.",
                "End Session", "Cancel");

            if (!confirmed)
            {
                return;
            }

            var command = new EndTableSessionCommand(
                SessionId: _ticket.SessionId.Value,
                CreateTicket: false, // Add to existing ticket
                UserId: _userContextService.GetCurrentUserId() != Guid.Empty ? _userContextService.GetCurrentUserId() : null,
                TerminalId: _terminalContext.TerminalId,
                ShiftId: null,
                OrderTypeId: null
            );

            var result = await _endTableSessionHandler.HandleAsync(command);

            _logger.LogInformation("Table session {SessionId} ended", _ticket.SessionId);

            // Reload ticket to get updated totals with session charges
            await LoadTicketAsync();

            await _dialogService.ShowMessageAsync(
                "Session Ended",
                $"Table session has been ended.\n\nSession charges have been added to the order.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to end table session");
            await _dialogService.ShowErrorAsync("Error", $"Failed to end session: {ex.Message}");
        }
    }
}

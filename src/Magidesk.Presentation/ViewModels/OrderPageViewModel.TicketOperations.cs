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

using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for ticket operations.
/// Handles splitting, merging, and note management for orders.
/// </summary>
public partial class OrderPageViewModel
{
    private async Task OnSplitOrderAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot split order: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "Please create an order before splitting.");
            return;
        }

        if (OrderItems.Count < 2)
        {
            _logger.LogWarning("Cannot split order: insufficient items");
            await _dialogService.ShowWarningAsync(
                "Insufficient Items",
                "You need at least 2 items to split an order.");
            return;
        }

        try
        {
            _logger.LogInformation("Split order requested for ticket {TicketId}", _ticketId);
            // TODO: Implement split order dialog and logic
            await _dialogService.ShowMessageAsync(
                "Split Order",
                "Split order feature coming soon.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to split order");
            await _dialogService.ShowErrorAsync("Error", $"Failed to split order: {ex.Message}");
        }
    }

    private async Task OnMergeOrderAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot merge order: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "Please create an order before merging.");
            return;
        }

        try
        {
            _logger.LogInformation("Merge order requested for ticket {TicketId}", _ticketId);
            // TODO: Implement merge order dialog and logic
            await _dialogService.ShowMessageAsync(
                "Merge Order",
                "Merge order feature coming soon.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to merge order");
            await _dialogService.ShowErrorAsync("Error", $"Failed to merge order: {ex.Message}");
        }
    }

    private async Task OnAddNoteAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot add note: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "Please create an order before adding notes.");
            return;
        }

        try
        {
            _logger.LogInformation("Add note requested for ticket {TicketId}", _ticketId);
            // TODO: Implement add note dialog
            await _dialogService.ShowMessageAsync(
                "Add Note",
                "Add note feature coming soon.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add note");
            await _dialogService.ShowErrorAsync("Error", $"Failed to add note: {ex.Message}");
        }
    }
}

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
/// Partial class for payment and navigation operations.
/// Handles printing, navigation to settle page, and payment initiation.
/// </summary>
public partial class OrderPageViewModel
{
    private async Task OnPrintOrderAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot print order: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "Please create an order before printing.");
            return;
        }

        if (OrderItems.Count == 0)
        {
            _logger.LogWarning("Cannot print order: no items in order");
            await _dialogService.ShowWarningAsync(
                "Empty Order",
                "Please add items to the order before printing.");
            return;
        }

        try
        {
            _logger.LogInformation("Print order requested for ticket {TicketId}", _ticketId);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var printToKitchenHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<PrintToKitchenCommand, PrintToKitchenResult>>();

                var command = new PrintToKitchenCommand
                {
                    TicketId = _ticketId.Value
                };

                var result = await printToKitchenHandler.HandleAsync(command);

                if (result.Success)
                {
                    _logger.LogInformation("Order ticket printed for ticket {TicketId}", _ticketId);

                    await _dialogService.ShowMessageAsync(
                        "Order Printed",
                        $"Order ticket has been printed.\n\nTicket #{_ticket?.TicketNumber}");
                }
                else
                {
                    var errorMsg = result.Errors.Any() ? string.Join("\n", result.Errors) : result.Message;
                    _logger.LogError("Failed to print order: {Error}", errorMsg);
                    await _dialogService.ShowErrorAsync(
                        "Print Error",
                        $"Failed to print order:\n\n{errorMsg}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print order for ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to print order: {ex.Message}");
        }
    }

    private async Task OnNavigateToSettleAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot navigate to settle: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "Please add items to the order before settling.");
            return;
        }

        // Check if there are any items in the order
        if (OrderItems.Count == 0)
        {
            _logger.LogWarning("Cannot navigate to settle: no items in order");
            await _dialogService.ShowWarningAsync(
                "Empty Order",
                "Please add items to the order before settling.");
            return;
        }

        try
        {
            _logger.LogInformation("Navigating to settle page for ticket {TicketId}", _ticketId);

            // Navigate to settle page with ticket ID
            _navigationService.Navigate(typeof(Views.SettlePageView), _ticketId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to navigate to settle page");
            await _dialogService.ShowErrorAsync(
                "Navigation Error",
                $"Failed to open settle page: {ex.Message}");
        }
    }

    private async Task OnPayNowAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot pay now: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "Please add items to the order before processing payment.");
            return;
        }

        if (OrderItems.Count == 0)
        {
            _logger.LogWarning("Cannot pay now: no items in order");
            await _dialogService.ShowWarningAsync(
                "Empty Order",
                "Please add items to the order before processing payment.");
            return;
        }

        try
        {
            _logger.LogInformation("Pay now requested for ticket {TicketId}", _ticketId);

            // Quick payment flow - navigate directly to settle page
            _navigationService.Navigate(typeof(Views.SettlePageView), _ticketId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initiate payment for ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to initiate payment: {ex.Message}");
        }
    }
}

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
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for discount and kitchen operations.
/// Handles applying discounts and sending orders to kitchen.
/// </summary>
public partial class OrderPageViewModel
{
    private async Task OnApplyDiscountAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot apply discount: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "Please create an order before applying a discount.");
            return;
        }

        try
        {
            _logger.LogInformation("Apply discount requested for ticket {TicketId}", _ticketId);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var discountRepository = scope.ServiceProvider.GetRequiredService<IDiscountRepository>();
                var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
                var applyDiscountHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<ApplyDiscountCommand>>();
                var userContextService = scope.ServiceProvider.GetRequiredService<IUserContextService>();
                var managerPinDialog = scope.ServiceProvider.GetRequiredService<ManagerPinDialogViewModel>();

                // Load the ticket
                var ticket = await ticketRepository.GetByIdAsync(_ticketId.Value);

                if (ticket == null)
                {
                    _logger.LogError("Ticket {TicketId} not found", _ticketId);
                    await _dialogService.ShowErrorAsync(
                        "Ticket Not Found",
                        "The ticket could not be found. It may have been deleted.");
                    return;
                }

                // Create ViewModel for discount selection dialog with all required dependencies
                var viewModel = new DiscountSelectionViewModel(
                    discountRepository,
                    applyDiscountHandler,
                    userContextService,
                    managerPinDialog);

                // Set ticket information
                viewModel.TicketId = _ticketId.Value;
                viewModel.TicketTotal = ticket.TotalAmount;

                // Load available discounts
                await viewModel.LoadDiscountsAsync();

                // Create Dialog
                var dialog = new DiscountSelectionDialog(viewModel);

                // Set XamlRoot for the dialog
                if (Microsoft.UI.Xaml.Window.Current?.Content is Microsoft.UI.Xaml.FrameworkElement element)
                {
                    dialog.XamlRoot = element.XamlRoot;
                }

                await dialog.ShowAsync();

                // If discount was applied successfully
                if (viewModel.IsSuccess)
                {
                    _logger.LogInformation("Discount applied to ticket {TicketId}", _ticketId);

                    // Reload ticket to get updated totals
                    await LoadTicketAsync();

                    await _dialogService.ShowMessageAsync(
                        "Discount Applied",
                        $"Discount has been applied to the order.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply discount to ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to apply discount: {ex.Message}");
        }
    }

    private async Task OnFireTicketAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot fire ticket: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "Please create an order before sending to kitchen.");
            return;
        }

        if (OrderItems.Count == 0)
        {
            _logger.LogWarning("Cannot fire ticket: no items in order");
            await _dialogService.ShowWarningAsync(
                "Empty Order",
                "Please add items to the order before sending to kitchen.");
            return;
        }

        try
        {
            _logger.LogInformation("Fire ticket requested for ticket {TicketId}", _ticketId);

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
                    _logger.LogInformation("Ticket {TicketId} sent to kitchen", _ticketId);

                    await _dialogService.ShowMessageAsync(
                        "Order Sent",
                        $"Order has been sent to the kitchen.\n\nTicket #{_ticket?.TicketNumber}");
                }
                else
                {
                    var errorMsg = result.Errors.Any() ? string.Join("\n", result.Errors) : result.Message;
                    _logger.LogError("Failed to fire ticket: {Error}", errorMsg);
                    await _dialogService.ShowErrorAsync(
                        "Kitchen Print Error",
                        $"Failed to send order to kitchen:\n\n{errorMsg}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fire ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to send order to kitchen: {ex.Message}");
        }
    }
}

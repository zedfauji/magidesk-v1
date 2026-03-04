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
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for administration operations.
/// Handles reprinting receipts and voiding tickets.
/// </summary>
public partial class OrderPageViewModel
{
    private async Task OnReprintAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot reprint: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "There is no active ticket to reprint.");
            return;
        }

        try
        {
            _logger.LogInformation("Reprint requested for ticket {TicketId}", _ticketId);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var printReceiptHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<PrintReceiptCommand, PrintReceiptResult>>();

                var command = new PrintReceiptCommand
                {
                    TicketId = _ticketId.Value
                };

                var result = await printReceiptHandler.HandleAsync(command);

                if (result.Success)
                {
                    _logger.LogInformation("Receipt reprinted for ticket {TicketId}", _ticketId);

                    await _dialogService.ShowMessageAsync(
                        "Receipt Reprinted",
                        $"Receipt has been reprinted.\n\nTicket #{_ticket?.TicketNumber}");
                }
                else
                {
                    _logger.LogError("Failed to reprint receipt");
                    await _dialogService.ShowErrorAsync(
                        "Print Error",
                        "Failed to reprint receipt. Please check the printer and try again.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reprint receipt for ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to reprint receipt: {ex.Message}");
        }
    }

    private async Task OnVoidTicketAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot void ticket: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "There is no active ticket to void.");
            return;
        }

        try
        {
            _logger.LogInformation("Void ticket requested for ticket {TicketId}", _ticketId);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
                var voidTicketHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<VoidTicketCommand>>();
                var userContextService = scope.ServiceProvider.GetRequiredService<IUserContextService>();

                // Load the ticket to pass to the dialog
                var ticket = await ticketRepository.GetByIdAsync(_ticketId.Value);

                if (ticket == null)
                {
                    _logger.LogError("Ticket {TicketId} not found", _ticketId);
                    await _dialogService.ShowErrorAsync(
                        "Ticket Not Found",
                        "The ticket could not be found. It may have been deleted.");
                    return;
                }

                // Convert domain ticket to DTO
                var ticketDto = new TicketDto
                {
                    Id = ticket.Id,
                    TicketNumber = ticket.TicketNumber,
                    TotalAmount = ticket.TotalAmount.Amount,
                    Status = ticket.Status
                };

                // Create ViewModel for void ticket dialog with required dependencies
                var viewModel = new VoidTicketViewModel(voidTicketHandler, userContextService);
                viewModel.Initialize(ticketDto);

                // Create Dialog
                var dialog = new VoidTicketDialog
                {
                    DataContext = viewModel
                };

                // Set XamlRoot for the dialog
                if (Microsoft.UI.Xaml.Window.Current?.Content is Microsoft.UI.Xaml.FrameworkElement element)
                {
                    dialog.XamlRoot = element.XamlRoot;
                }

                var result = await dialog.ShowAsync();

                // If the void was successful (dialog handles the void operation internally)
                if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary && !viewModel.HasError)
                {
                    _logger.LogInformation("Ticket {TicketId} voided successfully", _ticketId);

                    // Clear the current ticket and reset the page
                    _ticketId = null;
                    _ticket = null;
                    OrderItems.Clear();
                    RecalculateTotals();
                    OnPropertyChanged(nameof(TicketNumber));
                    OnPropertyChanged(nameof(HasTicket));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to void ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to void ticket: {ex.Message}");
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Presentation.Views.Dialogs;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for additional settlement operations.
/// Handles printing, tax exemption, and navigation.
/// </summary>
public partial class SettlePageViewModel
{
    private async Task OnPrintReceiptAsync()
    {
        if (_ticket == null)
        {
            _logger.LogWarning("Cannot print receipt: no ticket loaded");
            return;
        }

        try
        {
            _logger.LogInformation("Print receipt requested for ticket {TicketId}", _ticketId);
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var printReceiptHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<PrintReceiptCommand, PrintReceiptResult>>();
                
                var command = new PrintReceiptCommand
                {
                    TicketId = _ticketId
                };
                
                var result = await printReceiptHandler.HandleAsync(command);
                
                if (result.Success)
                {
                    _logger.LogInformation("Receipt printed for ticket {TicketId}", _ticketId);
                    
                    await _dialogService.ShowMessageAsync(
                        "Receipt Printed",
                        $"Receipt has been printed.\n\nTicket #{_ticket.TicketNumber}\nTotal: {_ticket.TotalAmount:C2}");
                }
                else
                {
                    _logger.LogError("Failed to print receipt for ticket {TicketId}", _ticketId);
                    await _dialogService.ShowErrorAsync(
                        "Print Error",
                        "Failed to print receipt. Please check the printer and try again.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print receipt for ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to print receipt: {ex.Message}");
        }
    }

    private async Task OnToggleTaxExemptAsync()
    {
        if (_ticket == null)
        {
            _logger.LogWarning("Cannot toggle tax exempt: no ticket loaded");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "No ticket is currently loaded. Please return to the order page and try again.");
            return;
        }

        if (_userContextService.GetCurrentUserId() == Guid.Empty)
        {
            _logger.LogError("Cannot toggle tax exempt: no user logged in");
            await _dialogService.ShowErrorAsync(
                "Authentication Error",
                "No user is currently logged in. Please log in and try again.");
            return;
        }

        try
        {
            IsBusy = true;

            var newTaxExemptStatus = !IsTaxExempt;

            var command = new SetTaxExemptCommand
            {
                TicketId = _ticket.Id,
                IsTaxExempt = newTaxExemptStatus,
                ModifiedBy = new UserId(_userContextService.GetCurrentUserId())
            };

            var result = await _setTaxExemptHandler.HandleAsync(command);

            if (result.Success)
            {
                _logger.LogInformation("Tax exempt status toggled to {Status} for ticket {TicketId}",
                    newTaxExemptStatus, _ticketId);

                // Reload ticket to get recalculated totals
                await LoadTicketAsync();
            }
            else
            {
                _logger.LogError("Failed to toggle tax exempt: {Error}", result.Error);
                await _dialogService.ShowErrorAsync(
                    "Tax Exempt Error",
                    $"Unable to change tax exempt status:\n\n{result.Error}");
            }
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while toggling tax exempt for ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync(
                "Network Error",
                "Unable to connect to the server. Please check your network connection and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to toggle tax exempt for ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync(
                "Error",
                $"An error occurred while changing tax exempt status:\n\n{ex.Message}",
                ex.ToString());
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnCancelSettlement()
    {
        // Navigate back without processing any payments
        // Ticket state is preserved (no modifications made)
        _logger.LogInformation("Settlement cancelled for ticket {TicketId}", _ticketId);
        _navigationService.GoBack();
    }

    private void OnNavigateBack()
    {
        // Navigate back to order page
        _logger.LogInformation("Navigating back from settle page for ticket {TicketId}", _ticketId);
        _navigationService.GoBack();
    }
}

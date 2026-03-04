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

using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Application.Commands;
using Magidesk.Domain.Enumerations;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for payment processing operations.
/// Handles ticket loading and payment processing.
/// </summary>
public partial class SettlePageViewModel
{
    private async Task LoadTicketAsync()
    {
        try
        {
            IsBusy = true;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var getTicketHandler = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetTicketQuery, TicketDto?>>();
                _ticket = await getTicketHandler.HandleAsync(new GetTicketQuery { TicketId = _ticketId });

                if (_ticket != null)
                {
                    PaidAmount = _ticket.PaidAmount;
                    BalanceDue = _ticket.DueAmount;
                    IsTaxExempt = _ticket.IsTaxExempt;

                    // Notify property changes
                    OnPropertyChanged(nameof(TicketNumber));
                    OnPropertyChanged(nameof(TableNumber));
                    OnPropertyChanged(nameof(TotalAmount));
                    OnPropertyChanged(nameof(TaxAmount));

                    _logger.LogInformation("Loaded ticket {TicketId} with balance due {BalanceDue}", _ticketId, BalanceDue);
                }
                else
                {
                    _logger.LogWarning("Ticket {TicketId} not found", _ticketId);
                    await _dialogService.ShowErrorAsync(
                        "Ticket Not Found",
                        $"Ticket {_ticketId} could not be found. It may have been deleted or moved.");
                }
            }
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while loading ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync(
                "Network Error",
                "Unable to connect to the server. Please check your network connection and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync(
                "Error Loading Ticket",
                $"An error occurred while loading the ticket:\n\n{ex.Message}",
                ex.ToString());
        }
        finally
        {
            IsBusy = false;
        }
    }


    private async Task ProcessPaymentAsync(PaymentType paymentType)
    {
        if (_ticket == null)
        {
            _logger.LogWarning("Cannot process payment: no ticket loaded");
            await _dialogService.ShowErrorAsync(
                "Payment Error",
                "No ticket is currently loaded. Please return to the order page and try again.");
            return;
        }

        if (_tenderAmount <= 0)
        {
            _logger.LogWarning("Cannot process payment: tender amount is zero or negative");
            await _dialogService.ShowWarningAsync(
                "Invalid Amount",
                "Please enter a tender amount greater than zero.");
            return;
        }

        if (_userContextService.GetCurrentUserId() == Guid.Empty)
        {
            _logger.LogError("Cannot process payment: no user logged in");
            await _dialogService.ShowErrorAsync(
                "Authentication Error",
                "No user is currently logged in. Please log in and try again.");
            return;
        }

        if (_terminalContext.TerminalId == null)
        {
            _logger.LogError("Cannot process payment: no terminal context");
            await _dialogService.ShowErrorAsync(
                "Terminal Error",
                "Terminal context is not available. Please restart the application.");
            return;
        }

        try
        {
            IsProcessingPayment = true;
            IsBusy = true;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var processPaymentHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult>>();
                var cashSessionRepository = scope.ServiceProvider.GetRequiredService<ICashSessionRepository>();

                var userId = _userContextService.GetCurrentUserId();
                var terminalId = _terminalContext.TerminalId.Value;
                var currency = "USD"; // Default currency

                // Determine amount to pay (handle partial payments)
                var amountToPay = _tenderAmount >= BalanceDue ? BalanceDue : _tenderAmount;

                var command = new ProcessPaymentCommand
                {
                    TicketId = _ticket.Id,
                    PaymentType = paymentType,
                    Amount = new Money(amountToPay, currency),
                    ProcessedBy = new UserId(userId),
                    TerminalId = terminalId,
                    GlobalId = Guid.NewGuid().ToString()
                };

                // Handle cash-specific logic
                if (paymentType == PaymentType.Cash)
                {
                    command.TenderAmount = new Money(_tenderAmount, currency);

                    // Get active cash session
                    var session = await cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
                    if (session != null)
                    {
                        command.CashSessionId = session.Id;
                    }
                    else
                    {
                        _logger.LogError("No active cash session for terminal {TerminalId}", terminalId);
                        await _dialogService.ShowErrorAsync(
                            "Session Error",
                            "No active cash session found for this terminal. Please start a cash session before processing cash payments.");
                        return;
                    }
                }
                else if (paymentType == PaymentType.CreditCard)
                {
                    // Simulate card data
                    command.Last4 = "1234";
                    command.CardType = "Visa";
                    command.AuthCode = "AUTH" + DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                else if (paymentType == PaymentType.GiftCertificate)
                {
                    // Simulate gift card
                    command.GiftCardNumber = "GC-" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
                }

                var result = await processPaymentHandler.HandleAsync(command);

                _logger.LogInformation("Payment processed: {PaymentId}, Change: {Change}, Ticket Paid: {IsPaid}",
                    result.PaymentId, result.ChangeAmount, result.TicketIsPaid);

                // Handle overpayment (change due)
                if (paymentType == PaymentType.Cash && result.ChangeAmount.Amount > 0)
                {
                    _logger.LogInformation("Change due: {Change}", result.ChangeAmount);
                    
                    // Show change dialog
                    await _dialogService.ShowMessageAsync(
                        "Change Due",
                        $"Change: {result.ChangeAmount.Amount:C2}\n\nPlease give the customer their change.");
                }

                // Reload ticket to get updated balances
                await LoadTicketAsync();

                // Clear tender for next payment if ticket not fully paid
                if (!result.TicketIsPaid)
                {
                    OnClearTender();
                }
                else
                {
                    // Ticket is fully paid - show confirmation and navigate back
                    _logger.LogInformation("Ticket {TicketId} is fully paid", _ticketId);
                    
                    await _dialogService.ShowMessageAsync(
                        "Payment Complete",
                        $"Ticket #{_ticket.TicketNumber} has been paid in full.\n\nTotal: {_ticket.TotalAmount:C2}");
                    
                    // Navigate back to main page
                    _navigationService.GoBack();
                }
            }
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            // Network connectivity error
            _logger.LogError(ex, "Network error while processing payment for ticket {TicketId}", _ticketId);
            
            var retry = await _dialogService.ShowConfirmationAsync(
                "Network Error",
                "Unable to connect to the payment server. The payment has not been processed.\n\nWould you like to retry?",
                "Retry", "Cancel");
            
            if (retry)
            {
                // Retry the payment
                await ProcessPaymentAsync(paymentType);
            }
        }
        catch (TimeoutException ex)
        {
            // Timeout error
            _logger.LogError(ex, "Timeout while processing payment for ticket {TicketId}", _ticketId);
            
            var retry = await _dialogService.ShowConfirmationAsync(
                "Timeout Error",
                "The payment request timed out. The payment may or may not have been processed.\n\nPlease verify the payment status before retrying.",
                "Retry", "Cancel");
            
            if (retry)
            {
                // Retry the payment
                await ProcessPaymentAsync(paymentType);
            }
        }
        catch (InvalidOperationException ex)
        {
            // Business logic error (e.g., invalid state)
            _logger.LogError(ex, "Invalid operation while processing payment for ticket {TicketId}", _ticketId);
            
            await _dialogService.ShowErrorAsync(
                "Payment Error",
                $"Unable to process payment: {ex.Message}\n\nPlease check the ticket status and try again.",
                ex.ToString());
        }
        catch (Exception ex)
        {
            // General error
            _logger.LogError(ex, "Failed to process payment for ticket {TicketId}", _ticketId);
            
            var retry = await _dialogService.ShowConfirmationAsync(
                "Payment Error",
                $"An error occurred while processing the payment:\n\n{ex.Message}\n\nWould you like to retry?",
                "Retry", "Cancel");
            
            if (retry)
            {
                // Retry the payment
                await ProcessPaymentAsync(paymentType);
            }
        }
        finally
        {
            IsProcessingPayment = false;
            IsBusy = false;
        }
    }

}

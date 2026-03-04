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
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Presentation.Views.Dialogs;
using Magidesk.Presentation.ViewModels.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for ticket modification operations.
/// Handles tips, hold, split, and discount operations.
/// </summary>
public partial class SettlePageViewModel
{
    private async Task OnAddTipAsync()
    {
        if (_ticket == null)
        {
            _logger.LogWarning("Cannot add tip: no ticket loaded");
            return;
        }

        try
        {
            _logger.LogInformation("Add tip requested for ticket {TicketId}", _ticketId);
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var gratuityService = scope.ServiceProvider.GetRequiredService<IGratuityService>();
                var applyGratuityHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<ApplyGratuityCommand, ApplyGratuityResult>>();
                var dialogService = scope.ServiceProvider.GetRequiredService<IDialogService>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<GratuitySelectionViewModel>>();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                
                // Get available servers (current user and ticket creator)
                var availableServers = new ObservableCollection<ServerItem>();
                
                // Add current user
                var currentUserId = _userContextService.GetCurrentUserId();
                if (currentUserId != Guid.Empty)
                {
                    availableServers.Add(new ServerItem(
                        new UserId(currentUserId),
                        _userService.CurrentUser?.FullName ?? "Current User"));
                }
                
                // Add ticket creator if different
                if (_ticket.CreatedBy != _userService.CurrentUser?.Id)
                {
                    var creator = await userRepository.GetByIdAsync(_ticket.CreatedBy);
                    if (creator != null)
                    {
                        availableServers.Add(new ServerItem(
                            new UserId(creator.Id),
                            $"{creator.FirstName} {creator.LastName}"));
                    }
                }
                
                // Create ViewModel for gratuity selection dialog
                var viewModel = new GratuitySelectionViewModel(
                    gratuityService,
                    applyGratuityHandler,
                    dialogService,
                    logger,
                    _ticket.Id,
                    $"#{_ticket.TicketNumber}",
                    new Money(_ticket.SubtotalAmount, "USD"),
                    new UserId(_userContextService.GetCurrentUserId()),
                    availableServers);
                
                // Create Dialog
                var dialog = new GratuitySelectionDialog(viewModel);
                
                // Ensure XamlRoot is set if available
                if (_xamlRoot != null)
                {
                    dialog.XamlRoot = _xamlRoot;
                }
                
                // Use NavigationService to show dialog (handles XamlRoot automatically)
                await _navigationService.ShowDialogAsync(dialog);

                // Reload ticket to get updated totals (gratuity is applied within the dialog)
                await LoadTicketAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add tip to ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to add tip: {ex.Message}");
        }
    }

    private async Task OnHoldTicketAsync()
    {
        if (_ticket == null)
        {
            _logger.LogWarning("Cannot hold ticket: no ticket loaded");
            return;
        }

        try
        {
            // Confirm with user before holding
            var confirmed = await _dialogService.ShowConfirmationAsync(
                "Hold Ticket",
                $"Hold ticket #{_ticket.TicketNumber}?\n\nYou can resume this ticket later from the held tickets list.",
                "Hold", "Cancel");
            
            if (confirmed)
            {
                _logger.LogInformation("Holding ticket {TicketId}", _ticketId);
                _navigationService.GoBack();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to hold ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", "Failed to hold ticket.", ex.Message);
        }
    }

    private async Task OnSplitPaymentAsync()
    {
        if (_ticket == null)
        {
            _logger.LogWarning("Cannot split payment: no ticket loaded");
            return;
        }

        try
        {
            _logger.LogInformation("Split payment requested for ticket {TicketId}", _ticketId);
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var processSplitPaymentHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<ProcessSplitPaymentCommand, ProcessSplitPaymentResult>>();
                
                var viewModel = new SplitPaymentViewModel(
                    processSplitPaymentHandler,
                    _userContextService);
                
                // Initialize with current Balance Due to ensuring we split the remaining amount
                // We assume default currency USD as per other methods in this VM
                var amountToSplit = new Money(BalanceDue, "USD");
                viewModel.Initialize(_ticket.Id, amountToSplit);
                
                var dialog = new SplitPaymentDialog(viewModel);
                
                // Show Dialog
                await _navigationService.ShowDialogAsync(dialog);
                
                if (viewModel.IsSuccess)
                {
                    _logger.LogInformation("Split payment successful for ticket {TicketId}", _ticketId);
                    
                    if (viewModel.ChangeAmount > Money.Zero())
                    {
                         await _dialogService.ShowMessageAsync(
                            "Payment Complete",
                            $"Split payment processed successfully.\n\nChange Due: {viewModel.ChangeAmount:C2}");
                    }
                    else
                    {
                         await _dialogService.ShowMessageAsync(
                            "Payment Complete",
                            "Split payment processed successfully.");
                    }
                    
                    await LoadTicketAsync();
                    
                    // If ticket is fully settled (or overpaid), navigate back
                    if (BalanceDue <= 0)
                    {
                        _navigationService.GoBack();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process split payment for ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to process split payment: {ex.Message}");
        }
    }

    private async Task OnApplyDiscountAsync()
    {
        if (_ticket == null)
        {
            _logger.LogWarning("Cannot apply discount: no ticket loaded");
            return;
        }

        try
        {
            _logger.LogInformation("Apply discount requested for ticket {TicketId}", _ticketId);
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var discountRepository = scope.ServiceProvider.GetRequiredService<IDiscountRepository>();
                var applyDiscountHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<ApplyDiscountCommand>>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var managerPinDialog = scope.ServiceProvider.GetRequiredService<ManagerPinDialogViewModel>();
                
                // Create ViewModel for discount selection dialog
                var viewModel = new DiscountSelectionViewModel(
                    discountRepository,
                    applyDiscountHandler,
                    _userContextService,
                    managerPinDialog);
                
                // Initialize with ticket information
                viewModel.TicketId = _ticket.Id;
                viewModel.TicketTotal = new Money(_ticket.TotalAmount, "USD");
                
                // Create and show dialog
                var dialog = new DiscountSelectionDialog(viewModel);
                
                // Use NavigationService to show dialog (handles XamlRoot automatically)
                await _navigationService.ShowDialogAsync(dialog);
                
                if (viewModel.IsSuccess)
                {
                    _logger.LogInformation("Discount applied successfully to ticket {TicketId}", _ticketId);
                    
                    await _dialogService.ShowMessageAsync(
                        "Discount Applied",
                        $"Discount has been applied to ticket #{_ticket.TicketNumber}.");
                    
                    // Reload ticket to get updated totals
                    await LoadTicketAsync();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply discount to ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to apply discount: {ex.Message}");
        }
    }

}

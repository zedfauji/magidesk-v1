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
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Partial class for adding products and creating tickets.
/// Handles product selection, modifier dialogs, and ticket creation.
/// </summary>
public partial class OrderPageViewModel
{
    private async Task OnAddProductAsync(ProductViewModel? product)
    {
        if (product == null) return;

        // Check if product is available
        if (!product.IsAvailable)
        {
            _logger.LogWarning("Cannot add product {ProductName}: product not available", product.Name);
            await _dialogService.ShowWarningAsync(
                "Product Unavailable",
                $"{product.Name} is currently unavailable and cannot be added to the order.");
            return;
        }

        try
        {
            // Create ticket if it doesn't exist
            if (!_ticketId.HasValue)
            {
                await CreateTicketAsync();
            }

            if (!_ticketId.HasValue)
            {
               if (_userContextService.GetCurrentUserId() == Guid.Empty)
            {
                await _dialogService.ShowWarningAsync(
                    "User Not Found",
                    "No current user is set. Please login again.");
                return;
            }
                _logger.LogError("Failed to create ticket");
                await _dialogService.ShowErrorAsync(
                    "Error",
                    "Failed to create ticket. Please try again.");
                return;
            }

            List<MenuModifier> selectedModifiers = new();

            // Check if product has modifiers
            if (product.HasModifiers)
            {
                _logger.LogInformation("Product {ProductName} has modifiers, showing dialog", product.Name);

                // Get the full menu item to check for modifiers
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var menuRepository = scope.ServiceProvider.GetRequiredService<IMenuRepository>();
                    var menuItem = await menuRepository.GetByIdAsync(product.ProductId);

                    if (menuItem == null)
                    {
                        _logger.LogError("Menu item {ProductId} not found", product.ProductId);
                        await _dialogService.ShowErrorAsync(
                            "Product Not Found",
                            $"{product.Name} could not be found in the menu. It may have been removed.");
                        return;
                    }

                    if (menuItem != null && menuItem.ModifierGroups.Any())
                    {
                        // Create a temporary order line DTO for the modifier dialog
                        var tempOrderLine = new OrderLineDto
                        {
                            Id = Guid.NewGuid(),
                            MenuItemId = product.ProductId,
                            MenuItemName = product.Name,
                            Quantity = 1,
                            UnitPrice = product.Price,
                            TaxRate = TaxRate,
                            Modifiers = new List<OrderLineModifierDto>()
                        };

                        // Show modifier selection dialog
                        var modifierViewModel = new Magidesk.Presentation.ViewModels.Dialogs.ModifierSelectionViewModel(
                            menuRepository,
                            tempOrderLine);

                        var dialog = new Magidesk.Presentation.Views.Dialogs.ModifierSelectionDialog(modifierViewModel);

                        // Set XamlRoot for the dialog
                        if (_xamlRoot != null)
                        {
                            dialog.XamlRoot = _xamlRoot;
                        }
                        else
                        {
                            _logger.LogError("XamlRoot is null - cannot show modifier dialog");
                            throw new InvalidOperationException("XamlRoot must be set before showing dialogs.");
                        }

                        await dialog.ShowAsync();

                        // If user confirmed, get the selected modifiers
                        if (modifierViewModel.IsConfirmed)
                        {
                            // Convert OrderLineModifierDto to MenuModifier entities
                            foreach (var modDto in modifierViewModel.ResultModifiers)
                            {
                                if (modDto.ModifierId.HasValue)
                                {
                                    var modifier = await menuRepository.GetModifierByIdAsync(modDto.ModifierId.Value);
                                    if (modifier != null)
                                    {
                                        selectedModifiers.Add(modifier);
                                    }
                                }
                            }

                            _logger.LogInformation("User selected {Count} modifiers for {ProductName}",
                                selectedModifiers.Count, product.Name);
                        }
                        else
                        {
                            // User cancelled the modifier selection
                            _logger.LogInformation("User cancelled modifier selection for {ProductName}", product.Name);
                            return;
                        }
                    }
                }
            }

            // Add order line with modifiers
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var addOrderLineHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<AddOrderLineCommand, AddOrderLineResult>>();
                var menuRepository = scope.ServiceProvider.GetRequiredService<IMenuRepository>();

                // Get the menu item to get accurate pricing and details
                var menuItem = await menuRepository.GetByIdAsync(product.ProductId);
                if (menuItem == null)
                {
                    _logger.LogError("Menu item {ProductId} not found", product.ProductId);
                    await _dialogService.ShowErrorAsync(
                        "Product Not Found",
                        $"{product.Name} could not be found in the menu. It may have been removed.");
                    return;
                }

                var command = new AddOrderLineCommand
                {
                    TicketId = _ticketId.Value,
                    MenuItemId = product.ProductId,
                    MenuItemName = product.Name,
                    Quantity = 1,
                    UnitPrice = menuItem.Price,
                    TaxRate = menuItem.TaxRate,
                    CategoryName = menuItem.Category?.Name,
                    GroupName = menuItem.Group?.Name,
                    AddedBy = _userContextService.GetCurrentUserId() != Guid.Empty
                        ? new UserId(_userContextService.GetCurrentUserId())
                        : null,
                    Modifiers = selectedModifiers
                };

                var result = await addOrderLineHandler.HandleAsync(command);

                // Reload ticket to get updated order lines
                await LoadTicketAsync();

                _logger.LogInformation("Added product {ProductName} to ticket {TicketId} with {ModifierCount} modifiers",
                    product.Name, _ticketId, selectedModifiers.Count);
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation while adding product {ProductName}", product.Name);
            await _dialogService.ShowErrorAsync(
                "Invalid Operation",
                $"Unable to add {product.Name} to the order:\n\n{ex.Message}");
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while adding product {ProductName}", product.Name);
            await _dialogService.ShowErrorAsync(
                "Network Error",
                "Unable to connect to the server. Please check your network connection and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add product {ProductName}", product.Name);
            await _dialogService.ShowErrorAsync(
                "Error",
                $"Failed to add product: {ex.Message}");
        }
    }

    private async Task CreateTicketAsync()
    {
        try
        {
            if (_userService.CurrentUser == null)
            {
                _logger.LogError("Cannot create ticket: no user logged in");
                await _dialogService.ShowErrorAsync(
                    "Authentication Error",
                    "No user is currently logged in. Please log in and try again.");
                return;
            }

            if (_terminalContext.TerminalId == null)
            {
                _logger.LogError("Cannot create ticket: no terminal context");
                await _dialogService.ShowErrorAsync(
                    "Terminal Error",
                    "Terminal context is not available. Please restart the application.");
                return;
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                // Check if there's an active session
                var cashSessionRepository = scope.ServiceProvider.GetRequiredService<ICashSessionRepository>();
                var activeSession = await cashSessionRepository.GetOpenSessionByTerminalIdAsync(_terminalContext.TerminalId.Value);

                if (activeSession == null)
                {
                    _logger.LogError("Cannot create ticket: no active session");

                    var startSession = await _dialogService.ShowConfirmationAsync(
                        "No Active Session",
                        "There is no active POS session. You must start a session before creating orders.\n\nWould you like to start a session now?",
                        "Start Session", "Cancel");

                    if (startSession)
                    {
                        await OnStartSessionAsync();
                        // After starting session, try again
                        activeSession = await cashSessionRepository.GetOpenSessionByTerminalIdAsync(_terminalContext.TerminalId.Value);
                        if (activeSession == null)
                        {
                            // Session start failed
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                var createTicketHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateTicketCommand, CreateTicketResult>>();

                var command = new CreateTicketCommand
                {
                    TableId = _tableId,
                    CreatedBy = new UserId(_userContextService.GetCurrentUserId()),
                    TerminalId = _terminalContext.TerminalId.Value
                };

                var result = await createTicketHandler.HandleAsync(command);

                _ticketId = result.TicketId;
                _logger.LogInformation("Created new ticket {TicketId}", _ticketId);

                // Reload the ticket to get the full ticket data including table assignment
                await LoadTicketAsync();
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation while creating ticket");
            await _dialogService.ShowErrorAsync(
                "Invalid Operation",
                $"Unable to create ticket:\n\n{ex.Message}");
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while creating ticket");
            await _dialogService.ShowErrorAsync(
                "Network Error",
                "Unable to connect to the server. Please check your network connection and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create ticket");
            await _dialogService.ShowErrorAsync(
                "Error",
                $"Failed to create ticket: {ex.Message}");
        }
    }
}
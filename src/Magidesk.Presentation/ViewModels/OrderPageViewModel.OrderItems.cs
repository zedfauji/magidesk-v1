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
/// Partial class for order item modifications.
/// Handles editing and removing order line items.
/// </summary>
public partial class OrderPageViewModel
{
    private async Task OnEditOrderItemAsync(OrderItemViewModel? item)
    {
        if (item == null || !_ticketId.HasValue) return;

        try
        {
            _logger.LogInformation("Edit order item {ItemId} requested", item.OrderItemId);

            // Get the current order line from the ticket
            if (_ticket == null)
            {
                _logger.LogError("Cannot edit item: ticket not loaded");
                await _dialogService.ShowErrorAsync(
                    "Error",
                    "Ticket is not loaded. Please refresh and try again.");
                return;
            }

            var orderLine = _ticket.OrderLines.FirstOrDefault(ol => ol.Id == item.OrderItemId);
            if (orderLine == null)
            {
                _logger.LogError("Order line {OrderLineId} not found in ticket", item.OrderItemId);
                await _dialogService.ShowErrorAsync(
                    "Item Not Found",
                    "The order item could not be found. It may have been removed.");
                return;
            }

            // Get the menu item to check for modifiers
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var menuRepository = scope.ServiceProvider.GetRequiredService<IMenuRepository>();
                var menuItem = await menuRepository.GetByIdAsync(orderLine.MenuItemId);

                if (menuItem == null)
                {
                    _logger.LogError("Menu item {MenuItemId} not found", orderLine.MenuItemId);
                    await _dialogService.ShowErrorAsync(
                        "Product Not Found",
                        "The product could not be found in the menu. It may have been removed.");
                    return;
                }

                // Check if the menu item has modifiers
                if (!menuItem.ModifierGroups.Any())
                {
                    _logger.LogInformation("Menu item {MenuItemName} has no modifiers to edit", menuItem.Name);
                    await _dialogService.ShowMessageAsync(
                        "Edit Item",
                        "This item has no modifiers to edit.");
                    return;
                }

                // Create order line DTO for the modifier dialog
                var orderLineDto = new OrderLineDto
                {
                    Id = orderLine.Id,
                    MenuItemId = orderLine.MenuItemId,
                    MenuItemName = orderLine.MenuItemName,
                    Quantity = orderLine.Quantity,
                    UnitPrice = orderLine.UnitPrice,
                    TaxRate = orderLine.TaxRate,
                    Modifiers = orderLine.Modifiers.Select(m => new OrderLineModifierDto
                    {
                        ModifierId = m.ModifierId,
                        Name = m.Name,
                        ModifierType = m.ModifierType,
                        ItemCount = m.ItemCount,
                        UnitPrice = m.UnitPrice,
                        TaxRate = m.TaxRate,
                        SectionName = m.SectionName,
                        ShouldPrintToKitchen = m.ShouldPrintToKitchen
                    }).ToList()
                };

                // Show modifier selection dialog
                var modifierViewModel = new Magidesk.Presentation.ViewModels.Dialogs.ModifierSelectionViewModel(
                    menuRepository,
                    orderLineDto);

                var dialog = new Magidesk.Presentation.Views.Dialogs.ModifierSelectionDialog(modifierViewModel);

                // Set XamlRoot for the dialog
                if (Microsoft.UI.Xaml.Window.Current?.Content is Microsoft.UI.Xaml.FrameworkElement element)
                {
                    dialog.XamlRoot = element.XamlRoot;
                }

                await dialog.ShowAsync();

                // If user confirmed, update the order line with new modifiers
                if (modifierViewModel.IsConfirmed)
                {
                    _logger.LogInformation("User confirmed modifier changes for order item {ItemId}", item.OrderItemId);

                    // Execute ModifyOrderLineCommand with new modifiers
                    var modifyOrderLineHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<ModifyOrderLineCommand>>();

                    var command = new ModifyOrderLineCommand
                    {
                        TicketId = _ticketId.Value,
                        OrderLineId = item.OrderItemId,
                        Quantity = orderLine.Quantity, // Keep the same quantity
                        Modifiers = modifierViewModel.ResultModifiers
                    };

                    await modifyOrderLineHandler.HandleAsync(command);

                    // Reload ticket to get updated order lines and recalculated totals
                    await LoadTicketAsync();

                    _logger.LogInformation("Updated modifiers for order item {ItemId} in ticket {TicketId}",
                        item.OrderItemId, _ticketId);
                }
                else
                {
                    _logger.LogInformation("User cancelled modifier changes for order item {ItemId}", item.OrderItemId);
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation while editing order item {ItemId}", item.OrderItemId);
            await _dialogService.ShowErrorAsync(
                "Invalid Operation",
                $"Unable to edit item:\n\n{ex.Message}");
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while editing order item {ItemId}", item.OrderItemId);
            await _dialogService.ShowErrorAsync(
                "Network Error",
                "Unable to connect to the server. Please check your network connection and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to edit order item {ItemId}", item.OrderItemId);
            await _dialogService.ShowErrorAsync(
                "Error",
                $"Failed to edit item: {ex.Message}");
        }
    }

    private async Task OnRemoveOrderItemAsync(OrderItemViewModel? item)
    {
        if (item == null || !_ticketId.HasValue) return;

        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var removeOrderLineHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<RemoveOrderLineCommand>>();

                var command = new RemoveOrderLineCommand
                {
                    TicketId = _ticketId.Value,
                    OrderLineId = item.OrderItemId
                };

                await removeOrderLineHandler.HandleAsync(command);

                // Reload ticket to get updated order lines
                await LoadTicketAsync();

                _logger.LogInformation("Removed order item {ItemId} from ticket {TicketId}",
                    item.OrderItemId, _ticketId);
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation while removing order item {ItemId}", item.OrderItemId);
            await _dialogService.ShowErrorAsync(
                "Invalid Operation",
                $"Unable to remove item:\n\n{ex.Message}");
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while removing order item {ItemId}", item.OrderItemId);
            await _dialogService.ShowErrorAsync(
                "Network Error",
                "Unable to connect to the server. Please check your network connection and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove order item {ItemId}", item.OrderItemId);
            await _dialogService.ShowErrorAsync(
                "Error",
                $"Failed to remove item: {ex.Message}");
        }
    }
}

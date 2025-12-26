using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Services;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Services;

public class AddOrderLineComboCommandHandler : ICommandHandler<AddOrderLineComboCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IAuditEventRepository _auditEventRepository;
    private readonly PriceCalculator _priceCalculator;

    public AddOrderLineComboCommandHandler(
        ITicketRepository ticketRepository,
        IMenuRepository menuRepository,
        IAuditEventRepository auditEventRepository,
        PriceCalculator priceCalculator)
    {
        _ticketRepository = ticketRepository;
        _menuRepository = menuRepository;
        _auditEventRepository = auditEventRepository;
        _priceCalculator = priceCalculator;
    }

    public async Task HandleAsync(AddOrderLineComboCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Load Ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken)
                     ?? throw new NotFoundException($"Ticket {command.TicketId} not found.");

        var orderLine = ticket.OrderLines.FirstOrDefault(ol => ol.Id == command.OrderLineId)
                        ?? throw new NotFoundException($"OrderLine {command.OrderLineId} not found.");

        // 2. Load MenuItem to identify Combo
        var menuItem = await _menuRepository.GetByIdAsync(orderLine.MenuItemId, cancellationToken)
                       ?? throw new NotFoundException($"MenuItem {orderLine.MenuItemId} not found.");

        if (!menuItem.ComboDefinitionId.HasValue)
        {
            throw new BusinessRuleViolationException($"MenuItem {menuItem.Name} is not a valid Combo.");
        }

        // 3. Load Combo Definition
        var combo = await _menuRepository.GetComboDefinitionByIdAsync(menuItem.ComboDefinitionId.Value, cancellationToken)
                    ?? throw new NotFoundException($"ComboDefinition {menuItem.ComboDefinitionId.Value} not found.");

        // 4. Validate and Add Selections
        foreach (var selection in command.Selections)
        {
            // Validate Group exists in Combo
            var group = combo.Groups.FirstOrDefault(g => g.Id == selection.ComboGroupId);
            if (group == null)
            {
                throw new BusinessRuleViolationException($"Combo Group {selection.ComboGroupId} does not belong to Combo {combo.Name}.");
            }

            // Validate Item exists in Group
            var groupItem = group.Items.FirstOrDefault(i => i.Id == selection.ComboGroupItemId);
            if (groupItem == null)
            {
                throw new BusinessRuleViolationException($"Item {selection.ComboGroupItemId} not found in Group {group.Name}.");
            }

            // Load actual MenuItem for the selection
            var selectionItem = await _menuRepository.GetByIdAsync(groupItem.MenuItemId, cancellationToken)
                                ?? throw new NotFoundException($"MenuItem {groupItem.MenuItemId} not found.");

            // 5. Create OrderLineModifier
            // The unit price for a combo item is typically just the Upcharge
            // Base Price of the selection item (e.g. $10) is IGNORED because it's part of the combo.
            // We use the ComboGroupItem.Upcharge.
            
            var unitPrice = _priceCalculator.CalculateComboItemPrice(selectionItem.Price, groupItem.Upcharge);
            
            // Create modifier
            var modifier = OrderLineModifier.Create(
                orderLine.Id,
                null, 
                selectionItem.Name,
                ModifierType.InfoOnly, // Or generic
                selection.Quantity,
                unitPrice,
                unitPrice // Base price snapshot
            );
            
            orderLine.AddModifier(modifier);
        }

        // 6. Recalculate and Persist
        ticket.CalculateTotals();
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);
    }
}

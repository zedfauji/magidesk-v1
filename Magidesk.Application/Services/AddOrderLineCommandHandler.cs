using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for AddOrderLineCommand.
/// </summary>
public class AddOrderLineCommandHandler : ICommandHandler<AddOrderLineCommand, AddOrderLineResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public AddOrderLineCommandHandler(
        ITicketRepository ticketRepository,
        IMenuRepository menuRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _menuRepository = menuRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<AddOrderLineResult> HandleAsync(AddOrderLineCommand command, CancellationToken cancellationToken = default)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // Create order line
        var orderLine = OrderLine.Create(
            command.TicketId,
            command.MenuItemId,
            command.MenuItemName,
            command.Quantity,
            new Domain.ValueObjects.Money(command.UnitPrice.Amount, command.UnitPrice.Currency), // Deep Clone
            command.TaxRate,
            command.CategoryName,
            command.GroupName);

        // Populate PrinterGroupId (F-0014)
        // Populate PrinterGroupId (F-0014)
        // Populate PrinterGroupId (F-0014) with Inheritance Logic
        var menuItem = await _menuRepository.GetByIdAsync(command.MenuItemId, cancellationToken);
        if (menuItem != null)
        {
            if (menuItem.PrinterGroupId.HasValue)
            {
                orderLine.SetPrinterGroup(menuItem.PrinterGroupId);
            }
            else if (menuItem.Group?.PrinterGroupId.HasValue == true)
            {
                 orderLine.SetPrinterGroup(menuItem.Group.PrinterGroupId);
            }
            else if (menuItem.Category?.PrinterGroupId.HasValue == true)
            {
                 orderLine.SetPrinterGroup(menuItem.Category.PrinterGroupId);
            }
        }

        // Add Modifiers
        foreach (var mod in command.Modifiers)
        {
            var orderLineModifier = OrderLineModifier.Create(
                orderLineId: orderLine.Id,
                modifierId: mod.Id,
                name: mod.Name,
                modifierType: mod.ModifierType,
                itemCount: 1, // Default to 1 for now
                unitPrice: new Domain.ValueObjects.Money(mod.BasePrice.Amount, mod.BasePrice.Currency), // Deep Clone
                basePrice: new Domain.ValueObjects.Money(mod.BasePrice.Amount, mod.BasePrice.Currency), // Deep Clone
                taxRate: mod.TaxRate,
                modifierGroupId: mod.ModifierGroupId,
                shouldPrintToKitchen: mod.ShouldPrintToKitchen
            );
            orderLine.AddModifier(orderLineModifier);
        }

        // Add to ticket
        ticket.AddOrderLine(orderLine);

        // Update ticket
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // Create audit event
        var userId = command.AddedBy?.Value ?? Guid.Empty;
        var isMisc = (command.CategoryName?.Contains("Misc", StringComparison.OrdinalIgnoreCase) == true) || 
                     (command.MenuItemName.Contains("Misc", StringComparison.OrdinalIgnoreCase));
        
        var action = isMisc ? "Misc Item Added" : "Item Added";
        var details = isMisc ? $"Misc/Ad-hoc item '{command.MenuItemName}' equal to {command.UnitPrice} added to ticket {ticket.TicketNumber}" 
                             : $"Order line added to ticket {ticket.TicketNumber}";

        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Ticket),
            ticket.Id,
            userId,
            System.Text.Json.JsonSerializer.Serialize(new { OrderLineId = orderLine.Id, Action = action, IsMisc = isMisc }),
            details,
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new AddOrderLineResult
        {
            OrderLineId = orderLine.Id
        };
    }
}


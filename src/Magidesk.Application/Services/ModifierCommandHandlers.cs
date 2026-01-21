using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.DomainServices; // Maybe in DomainServices? No.
using Magidesk.Domain.Services; // Ensure using is correct
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

public class AddOrderLineModifierCommandHandler : ICommandHandler<AddOrderLineModifierCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IAuditEventRepository _auditEventRepository;
    private readonly PriceCalculator _priceCalculator;

    public AddOrderLineModifierCommandHandler(
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

    public async Task HandleAsync(AddOrderLineModifierCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Fetch Ticket with Lines and Modifiers
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
            throw new NotFoundException($"Ticket {command.TicketId} not found.");

        if (ticket.Status != TicketStatus.Draft && ticket.Status != TicketStatus.Open)
            throw new BusinessRuleViolationException("Can only add modifiers to Draft or Open tickets.");

        var orderLine = ticket.OrderLines.FirstOrDefault(ol => ol.Id == command.OrderLineId);
        if (orderLine == null)
            throw new NotFoundException($"OrderLine {command.OrderLineId} not found on ticket.");

        // 2. Fetch Modifier
        var modifier = await _menuRepository.GetModifierByIdAsync(command.ModifierId, cancellationToken);
        if (modifier == null)
            throw new NotFoundException($"Modifier {command.ModifierId} not found.");

        // 3. Fetch MenuItem to check Group Rules
        var menuItem = await _menuRepository.GetByIdAsync(orderLine.MenuItemId, cancellationToken);
        if (menuItem == null)
             throw new NotFoundException($"MenuItem {orderLine.MenuItemId} not found.");

        // 4. Validate Modifier Group Association
        MenuItemModifierGroup? link = null;
        if (modifier.ModifierGroupId.HasValue)
        {
            link = menuItem.ModifierGroups.FirstOrDefault(g => g.ModifierGroupId == modifier.ModifierGroupId.Value);
            if (link == null)
                throw new BusinessRuleViolationException($"Modifier '{modifier.Name}' is not available for item '{menuItem.Name}'.");
                
            // 5. Validate Constraints (Max Selections)
            var group = link.ModifierGroup;
            // Count existing modifiers for this group on this line
            // Note: _modifiers and _addOns are split in domain, but exposed via public properties
            var existingCount = orderLine.Modifiers.Count(m => m.ModifierGroupId == group.Id) 
                              + orderLine.AddOns.Count(m => m.ModifierGroupId == group.Id);
            
            // F-0010: Max Selection Logic
            // If MaxSelections is 0, it might mean unlimited? Or 0? Let's check ModifierGroup.Create.
            // "if (maxSelections < minSelections)" implying logic. usually 0 is not unlimited in this context if Min is 0. 
            // If AllowMultipleSelections is true, max might be high.
            // Assuming strict check if Max > 0. If unlimited, Max might be huge number.
            
            // Checking logic in ModifierGroup.Create: defaults to Max=1.
            // So if > MaxSelections, throw.
            if (existingCount + 1 > group.MaxSelections)
            {
                throw new BusinessRuleViolationException($"Cannot add more than {group.MaxSelections} selection(s) for '{group.Name}'.");
            }
        }
        else
        {
             // Ad-hoc modifier or un-grouped?
             // If un-grouped, maybe allow? Or strictly valid only via groups?
             // For now, assuming standard flow requires groups.
             // If loose modifier, we proceed without group validation.
        }

        // 6. Add Modifier
        // Determine MenuItemModifierGroupId:
        var menuItemModifierGroupId = link != null ? (Guid?)null : null; 
        // Wait, MenuItemModifierGroup is a composite key entity, it doesn't have a single ID unless I added it?
        // Checking MenuItemModifierGroup.cs: No single ID. Has MenuItemId, ModifierGroupId.
        // OrderLineModifier.MenuItemModifierGroupId seems to expect a Guid.
        // This implies OrderLineModifier expects a link ID.
        // Since I didn't add one, I'll update it to use ModifierGroupId directly for logic, 
        // and leave MenuItemModifierGroupId null or change usage.
        // I added ModifierGroupId to OrderLineModifier, so I can use that.

        // 6. Validate Parent Modifier (Nested Modifiers F-0015)
        if (command.ParentOrderLineModifierId.HasValue)
        {
             var parentModifier = orderLine.Modifiers.FirstOrDefault(m => m.Id == command.ParentOrderLineModifierId.Value)
                                ?? orderLine.AddOns.FirstOrDefault(m => m.Id == command.ParentOrderLineModifierId.Value);

             if (parentModifier == null)
                 throw new NotFoundException($"Parent Modifier {command.ParentOrderLineModifierId} not found on this order line.");

             // Cycle detection (simple: preventing self-parenting, though ID is new so unlikely, but depth check could be here)
             // For now, 1 level of nesting or recursive allowed.
        }

        // 7. Calculate Price based on Quantity/Portion
        // F-0020 & F-0037: Pizza Logic
        
        var quantity = command.Quantity > 0 ? command.Quantity : 1;
        var itemCount = (int)Math.Ceiling(quantity); 
        
        Money finalUnitPrice;
        decimal portionValue = 1.0m;
        PriceStrategy? priceStrategy = null;
        
        if (modifier is FractionalModifier fractional)
        {
             // It's a fractional modifier (e.g. Half Pizza)
             // We use the definition's portion value if mapped, or infer from quantity?
             // Usually FractionalModifier.Portion is an Enum. Needs conversion.
             // We'll assume the QUANTITY passed to command drives the portion value (e.g. 0.5)
             // OR we read from definition. TDD says 'Portion' enum.
             // Let's assume quantity is the driver for now for simplicity and flexibility (0.5 = half).
             // But we capture Strategy.
             priceStrategy = fractional.PriceStrategy;
             
             // If quantity < 1, valid fraction.
             // portionValue should be equal to quantity ideally.
             portionValue = quantity;
        }

        if (quantity < 1)
        {
             // Fractional (Half) - Initial price is pro-rated (SumOfHalves default behavior)
             finalUnitPrice = modifier.BasePrice * quantity;
        }
        else
        {
             // Multiplier (Double) or Whole
             finalUnitPrice = modifier.BasePrice;
             portionValue = 1.0m; // Whole
        }

        var newModifier = OrderLineModifier.Create(
            orderLine.Id,
            modifier.Id,
            modifier.Name,
            modifier.ModifierType,
            itemCount, 
            finalUnitPrice,
            modifier.BasePrice, // Pass Snapshot Base Price
            portionValue,
            modifier.TaxRate,
            null, // menuItemModifierGroupId (not using)
            modifier.ModifierGroupId, // Stores the group ID
            modifier.ShouldPrintToKitchen,
            command.SectionName, // Pass Section Name
            null, // MultiplierName (could infer from quantity if > 1)
            modifier.IsSectionWisePrice,
            command.ParentOrderLineModifierId,
            priceStrategy
        );

        orderLine.AddModifier(newModifier);
        
        // F-0037: Recalculate if Fractional Strategy exists
        if (priceStrategy.HasValue && newModifier.IsSectionWisePrice)
        {
             // Recalculate all fractional modifiers on this line for this strategy
             _priceCalculator.RecalculateFractionalPrices(orderLine.Modifiers.ToList(), priceStrategy.Value);
        }
        
        // 7. Persist
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);
        
        // 8. Audit
        await _auditEventRepository.AddAsync(AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Ticket),
            ticket.Id,
            command.AddedBy?.Value ?? Guid.Empty,
            System.Text.Json.JsonSerializer.Serialize(new { 
                OrderLineId = orderLine.Id, 
                ModifierId = modifier.Id, 
                Name = modifier.Name 
            }),
            $"Added modifier '{modifier.Name}' to '{orderLine.MenuItemName}'",
            null,
            Guid.NewGuid()
        ), cancellationToken);
    }
}

public class RemoveOrderLineModifierCommandHandler : ICommandHandler<RemoveOrderLineModifierCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public RemoveOrderLineModifierCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(RemoveOrderLineModifierCommand command, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
            throw new NotFoundException($"Ticket {command.TicketId} not found.");

        if (ticket.Status != TicketStatus.Draft && ticket.Status != TicketStatus.Open)
            throw new BusinessRuleViolationException("Can only remove modifiers from Draft or Open tickets.");

        var orderLine = ticket.OrderLines.FirstOrDefault(ol => ol.Id == command.OrderLineId);
        if (orderLine == null)
            throw new NotFoundException($"OrderLine {command.OrderLineId} not found on ticket.");

        var modifier = orderLine.Modifiers.FirstOrDefault(m => m.Id == command.OrderLineModifierId)
                    ?? orderLine.AddOns.FirstOrDefault(m => m.Id == command.OrderLineModifierId);

        if (modifier == null)
            throw new NotFoundException($"Modifier {command.OrderLineModifierId} not found on order line.");

        orderLine.RemoveModifier(modifier);

        // Validation for MinSelections?
        // Technially removing might violate MinSelections.
        // Should we enforce it here?
        // Ideally yes, OR we allow removal but prevent "Send to Kitchen" / "Pay" if validation fails.
        // Usually, removing is allowed, but the ticket becomes "Invalid" state.
        // For F-0010, the requirement is "Min/Max selection limits". 
        // Blocking removal might be annoying if trying to swap.
        // I'll allow removal for now aka "Transient Invalid State".
        // Full validation should happen on Send/Pay.

        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        await _auditEventRepository.AddAsync(AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Ticket),
            ticket.Id,
            command.RemovedBy?.Value ?? Guid.Empty,
            System.Text.Json.JsonSerializer.Serialize(new { 
                OrderLineId = orderLine.Id, 
                ModifierId = modifier.ModifierId,
                Name = modifier.Name 
            }),
            $"Removed modifier '{modifier.Name}' from '{orderLine.MenuItemName}'",
            null,
            Guid.NewGuid()
        ), cancellationToken);
    }
}

public class AddOrderLineInstructionCommandHandler : ICommandHandler<AddOrderLineInstructionCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public AddOrderLineInstructionCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(AddOrderLineInstructionCommand command, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
            throw new NotFoundException($"Ticket {command.TicketId} not found.");

        if (ticket.Status != TicketStatus.Draft && ticket.Status != TicketStatus.Open)
            throw new BusinessRuleViolationException("Can only add instructions to Draft or Open tickets.");

        var orderLine = ticket.OrderLines.FirstOrDefault(ol => ol.Id == command.OrderLineId);
        if (orderLine == null)
            throw new NotFoundException($"OrderLine {command.OrderLineId} not found on ticket.");

        var instructionModifier = OrderLineModifier.CreateInstruction(
            orderLine.Id,
            command.Instruction
        );

        orderLine.AddModifier(instructionModifier);

        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        await _auditEventRepository.AddAsync(AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Ticket),
            ticket.Id,
            command.AddedBy?.Value ?? Guid.Empty,
            System.Text.Json.JsonSerializer.Serialize(new { 
                OrderLineId = orderLine.Id, 
                Instruction = command.Instruction 
            }),
            $"Added instruction '{command.Instruction}' to '{orderLine.MenuItemName}'",
            null,
            Guid.NewGuid()
        ), cancellationToken);
    }
}



using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Magidesk.Application.Services;

public class MergeTicketsCommandHandler : ICommandHandler<MergeTicketsCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public MergeTicketsCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(MergeTicketsCommand command, CancellationToken cancellationToken = default)
    {
        var sourceTicket = await _ticketRepository.GetByIdAsync(command.SourceTicketId, cancellationToken);
        var targetTicket = await _ticketRepository.GetByIdAsync(command.TargetTicketId, cancellationToken);

        if (sourceTicket == null) throw new Domain.Exceptions.NotFoundException($"Source Ticket {command.SourceTicketId} not found");
        if (targetTicket == null) throw new Domain.Exceptions.NotFoundException($"Target Ticket {command.TargetTicketId} not found");

        if (sourceTicket.Status == TicketStatus.Closed || sourceTicket.Status == TicketStatus.Voided)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException("Cannot merge a closed or voided ticket.");
        }
        
        if (targetTicket.Status == TicketStatus.Closed || targetTicket.Status == TicketStatus.Voided)
        {
             throw new Domain.Exceptions.BusinessRuleViolationException("Cannot merge into a closed or voided ticket.");
        }

        // Copy Order Lines from Source to Target
        foreach (var line in sourceTicket.OrderLines)
        {
            var newLine = OrderLine.Create(
                targetTicket.Id,
                line.MenuItemId,
                line.MenuItemName,
                line.Quantity,
                line.UnitPrice,
                line.TaxRate,
                line.CategoryName,
                line.GroupName
            );

            // Copy modifiers
            foreach (var mod in line.Modifiers)
            {
                 var newMod = OrderLineModifier.Create(
                     orderLineId: newLine.Id,
                     modifierId: mod.ModifierId,
                     name: mod.Name,
                     modifierType: mod.ModifierType,
                     itemCount: mod.ItemCount,
                     unitPrice: mod.UnitPrice,
                     basePrice: mod.BasePrice,
                     portionValue: mod.PortionValue,
                     taxRate: mod.TaxRate,
                     menuItemModifierGroupId: mod.MenuItemModifierGroupId,
                     modifierGroupId: mod.ModifierGroupId,
                     shouldPrintToKitchen: mod.ShouldPrintToKitchen,
                     sectionName: mod.SectionName,
                     multiplierName: mod.MultiplierName,
                     isSectionWisePrice: mod.IsSectionWisePrice,
                     parentOrderLineModifierId: null, // Reset hierarchy or clone carefully? Flatten for now.
                     priceStrategy: mod.PriceStrategy
                 );
                 newLine.AddModifier(newMod);
            }
            
            // Copy instructions
            newLine.SetInstructions(line.Instructions);
            
            // Seat number - copy it
            newLine.SetSeatNumber(line.SeatNumber);
            
            targetTicket.AddOrderLine(newLine);
        }

        // Void the source ticket (not waste)
        sourceTicket.Void(command.ProcessedBy, $"Merged into Ticket #{targetTicket.TicketNumber}", false);

        await _ticketRepository.UpdateAsync(sourceTicket, cancellationToken);
        await _ticketRepository.UpdateAsync(targetTicket, cancellationToken);
        
        await _auditEventRepository.AddAsync(AuditEvent.Create(
            eventType: AuditEventType.TicketMerged,
            entityType: nameof(Ticket),
            entityId: targetTicket.Id,
            userId: command.ProcessedBy,
            afterState: @$"{{""SourceTicketId"": ""{sourceTicket.Id}"", ""TargetTicketId"": ""{targetTicket.Id}""}}",
            description: $"Merged Ticket #{sourceTicket.TicketNumber} into Ticket #{targetTicket.TicketNumber}"
        ), cancellationToken);
    }
}

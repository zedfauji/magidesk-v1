using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for RemoveDiscountCommand.
/// Task 2.1.6: Create RemoveDiscountCommand and handler
/// </summary>
public class RemoveDiscountCommandHandler : ICommandHandler<RemoveDiscountCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public RemoveDiscountCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(RemoveDiscountCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Load ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // 2. Find the discount to capture details before removal (for audit)
        var discount = ticket.Discounts.FirstOrDefault(d => d.Id == command.DiscountId);
        if (discount == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Discount {command.DiscountId} not found on ticket {command.TicketId}.");
        }

        // Capture discount details for audit
        var discountName = discount.Name;
        var discountAmount = discount.Amount;
        var totalBeforeRemoval = ticket.TotalAmount;

        // 3. Remove discount using Ticket.RemoveDiscount() method
        // This method will:
        // - Validate ticket status allows discount removal
        // - Remove the discount from the collection
        // - Recalculate totals
        // - Raise DiscountRemovedEvent (when event raising is implemented)
        ticket.RemoveDiscount(command.DiscountId);

        // 4. Create audit event
        var auditDetails = $"DiscountId={command.DiscountId}, Name={discountName}, Amount={discountAmount.Amount}, RemovedBy={command.RemovedBy}, TotalBefore={totalBeforeRemoval.Amount}, TotalAfter={ticket.TotalAmount.Amount}";
        
        var auditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Ticket),
            ticket.Id,
            command.RemovedBy,
            auditDetails,
            $"Removed discount '{discountName}' ({discountAmount}) from ticket #{ticket.TicketNumber}",
            beforeState: $"Total={totalBeforeRemoval.Amount}",
            correlationId: Guid.NewGuid()
        );

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        // 5. Save ticket
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);
    }
}

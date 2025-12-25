using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for ModifyOrderLineCommand.
/// </summary>
public class ModifyOrderLineCommandHandler : ICommandHandler<ModifyOrderLineCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public ModifyOrderLineCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(ModifyOrderLineCommand command, CancellationToken cancellationToken = default)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // Find order line
        var orderLine = ticket.OrderLines.FirstOrDefault(ol => ol.Id == command.OrderLineId);
        if (orderLine == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"OrderLine {command.OrderLineId} not found.");
        }

        // Update quantity
        orderLine.UpdateQuantity(command.Quantity);

        // Recalculate ticket totals
        ticket.CalculateTotals();

        // Update ticket
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            nameof(OrderLine),
            orderLine.Id,
            Guid.Empty, // Would need to get from context
            System.Text.Json.JsonSerializer.Serialize(new { Quantity = orderLine.Quantity, Action = "Modified" }),
            $"Order line {command.OrderLineId} modified in ticket {ticket.TicketNumber}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);
    }
}


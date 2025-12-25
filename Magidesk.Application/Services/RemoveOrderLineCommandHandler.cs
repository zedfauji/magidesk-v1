using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for RemoveOrderLineCommand.
/// </summary>
public class RemoveOrderLineCommandHandler : ICommandHandler<RemoveOrderLineCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public RemoveOrderLineCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(RemoveOrderLineCommand command, CancellationToken cancellationToken = default)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // Remove order line
        ticket.RemoveOrderLine(command.OrderLineId);

        // Update ticket
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Domain.Entities.Ticket),
            ticket.Id,
            Guid.Empty, // Would need to get from context
            System.Text.Json.JsonSerializer.Serialize(new { OrderLineId = command.OrderLineId, Action = "Removed" }),
            $"Order line removed from ticket {ticket.TicketNumber}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);
    }
}


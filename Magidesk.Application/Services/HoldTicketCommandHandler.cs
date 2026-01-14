using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for HoldTicketCommand.
/// Holds a ticket for later payment and releases the associated table session.
/// </summary>
public class HoldTicketCommandHandler : ICommandHandler<HoldTicketCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ITableSessionRepository _sessionRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public HoldTicketCommandHandler(
        ITicketRepository ticketRepository,
        ITableSessionRepository sessionRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _sessionRepository = sessionRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(HoldTicketCommand command, CancellationToken cancellationToken = default)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // Hold the ticket
        try
        {
            ticket.Hold(command.Reason, command.UserId);
        }
        catch (Domain.Exceptions.InvalidOperationException ex)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException(ex.Message);
        }

        // If ticket is linked to a session, end the session to release the table
        if (ticket.SessionId.HasValue)
        {
            var session = await _sessionRepository.GetByIdAsync(ticket.SessionId.Value);
            if (session != null && session.Status == TableSessionStatus.Active)
            {
                // End session with zero charge since ticket is being held for later payment
                session.End(Domain.ValueObjects.Money.Zero());
                await _sessionRepository.UpdateAsync(session);
            }
        }

        // Update ticket
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.StatusChanged,
            nameof(Ticket),
            ticket.Id,
            command.UserId.Value,
            System.Text.Json.JsonSerializer.Serialize(new { Status = ticket.Status, Reason = command.Reason }),
            $"Ticket {ticket.TicketNumber} held: {command.Reason}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);
    }
}

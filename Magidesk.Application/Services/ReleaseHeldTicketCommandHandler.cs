using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for ReleaseHeldTicketCommand.
/// Releases a held ticket back to open status for payment processing.
/// </summary>
public class ReleaseHeldTicketCommandHandler : ICommandHandler<ReleaseHeldTicketCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public ReleaseHeldTicketCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(ReleaseHeldTicketCommand command, CancellationToken cancellationToken = default)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // Release the ticket
        try
        {
            ticket.Release();
        }
        catch (Domain.Exceptions.InvalidOperationException ex)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException(ex.Message);
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
            System.Text.Json.JsonSerializer.Serialize(new { Status = ticket.Status }),
            $"Held ticket {ticket.TicketNumber} released for payment",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);
    }
}

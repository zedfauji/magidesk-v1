using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for CloseTicketCommand.
/// </summary>
public class CloseTicketCommandHandler : ICommandHandler<CloseTicketCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;
    private readonly Domain.DomainServices.TicketDomainService _ticketDomainService;

    public CloseTicketCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository,
        Domain.DomainServices.TicketDomainService ticketDomainService)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
        _ticketDomainService = ticketDomainService;
    }

    public async Task HandleAsync(CloseTicketCommand command, CancellationToken cancellationToken = default)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // Validate can close
        if (!_ticketDomainService.CanCloseTicket(ticket))
        {
            throw new Domain.Exceptions.InvalidOperationException($"Ticket {ticket.TicketNumber} cannot be closed.");
        }

        // Close ticket
        ticket.Close(command.ClosedBy);

        // Update ticket
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.StatusChanged,
            nameof(Domain.Entities.Ticket),
            ticket.Id,
            command.ClosedBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new { Status = ticket.Status }),
            $"Ticket {ticket.TicketNumber} closed",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);
    }
}


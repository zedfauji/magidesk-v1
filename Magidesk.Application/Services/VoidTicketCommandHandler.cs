using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for VoidTicketCommand.
/// </summary>
public class VoidTicketCommandHandler : ICommandHandler<VoidTicketCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;
    private readonly Domain.DomainServices.TicketDomainService _ticketDomainService;
    private readonly ISecurityService _securityService;

    public VoidTicketCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository,
        Domain.DomainServices.TicketDomainService ticketDomainService,
        ISecurityService securityService)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
        _ticketDomainService = ticketDomainService;
        _securityService = securityService;
    }

    public async Task HandleAsync(VoidTicketCommand command, CancellationToken cancellationToken = default)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // Check Permissions
        if (!await _securityService.HasPermissionAsync(command.VoidedBy, Domain.Enumerations.UserPermission.VoidTicket, cancellationToken))
        {
             throw new Domain.Exceptions.BusinessRuleViolationException("User does not have permission to void tickets.");
        }

        // Validate can void
        if (!_ticketDomainService.CanVoidTicket(ticket))
        {
            throw new Domain.Exceptions.InvalidOperationException($"Ticket {ticket.TicketNumber} cannot be voided.");
        }

        // Void ticket
        ticket.Void(command.VoidedBy);

        // Update ticket
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.Voided,
            nameof(Domain.Entities.Ticket),
            ticket.Id,
            command.VoidedBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new { Status = ticket.Status }),
            $"Ticket {ticket.TicketNumber} voided",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);
    }
}


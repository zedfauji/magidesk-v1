using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for VoidTicketCommand.
/// REQ-5.1, REQ-5.2, REQ-5.3: Validates authorization and voids open tickets.
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
        // Validate command
        if (string.IsNullOrWhiteSpace(command.Reason))
        {
            throw new Domain.Exceptions.BusinessRuleViolationException("Void reason is required.");
        }

        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // REQ-5.2: Validate manager authorization
        // Check if the authorizing user has manager permissions
        if (!await _securityService.HasPermissionAsync(command.AuthorizedBy, UserPermission.VoidTicket, cancellationToken))
        {
            throw new Domain.Exceptions.BusinessRuleViolationException(
                "Manager authorization is required to void tickets. The authorizing user does not have VoidTicket permission.");
        }

        // REQ-5.3: Check if ticket is paid - suggest refund instead
        if (ticket.Status == TicketStatus.Paid || ticket.PaidAmount > Domain.ValueObjects.Money.Zero())
        {
            throw new Domain.Exceptions.BusinessRuleViolationException(
                $"Cannot void paid ticket #{ticket.TicketNumber}. Use refund operation instead.");
        }

        // Validate can void using domain service
        if (!_ticketDomainService.CanVoidTicket(ticket))
        {
            throw new Domain.Exceptions.InvalidOperationException(
                $"Ticket #{ticket.TicketNumber} cannot be voided in {ticket.Status} status.");
        }

        // REQ-5.1: Void ticket
        try
        {
            ticket.Void(command.Reason, command.VoidedBy);
        }
        catch (Domain.Exceptions.InvalidOperationException ex)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException(ex.Message, ex);
        }

        // Update ticket
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // REQ-5.8: Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.Voided,
            nameof(Ticket),
            ticket.Id,
            command.VoidedBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new 
            { 
                Status = ticket.Status.ToString(),
                Reason = command.Reason,
                VoidedBy = command.VoidedBy.Value,
                AuthorizedBy = command.AuthorizedBy.Value
            }),
            $"Ticket #{ticket.TicketNumber} voided by {command.VoidedBy.Value}, authorized by {command.AuthorizedBy.Value}. Reason: {command.Reason}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);
    }
}



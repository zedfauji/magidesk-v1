using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

public class SettleTicketCommandHandler : ICommandHandler<SettleTicketCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public SettleTicketCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(SettleTicketCommand command, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // 1. Validate Balance
        ticket.CalculateTotals();
        if (ticket.DueAmount > Money.Zero())
        {
             // F-0008 Requirement: Forbidden State - Unbalanced Settlement
             throw new Domain.Exceptions.BusinessRuleViolationException("Cannot settle ticket with remaining balance.");
        }

        // 2. Settle/Close
        // Ticket.Close() implies settlement if balance is zero.
        ticket.Close(new UserId(command.UserId));

        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // 3. Audit
        var audit = AuditEvent.Create(
            AuditEventType.StatusChanged,
            nameof(Ticket),
            ticket.Id,
            command.UserId,
            "Settled",
            $"Ticket {ticket.TicketNumber} settled and closed"
        );
        await _auditEventRepository.AddAsync(audit, cancellationToken);
    }
}

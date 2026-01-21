using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

public class TransferTicketCommandHandler : ICommandHandler<TransferTicketCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public TransferTicketCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(TransferTicketCommand command, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new KeyNotFoundException($"Ticket {command.TicketId} not found.");
        }

        var oldOwner = ticket.CreatedBy;
        
        // Domain logic to transfer
        ticket.Transfer(command.NewOwnerId);

        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // Audit
        var audit = AuditEvent.Create(
            AuditEventType.TicketTransferred,
            nameof(Ticket),
            ticket.Id,
            command.TransferredBy.Value,
            "TransferServer",
            $"Ticket {ticket.TicketNumber} transferred from {oldOwner.Value} to {command.NewOwnerId.Value}"
        );
        await _auditEventRepository.AddAsync(audit, cancellationToken);
    }
}

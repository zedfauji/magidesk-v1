using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using System.Text.Json;

namespace Magidesk.Application.Services;

public class SetTaxExemptCommandHandler : ICommandHandler<SetTaxExemptCommand, SetTaxExemptResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public SetTaxExemptCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<SetTaxExemptResult> HandleAsync(SetTaxExemptCommand command, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
             return new SetTaxExemptResult { Success = false, Error = $"Ticket {command.TicketId} not found." };
        }

        try
        {
            // capture old state for audit
            var oldState = ticket.IsTaxExempt;
            
            // Domain method
            ticket.SetTaxExempt(command.IsTaxExempt);

            // Persist
            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            // Audit
            if (oldState != command.IsTaxExempt)
            {
                var auditEvent = AuditEvent.Create(
                    AuditEventType.Modified,
                    nameof(Ticket),
                    ticket.Id,
                    command.ModifiedBy.Value,
                    JsonSerializer.Serialize(new { OldTaxExempt = oldState, NewTaxExempt = command.IsTaxExempt }),
                    $"Ticket {ticket.TicketNumber} Tax Exempt set to {command.IsTaxExempt}",
                    correlationId: Guid.NewGuid());
                
                await _auditEventRepository.AddAsync(auditEvent, cancellationToken);
            }

            return new SetTaxExemptResult { Success = true };
        }
        catch (Exception ex)
        {
            return new SetTaxExemptResult { Success = false, Error = ex.Message };
        }
    }
}

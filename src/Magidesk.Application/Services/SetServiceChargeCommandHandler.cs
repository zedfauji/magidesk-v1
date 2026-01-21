using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using AuditEventType = Magidesk.Domain.Enumerations.AuditEventType;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for SetServiceChargeCommand.
/// </summary>
public class SetServiceChargeCommandHandler : ICommandHandler<SetServiceChargeCommand, SetServiceChargeResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public SetServiceChargeCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<SetServiceChargeResult> HandleAsync(
        SetServiceChargeCommand command,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            return new SetServiceChargeResult
            {
                Success = false,
                ErrorMessage = $"Ticket {command.TicketId} not found."
            };
        }

        var oldAmount = ticket.ServiceChargeAmount;

        try
        {
            ticket.SetServiceCharge(command.Amount);

            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            // Create audit event
            var auditEvent = AuditEvent.Create(
                AuditEventType.Modified,
                nameof(Ticket),
                ticket.Id,
                command.ProcessedBy.Value,
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    Action = "SetServiceCharge",
                    OldAmount = oldAmount,
                    NewAmount = command.Amount
                }),
                $"Service charge set to {command.Amount} on ticket {ticket.TicketNumber}",
                correlationId: Guid.NewGuid());

            await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

            return new SetServiceChargeResult
            {
                Success = true,
                NewTotalAmount = ticket.TotalAmount
            };
        }
        catch (Exception ex)
        {
            return new SetServiceChargeResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}


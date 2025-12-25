using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using AuditEventType = Magidesk.Domain.Enumerations.AuditEventType;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for SetAdjustmentCommand.
/// </summary>
public class SetAdjustmentCommandHandler : ICommandHandler<SetAdjustmentCommand, SetAdjustmentResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public SetAdjustmentCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<SetAdjustmentResult> HandleAsync(
        SetAdjustmentCommand command,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            return new SetAdjustmentResult
            {
                Success = false,
                ErrorMessage = $"Ticket {command.TicketId} not found."
            };
        }

        var oldAmount = ticket.AdjustmentAmount;

        try
        {
            ticket.SetAdjustment(command.Amount);

            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            // Create audit event
            var auditEvent = AuditEvent.Create(
                AuditEventType.Modified,
                nameof(Ticket),
                ticket.Id,
                command.ProcessedBy.Value,
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    Action = "SetAdjustment",
                    OldAmount = oldAmount,
                    NewAmount = command.Amount,
                    Reason = command.Reason
                }),
                $"Adjustment set to {command.Amount} on ticket {ticket.TicketNumber}. Reason: {command.Reason ?? "N/A"}",
                correlationId: Guid.NewGuid());

            await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

            return new SetAdjustmentResult
            {
                Success = true,
                NewTotalAmount = ticket.TotalAmount
            };
        }
        catch (Exception ex)
        {
            return new SetAdjustmentResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}


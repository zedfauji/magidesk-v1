using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using AuditEventType = Magidesk.Domain.Enumerations.AuditEventType;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for SetDeliveryChargeCommand.
/// </summary>
public class SetDeliveryChargeCommandHandler : ICommandHandler<SetDeliveryChargeCommand, SetDeliveryChargeResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public SetDeliveryChargeCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<SetDeliveryChargeResult> HandleAsync(
        SetDeliveryChargeCommand command,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            return new SetDeliveryChargeResult
            {
                Success = false,
                ErrorMessage = $"Ticket {command.TicketId} not found."
            };
        }

        var oldAmount = ticket.DeliveryChargeAmount;

        try
        {
            ticket.SetDeliveryCharge(command.Amount);

            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            // Create audit event
            var auditEvent = AuditEvent.Create(
                AuditEventType.Modified,
                nameof(Ticket),
                ticket.Id,
                command.ProcessedBy.Value,
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    Action = "SetDeliveryCharge",
                    OldAmount = oldAmount,
                    NewAmount = command.Amount
                }),
                $"Delivery charge set to {command.Amount} on ticket {ticket.TicketNumber}",
                correlationId: Guid.NewGuid());

            await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

            return new SetDeliveryChargeResult
            {
                Success = true,
                NewTotalAmount = ticket.TotalAmount
            };
        }
        catch (Exception ex)
        {
            return new SetDeliveryChargeResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}


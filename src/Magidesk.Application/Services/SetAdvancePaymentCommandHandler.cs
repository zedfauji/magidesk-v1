using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using AuditEventType = Magidesk.Domain.Enumerations.AuditEventType;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for SetAdvancePaymentCommand.
/// </summary>
public class SetAdvancePaymentCommandHandler : ICommandHandler<SetAdvancePaymentCommand, SetAdvancePaymentResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public SetAdvancePaymentCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<SetAdvancePaymentResult> HandleAsync(
        SetAdvancePaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            return new SetAdvancePaymentResult
            {
                Success = false,
                ErrorMessage = $"Ticket {command.TicketId} not found."
            };
        }

        var oldAmount = ticket.AdvanceAmount;

        try
        {
            ticket.SetAdvancePayment(command.Amount);

            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            // Create audit event
            var auditEvent = AuditEvent.Create(
                AuditEventType.Modified,
                nameof(Ticket),
                ticket.Id,
                command.ProcessedBy.Value,
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    Action = "SetAdvancePayment",
                    OldAmount = oldAmount,
                    NewAmount = command.Amount
                }),
                $"Advance payment set to {command.Amount} on ticket {ticket.TicketNumber}",
                correlationId: Guid.NewGuid());

            await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

            return new SetAdvancePaymentResult
            {
                Success = true,
                NewTotalAmount = ticket.TotalAmount
            };
        }
        catch (Exception ex)
        {
            return new SetAdvancePaymentResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}


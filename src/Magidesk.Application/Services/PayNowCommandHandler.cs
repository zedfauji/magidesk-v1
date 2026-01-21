using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for PayNowCommand.
/// </summary>
public class PayNowCommandHandler : ICommandHandler<PayNowCommand, PayNowResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;
    
    public PayNowCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<PayNowResult> HandleAsync(PayNowCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
            if (ticket == null)
            {
                return PayNowResult.Failure($"Ticket {command.TicketId} not found.");
            }

            // 1. Recalculate to ensure totals are correct
            ticket.CalculateTotals();

            // 2. Determine Amount
            var amountToPay = command.Amount > 0 
                ? new Money(command.Amount) 
                : ticket.DueAmount;

            if (amountToPay <= Money.Zero())
            {
                 // Check if already paid
                 if (ticket.Status == TicketStatus.Closed || ticket.DueAmount <= Money.Zero())
                 {
                     return PayNowResult.Failure("Ticket is already paid or has zero balance.");
                 }
            }

            // 3. Create Transaction
            var userId = command.ProcessedBy;
            var terminalId = command.TerminalId;

            var payment = Payment.Create(
                ticket.Id,
                PaymentType.Cash, 
                amountToPay,
                userId,
                terminalId
            );
            
            ticket.AddPayment(payment);

            // 4. Check if settled
            // Recalculate to update DueAmount after payment
            ticket.CalculateTotals();
            
            if (ticket.DueAmount <= Money.Zero())
            {
                ticket.Close(userId);
            }

            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            // 5. Audit
            var audit = AuditEvent.Create(
                AuditEventType.PaymentProcessed,
                nameof(Ticket),
                ticket.Id,
                userId.Value,
                System.Text.Json.JsonSerializer.Serialize(new { Amount = amountToPay.Amount, Action = "PayNow" }),
                $"Pay Now executed for ticket {ticket.TicketNumber}"
            );
            await _auditEventRepository.AddAsync(audit, cancellationToken);
            
            return PayNowResult.Successful(amountToPay.Amount);
        }
        catch (Exception ex)
        {
            // Log? The VM might log too, but good to have safety.
            return PayNowResult.Failure($"Payment Processing Error: {ex.Message}");
        }
    }
}

using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for PayNowCommand.
/// </summary>
public class PayNowCommandHandler : ICommandHandler<PayNowCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;
    
    // We would inject a PaymentProcessor or similar here in a real scenario
    
    public PayNowCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(PayNowCommand command, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // 1. Recalculate to ensure totals are correct
        ticket.CalculateTotals();

        // 2. Determine Amount
        var amountToPay = command.Amount > 0 
            ? new Money(command.Amount) 
            : ticket.DueAmount;

        if (amountToPay <= Money.Zero())
        {
             // Already paid or invalid?
             // If due is 0, we can just close/settle.
        }

        // 3. Create Transaction
        // Using placeholder User/Terminal IDs as they aren't on the command yet. 
        // In a real app, these would come from ICurrentUserService or the command.
        var userId = new UserId(Guid.Parse("11111111-1111-1111-1111-111111111111")); 
        var terminalId = Guid.Empty;

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
            // Ticket doesn't have Settle(), Close() implies completion of transaction
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
    }
}

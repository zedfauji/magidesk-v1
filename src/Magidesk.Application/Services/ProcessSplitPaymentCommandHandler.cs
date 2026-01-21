using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for ProcessSplitPaymentCommand.
/// </summary>
public class ProcessSplitPaymentCommandHandler : ICommandHandler<ProcessSplitPaymentCommand, ProcessSplitPaymentResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICashSessionRepository _cashSessionRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public ProcessSplitPaymentCommandHandler(
        ITicketRepository ticketRepository,
        IPaymentRepository paymentRepository,
        ICashSessionRepository cashSessionRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _paymentRepository = paymentRepository;
        _cashSessionRepository = cashSessionRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<ProcessSplitPaymentResult> HandleAsync(
        ProcessSplitPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // Calculate sum of all payment amounts
        var totalPaymentAmount = command.Payments
            .Select(p => p.Amount)
            .Aggregate(Money.Zero(), (sum, amount) => sum + amount);

        // Get ticket remaining amount (total - already paid)
        var ticketTotal = ticket.TotalAmount;
        var alreadyPaid = ticket.Payments
            .Where(p => !p.IsVoided)
            .Select(p => p.Amount)
            .Aggregate(Money.Zero(), (sum, amount) => sum + amount);
        var remainingAmount = ticketTotal - alreadyPaid;

        // Check for underpayment
        if (totalPaymentAmount < remainingAmount)
        {
            return new ProcessSplitPaymentResult
            {
                PaymentIds = Array.Empty<Guid>(),
                ChangeAmount = Money.Zero(),
                RemainingAmount = remainingAmount - totalPaymentAmount,
                TicketIsPaid = false,
                IsUnderpayment = true
            };
        }

        // Calculate change if overpayment
        var changeAmount = totalPaymentAmount > remainingAmount
            ? totalPaymentAmount - remainingAmount
            : Money.Zero();

        // Generate unique SplitGroupId for this payment group
        var splitGroupId = Guid.NewGuid();
        var paymentIds = new List<Guid>();
        var terminalId = Guid.NewGuid(); // TODO: Get from context/command

        // Create Payment entity for each entry with sequence number
        for (int i = 0; i < command.Payments.Count; i++)
        {
            var entry = command.Payments[i];
            var sequence = i + 1;

            Payment payment = entry.Method switch
            {
                PaymentType.Cash => CashPayment.Create(
                    command.TicketId,
                    entry.Amount,
                    command.ProcessedBy,
                    terminalId,
                    splitGroupId: splitGroupId,
                    splitSequence: sequence),

                PaymentType.CreditCard => CreditCardPayment.Create(
                    command.TicketId,
                    entry.Amount,
                    command.ProcessedBy,
                    terminalId,
                    globalId: null,
                    splitGroupId: splitGroupId,
                    splitSequence: sequence),

                PaymentType.DebitCard => DebitCardPayment.Create(
                    command.TicketId,
                    entry.Amount,
                    command.ProcessedBy,
                    terminalId,
                    globalId: null,
                    splitGroupId: splitGroupId,
                    splitSequence: sequence),

                PaymentType.GiftCertificate => throw new Domain.Exceptions.BusinessRuleViolationException(
                    "Gift certificate payments require additional information and cannot be used in split payments yet."),

                PaymentType.CustomPayment => throw new Domain.Exceptions.BusinessRuleViolationException(
                    "Custom payments require additional information and cannot be used in split payments yet."),

                _ => throw new Domain.Exceptions.BusinessRuleViolationException(
                    $"Payment type {entry.Method} is not supported for split payments.")
            };

            // Add payment to ticket
            ticket.AddPayment(payment);

            // Save payment
            await _paymentRepository.AddAsync(payment, cancellationToken);

            paymentIds.Add(payment.Id);
        }

        // Update ticket
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.PaymentProcessed,
            nameof(Payment),
            splitGroupId,
            command.ProcessedBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new
            {
                SplitGroupId = splitGroupId,
                PaymentCount = command.Payments.Count,
                TotalAmount = totalPaymentAmount,
                ChangeAmount = changeAmount
            }),
            $"Split payment of {totalPaymentAmount} ({command.Payments.Count} payments) processed for ticket {ticket.TicketNumber}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new ProcessSplitPaymentResult
        {
            PaymentIds = paymentIds,
            ChangeAmount = changeAmount,
            RemainingAmount = Money.Zero(),
            TicketIsPaid = ticket.Status == TicketStatus.Paid,
            IsUnderpayment = false
        };
    }
}

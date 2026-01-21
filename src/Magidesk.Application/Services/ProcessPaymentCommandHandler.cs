using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using CashPayment = Magidesk.Domain.Entities.CashPayment;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.DomainServices;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for ProcessPaymentCommand.
/// </summary>
public class ProcessPaymentCommandHandler : ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICashSessionRepository _cashSessionRepository;
    private readonly IAuditEventRepository _auditEventRepository;
    private readonly PaymentDomainService _paymentDomainService;

    public ProcessPaymentCommandHandler(
        ITicketRepository ticketRepository,
        IPaymentRepository paymentRepository,
        ICashSessionRepository cashSessionRepository,
        IAuditEventRepository auditEventRepository,
        PaymentDomainService paymentDomainService)
    {
        _ticketRepository = ticketRepository;
        _paymentRepository = paymentRepository;
        _cashSessionRepository = cashSessionRepository;
        _auditEventRepository = auditEventRepository;
        _paymentDomainService = paymentDomainService;
    }

    public async Task<ProcessPaymentResult> HandleAsync(ProcessPaymentCommand command, CancellationToken cancellationToken = default)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // Create payment (simplified - will be enhanced for different payment types)
        Payment payment;
        Money changeAmount = Money.Zero();

        if (command.PaymentType == PaymentType.Cash)
        {
            // Validate cash session if provided
            if (command.CashSessionId.HasValue)
            {
                var cashSession = await _cashSessionRepository.GetByIdAsync(command.CashSessionId.Value, cancellationToken);
                if (cashSession == null)
                {
                    throw new Domain.Exceptions.BusinessRuleViolationException($"Cash session {command.CashSessionId} not found.");
                }
            }

            // Validate tender amount
            if (command.TenderAmount == null)
            {
                throw new Domain.Exceptions.BusinessRuleViolationException("Tender amount is required for cash payments.");
            }

            if (command.TenderAmount < command.Amount)
            {
                throw new Domain.Exceptions.BusinessRuleViolationException("Tender amount must be greater than or equal to payment amount.");
            }

            // Create cash payment
            var cashPayment = CashPayment.Create(
                command.TicketId,
                command.Amount,
                command.ProcessedBy,
                command.TerminalId,
                command.GlobalId);

            // Set tender amount and calculate change
            if (command.TenderAmount != null)
            {
                // Use reflection to access protected methods
                var setTenderMethod = typeof(Payment).GetMethod("SetTenderAmount", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                setTenderMethod?.Invoke(cashPayment, new object[] { command.TenderAmount });
                
                changeAmount = command.TenderAmount - command.Amount;
                var setChangeMethod = typeof(Payment).GetMethod("SetChangeAmount", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                setChangeMethod?.Invoke(cashPayment, new object[] { changeAmount });
            }

            // Set tips if provided
            if (command.TipsAmount != null && command.TipsAmount > Money.Zero())
            {
                var setTipsMethod = typeof(Payment).GetMethod("SetTipsAmount", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                setTipsMethod?.Invoke(cashPayment, new object[] { command.TipsAmount });
            }

            // Set cash session if provided
            if (command.CashSessionId.HasValue)
            {
                var setCashSessionMethod = typeof(Payment).GetMethod("SetCashSessionId", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                setCashSessionMethod?.Invoke(cashPayment, new object[] { command.CashSessionId.Value });
            }

            payment = cashPayment;
            changeAmount = _paymentDomainService.CalculateChange(payment);
        }
        else if (command.PaymentType == PaymentType.CreditCard || command.PaymentType == PaymentType.DebitCard)
        {
            // Create card payment
            var cardPayment = CreditCardPayment.Create(
                command.TicketId,
                command.Amount,
                command.ProcessedBy,
                command.TerminalId,
                cardNumber: command.Last4, // Storing Last4 as CardNumber for simulation
                cardHolderName: "Simulated User",
                authorizationCode: command.AuthCode ?? "AUTH" + DateTime.Now.Ticks.ToString()[^6..],
                referenceNumber: "REF" + DateTime.Now.Ticks.ToString()[^8..],
                cardType: command.CardType ?? "Generic",
                globalId: command.GlobalId);

            payment = cardPayment;
        }
        else if (command.PaymentType == PaymentType.GiftCertificate)
        {
             // Validate GC Number
             if (string.IsNullOrWhiteSpace(command.GiftCardNumber))
             {
                 throw new Domain.Exceptions.BusinessRuleViolationException("Gift Certificate Number is required.");
             }

             // Simulated GC check: Assume it has enough balance if it's being processed
             // In real world, we'd query a GiftCertificateService here.
             var simulatedBalance = command.Amount + new Money(10.00m, command.Amount.Currency); 

             var gcPayment = GiftCertificatePayment.Create(
                 command.TicketId,
                 command.Amount,
                 command.ProcessedBy,
                 command.TerminalId,
                 command.GiftCardNumber,
                 originalAmount: simulatedBalance, // Simulating original amount > payment
                 remainingBalance: simulatedBalance - command.Amount,
                 globalId: command.GlobalId);

             payment = gcPayment;
        }
        else
        {
            // For Phase 2, other payment types will be supported
            throw new Domain.Exceptions.BusinessRuleViolationException($"Payment type {command.PaymentType} is not yet supported.");
        }

        // Add payment to ticket
        ticket.AddPayment(payment);

        // Save payment
        await _paymentRepository.AddAsync(payment, cancellationToken);

        // Update ticket
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // Update cash session if cash payment
        if (command.PaymentType == PaymentType.Cash && command.CashSessionId.HasValue)
        {
            var cashSession = await _cashSessionRepository.GetByIdAsync(command.CashSessionId.Value, cancellationToken);
            if (cashSession != null)
            {
                cashSession.AddPayment(payment);
                await _cashSessionRepository.UpdateAsync(cashSession, cancellationToken);
            }
        }

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.PaymentProcessed,
            nameof(Payment),
            payment.Id,
            command.ProcessedBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new { PaymentId = payment.Id, Amount = payment.Amount }),
            $"Payment of {payment.Amount} processed for ticket {ticket.TicketNumber}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new ProcessPaymentResult
        {
            PaymentId = payment.Id,
            ChangeAmount = changeAmount,
            TicketIsPaid = ticket.Status == TicketStatus.Paid
        };
    }
}



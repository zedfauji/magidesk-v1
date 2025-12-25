using System;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using AuditEventType = Magidesk.Domain.Enumerations.AuditEventType;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for RefundTicketCommand.
/// Refunds all payments on a ticket.
/// </summary>
public class RefundTicketCommandHandler : ICommandHandler<RefundTicketCommand, RefundTicketResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IAuditEventRepository _auditEventRepository;
    private readonly Domain.DomainServices.PaymentDomainService _paymentDomainService;
    private readonly Domain.DomainServices.TicketDomainService _ticketDomainService;

    public RefundTicketCommandHandler(
        ITicketRepository ticketRepository,
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway,
        IAuditEventRepository auditEventRepository,
        Domain.DomainServices.PaymentDomainService paymentDomainService,
        Domain.DomainServices.TicketDomainService ticketDomainService)
    {
        _ticketRepository = ticketRepository;
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
        _auditEventRepository = auditEventRepository;
        _paymentDomainService = paymentDomainService;
        _ticketDomainService = ticketDomainService;
    }

    public async Task<RefundTicketResult> HandleAsync(
        RefundTicketCommand command,
        CancellationToken cancellationToken = default)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            return new RefundTicketResult
            {
                Success = false,
                ErrorMessage = $"Ticket {command.TicketId} not found."
            };
        }

        // Validate can refund
        if (!_ticketDomainService.CanRefundTicket(ticket, ticket.PaidAmount))
        {
            return new RefundTicketResult
            {
                Success = false,
                ErrorMessage = $"Ticket {ticket.TicketNumber} cannot be refunded."
            };
        }

        int refundPaymentsCreated = 0;

        // Refund each payment
        foreach (var payment in ticket.Payments.Where(p => !p.IsVoided && p.TransactionType == TransactionType.Credit))
        {
            // Validate refund
            if (!_paymentDomainService.CanRefundPayment(payment, payment.Amount))
            {
                continue; // Skip if cannot refund
            }

            // For card payments, call payment gateway
            if (payment is CreditCardPayment creditCardPayment)
            {
                var gatewayResult = await _paymentGateway.RefundAsync(
                    creditCardPayment,
                    payment.Amount,
                    cancellationToken);

                if (!gatewayResult.Success)
                {
                    // Log error but continue with other payments
                    var auditEvent = AuditEvent.Create(
                        AuditEventType.RefundProcessed,
                        nameof(Payment),
                        payment.Id,
                        command.ProcessedBy.Value,
                        System.Text.Json.JsonSerializer.Serialize(new { 
                            Action = "Refund",
                            Success = false,
                            Error = gatewayResult.ErrorMessage
                        }),
                        $"Card payment refund failed: {gatewayResult.ErrorMessage}",
                        correlationId: Guid.NewGuid());

                    await _auditEventRepository.AddAsync(auditEvent, cancellationToken);
                    continue;
                }
            }

            // Create refund payment
            var refundPayment = Payment.CreateRefund(
                payment,
                payment.Amount,
                command.ProcessedBy,
                command.TerminalId,
                command.Reason);

            // Add refund payment
            await _paymentRepository.AddAsync(refundPayment, cancellationToken);
            refundPaymentsCreated++;

            // Process refund on ticket
            ticket.ProcessRefund(refundPayment);

            // Create audit event
            var successAuditEvent = AuditEvent.Create(
                AuditEventType.RefundProcessed,
                nameof(Payment),
                refundPayment.Id,
                command.ProcessedBy.Value,
                System.Text.Json.JsonSerializer.Serialize(new { 
                    Action = "Refund",
                    Success = true,
                    OriginalPaymentId = payment.Id,
                    RefundAmount = payment.Amount
                }),
                $"Payment refunded: {payment.Amount}",
                correlationId: Guid.NewGuid());

            await _auditEventRepository.AddAsync(successAuditEvent, cancellationToken);
        }

        // Update ticket
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // Create ticket-level audit event
        var ticketAuditEvent = AuditEvent.Create(
            AuditEventType.RefundProcessed,
            nameof(Ticket),
            ticket.Id,
            command.ProcessedBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new { 
                Action = "RefundTicket",
                RefundPaymentsCreated = refundPaymentsCreated,
                Reason = command.Reason
            }),
            $"Ticket {ticket.TicketNumber} refunded. {refundPaymentsCreated} refund payments created.",
            correlationId: Guid.NewGuid());

        await _auditEventRepository.AddAsync(ticketAuditEvent, cancellationToken);

        return new RefundTicketResult
        {
            Success = true,
            RefundPaymentsCreated = refundPaymentsCreated
        };
    }
}


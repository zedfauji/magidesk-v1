using System;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using AuditEventType = Magidesk.Domain.Enumerations.AuditEventType;
using Magidesk.Application.Queries; // For RefundMode
using Magidesk.Domain.ValueObjects; // For Money
using System.Linq; // For LINQ extension methods like Contains

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
    private readonly ISecurityService _securityService;

    public RefundTicketCommandHandler(
        ITicketRepository ticketRepository,
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway,
        IAuditEventRepository auditEventRepository,
        Domain.DomainServices.PaymentDomainService paymentDomainService,
        Domain.DomainServices.TicketDomainService ticketDomainService,
        ISecurityService securityService)
    {
        _ticketRepository = ticketRepository;
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
        _auditEventRepository = auditEventRepository;
        _paymentDomainService = paymentDomainService;
        _ticketDomainService = ticketDomainService;
        _securityService = securityService;
    }

    public async Task<RefundTicketResult> HandleAsync(
        RefundTicketCommand command,
        CancellationToken cancellationToken = default)
    {
        // Check Permissions
        if (!await _securityService.HasPermissionAsync(command.ProcessedBy, Domain.Enumerations.UserPermission.RefundTicket, cancellationToken))
        {
             return new RefundTicketResult
             {
                 Success = false,
                 ErrorMessage = "User does not have permission to refund tickets."
             };
        }

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
        List<Payment> paymentsToRefund = new List<Payment>();
        var activePayments = ticket.Payments.Where(p => !p.IsVoided && p.TransactionType == TransactionType.Credit).ToList();

        // UNIFIED REFUND LOGIC:
        // Identify the Total Target Amount to refund.
        // For Full Mode, this is strictly the Ticket's current PaidAmount (remaining balance).
        // For Partial Mode, it is the requested amount (validated against PaidAmount).
        // For Specific Payments, we target the sum of selected payments (but still must not exceed global PaidAmount).

        Money targetRefundAmount;

        if (command.Mode == RefundMode.Full)
        {
            if (ticket.PaidAmount <= Money.Zero())
            {
                return new RefundTicketResult { Success = false, ErrorMessage = "Ticket has no paid amount to refund." };
            }
            targetRefundAmount = ticket.PaidAmount;
        }
        else if (command.Mode == RefundMode.Partial)
        {
            if (command.PartialAmount == null || command.PartialAmount.Amount <= 0)
                return new RefundTicketResult { Success = false, ErrorMessage = "Partial amount required." };

            if (command.PartialAmount > ticket.PaidAmount)
                return new RefundTicketResult { Success = false, ErrorMessage = "Cannot refund more than paid amount." };

            targetRefundAmount = command.PartialAmount;
        }
        else // Specific Payments
        {
            // For specific payments, the "target" is the sum of those payments.
            // However, we must ensure we don't exceed the global PaidAmount (in case they were already partially refunded).
            var specificTotal = paymentsToRefund.Aggregate(Money.Zero(), (sum, p) => sum + p.Amount);
            
            if (specificTotal > ticket.PaidAmount)
            {
                // This implies some of these payments were already refunded!
                // We should clamp to PaidAmount to avoid crash, or error?
                // Clamping is safer for now to avoid the specific crash.
                targetRefundAmount = ticket.PaidAmount;
            }
            else
            {
                targetRefundAmount = specificTotal;
            }
        }

        Money totalRefundedSoFar = new Money(0);
        
        // We process payments (either all active credits, or specific selected ones)
        // We iterate them and refund up to their amount, UNTIL targetRefundAmount is met.
        // Prefer LIFO (Last-In-First-Out) for Partial/Full to unwind most recent transactions first.
        
        var paymentsToProcess = command.Mode == RefundMode.SpecificPayments 
            ? paymentsToRefund // Keep user selection order? Or safe LIFO? Let's use list as likely provided.
            : activePayments.OrderByDescending(x => x.TransactionTime).ToList();

        foreach (var payment in paymentsToProcess)
        {
             if (totalRefundedSoFar >= targetRefundAmount) break;

             // Calculate how much we NEED to refund
             Money remainingNeeded = targetRefundAmount - totalRefundedSoFar;
             
             // Calculate how much we CAN refund on this payment
             Money amountToRefundOnThisPayment;

             if (remainingNeeded >= payment.Amount)
             {
                 amountToRefundOnThisPayment = payment.Amount;
             }
             else
             {
                 amountToRefundOnThisPayment = remainingNeeded;
             }
             
             // Safety check: Don't refund zero
             if (amountToRefundOnThisPayment.Amount <= 0) continue;

            // Validate refund
            if (!_paymentDomainService.CanRefundPayment(payment, amountToRefundOnThisPayment))
            {
                continue; // Skip if cannot refund
            }

            // For card payments, call payment gateway
            if (payment is CreditCardPayment creditCardPayment)
            {
                var gatewayResult = await _paymentGateway.RefundAsync(
                    creditCardPayment,
                    amountToRefundOnThisPayment,
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
            // IMPORTANT: Create a NEW Money instance to avoid EF Core "Owned Type" change tracking key conflicts.
            var refundMoney = new Money(amountToRefundOnThisPayment.Amount, amountToRefundOnThisPayment.Currency);

            var refundPayment = Payment.CreateRefund(
                payment,
                refundMoney,
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
                    RefundAmount = amountToRefundOnThisPayment
                }),
                $"Payment refunded: {amountToRefundOnThisPayment}",
                correlationId: Guid.NewGuid());

            await _auditEventRepository.AddAsync(successAuditEvent, cancellationToken);
            
            totalRefundedSoFar += amountToRefundOnThisPayment;
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


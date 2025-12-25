using System;
using System.Reflection;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using AuditEventType = Magidesk.Domain.Enumerations.AuditEventType;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for RefundPaymentCommand.
/// </summary>
public class RefundPaymentCommandHandler : ICommandHandler<RefundPaymentCommand, RefundPaymentResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IAuditEventRepository _auditEventRepository;
    private readonly Domain.DomainServices.PaymentDomainService _paymentDomainService;

    public RefundPaymentCommandHandler(
        ITicketRepository ticketRepository,
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway,
        IAuditEventRepository auditEventRepository,
        Domain.DomainServices.PaymentDomainService paymentDomainService)
    {
        _ticketRepository = ticketRepository;
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
        _auditEventRepository = auditEventRepository;
        _paymentDomainService = paymentDomainService;
    }

    public async Task<RefundPaymentResult> HandleAsync(
        RefundPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        // Get original payment
        var originalPayment = await _paymentRepository.GetByIdAsync(command.OriginalPaymentId, cancellationToken);
        if (originalPayment == null)
        {
            return new RefundPaymentResult
            {
                Success = false,
                ErrorMessage = $"Payment {command.OriginalPaymentId} not found."
            };
        }

        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(originalPayment.TicketId, cancellationToken);
        if (ticket == null)
        {
            return new RefundPaymentResult
            {
                Success = false,
                ErrorMessage = $"Ticket {originalPayment.TicketId} not found."
            };
        }

        // Validate refund
        if (!_paymentDomainService.CanRefundPayment(originalPayment, command.RefundAmount))
        {
            return new RefundPaymentResult
            {
                Success = false,
                ErrorMessage = "Payment cannot be refunded (validation failed)."
            };
        }

        // For card payments, call payment gateway to process refund
        if (originalPayment is CreditCardPayment creditCardPayment)
        {
            var gatewayResult = await _paymentGateway.RefundAsync(
                creditCardPayment,
                command.RefundAmount,
                cancellationToken);

            if (!gatewayResult.Success)
            {
                // Create audit event for failed refund
                var auditEvent = AuditEvent.Create(
                    AuditEventType.RefundProcessed,
                    nameof(Payment),
                    originalPayment.Id,
                    command.ProcessedBy.Value,
                    System.Text.Json.JsonSerializer.Serialize(new { 
                        Action = "Refund",
                        Success = false,
                        Error = gatewayResult.ErrorMessage,
                        Reason = command.Reason
                    }),
                    $"Card payment refund failed: {gatewayResult.ErrorMessage}",
                    correlationId: Guid.NewGuid());

                await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

                return new RefundPaymentResult
                {
                    Success = false,
                    ErrorMessage = gatewayResult.ErrorMessage
                };
            }
        }

        // Create refund payment
        var refundPayment = Payment.CreateRefund(
            originalPayment,
            command.RefundAmount,
            command.ProcessedBy,
            command.TerminalId,
            command.Reason,
            command.GlobalId);

        // Add refund payment to repository
        await _paymentRepository.AddAsync(refundPayment, cancellationToken);

        // Process refund on ticket
        ticket.ProcessRefund(refundPayment);
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // Create audit event for successful refund
        var successAuditEvent = AuditEvent.Create(
            AuditEventType.RefundProcessed,
            nameof(Payment),
            refundPayment.Id,
            command.ProcessedBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new { 
                Action = "Refund",
                Success = true,
                OriginalPaymentId = originalPayment.Id,
                RefundAmount = command.RefundAmount,
                Reason = command.Reason
            }),
            $"Payment refunded: {command.RefundAmount}. Reason: {command.Reason}",
            correlationId: Guid.NewGuid());

        await _auditEventRepository.AddAsync(successAuditEvent, cancellationToken);

        return new RefundPaymentResult
        {
            Success = true,
            RefundPaymentId = refundPayment.Id,
            TicketId = ticket.Id,
            TicketFullyRefunded = ticket.Status == TicketStatus.Refunded
        };
    }
}


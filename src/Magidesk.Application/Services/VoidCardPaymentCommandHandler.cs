using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using AuditEventType = Magidesk.Domain.Enumerations.AuditEventType;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for VoidCardPaymentCommand.
/// </summary>
public class VoidCardPaymentCommandHandler : ICommandHandler<VoidCardPaymentCommand, VoidCardPaymentResult>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IAuditEventRepository _auditEventRepository;

    public VoidCardPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway,
        IAuditEventRepository auditEventRepository)
    {
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<VoidCardPaymentResult> HandleAsync(
        VoidCardPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        // Get payment
        var payment = await _paymentRepository.GetByIdAsync(command.PaymentId, cancellationToken);
        if (payment == null)
        {
            return new VoidCardPaymentResult
            {
                Success = false,
                ErrorMessage = $"Payment {command.PaymentId} not found."
            };
        }

        // Validate payment type
        if (payment is not CreditCardPayment creditCardPayment)
        {
            return new VoidCardPaymentResult
            {
                Success = false,
                ErrorMessage = $"Payment {command.PaymentId} is not a credit card payment."
            };
        }

        // Validate payment state
        if (creditCardPayment.IsVoided)
        {
            return new VoidCardPaymentResult
            {
                Success = false,
                ErrorMessage = "Payment is already voided."
            };
        }

        if (creditCardPayment.IsCaptured)
        {
            return new VoidCardPaymentResult
            {
                Success = false,
                ErrorMessage = "Cannot void a captured payment. Use refund instead."
            };
        }

        if (string.IsNullOrEmpty(creditCardPayment.AuthorizationCode))
        {
            return new VoidCardPaymentResult
            {
                Success = false,
                ErrorMessage = "Payment must be authorized before void."
            };
        }

        // Call payment gateway
        var gatewayResult = await _paymentGateway.VoidAsync(
            creditCardPayment,
            cancellationToken);

        if (!gatewayResult.Success)
        {
            // Create audit event for failed void
            var auditEvent = AuditEvent.Create(
                AuditEventType.PaymentProcessed,
                nameof(Payment),
                payment.Id,
                command.ProcessedBy.Value,
                System.Text.Json.JsonSerializer.Serialize(new { 
                    Action = "Void",
                    Success = false,
                    Error = gatewayResult.ErrorMessage 
                }),
                $"Card payment void failed: {gatewayResult.ErrorMessage}",
                correlationId: Guid.NewGuid());

            await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

            return new VoidCardPaymentResult
            {
                Success = false,
                ErrorMessage = gatewayResult.ErrorMessage
            };
        }

        // Update payment - void
        creditCardPayment.Void();

        // Save payment
        await _paymentRepository.UpdateAsync(creditCardPayment, cancellationToken);

        // Create audit event
        var successAuditEvent = AuditEvent.Create(
            AuditEventType.Voided, // Use Voided audit event type
            nameof(Payment),
            payment.Id,
            command.ProcessedBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new { 
                Action = "Void",
                Success = true,
                VoidCode = gatewayResult.VoidCode
            }),
            $"Card payment voided: {gatewayResult.VoidCode}",
            correlationId: Guid.NewGuid());

        await _auditEventRepository.AddAsync(successAuditEvent, cancellationToken);

        return new VoidCardPaymentResult
        {
            Success = true,
            VoidCode = gatewayResult.VoidCode
        };
    }
}



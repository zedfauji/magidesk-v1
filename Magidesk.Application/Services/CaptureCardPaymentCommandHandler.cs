using System.Reflection;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using AuditEventType = Magidesk.Domain.Enumerations.AuditEventType;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for CaptureCardPaymentCommand.
/// </summary>
public class CaptureCardPaymentCommandHandler : ICommandHandler<CaptureCardPaymentCommand, CaptureCardPaymentResult>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IAuditEventRepository _auditEventRepository;

    public CaptureCardPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway,
        IAuditEventRepository auditEventRepository)
    {
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<CaptureCardPaymentResult> HandleAsync(
        CaptureCardPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        // Get payment
        var payment = await _paymentRepository.GetByIdAsync(command.PaymentId, cancellationToken);
        if (payment == null)
        {
            return new CaptureCardPaymentResult
            {
                Success = false,
                ErrorMessage = $"Payment {command.PaymentId} not found."
            };
        }

        // Validate payment type
        if (payment is not CreditCardPayment creditCardPayment)
        {
            return new CaptureCardPaymentResult
            {
                Success = false,
                ErrorMessage = $"Payment {command.PaymentId} is not a credit card payment."
            };
        }

        // Validate payment state
        if (creditCardPayment.IsVoided)
        {
            return new CaptureCardPaymentResult
            {
                Success = false,
                ErrorMessage = "Cannot capture a voided payment."
            };
        }

        if (creditCardPayment.IsCaptured)
        {
            return new CaptureCardPaymentResult
            {
                Success = false,
                ErrorMessage = "Payment is already captured."
            };
        }

        if (string.IsNullOrEmpty(creditCardPayment.AuthorizationCode))
        {
            return new CaptureCardPaymentResult
            {
                Success = false,
                ErrorMessage = "Payment must be authorized before capture."
            };
        }

        // Call payment gateway
        var gatewayResult = await _paymentGateway.CaptureAsync(
            creditCardPayment,
            command.Amount,
            cancellationToken);

        if (!gatewayResult.Success)
        {
            // Create audit event for failed capture
            var auditEvent = AuditEvent.Create(
                AuditEventType.PaymentProcessed,
                nameof(Payment),
                payment.Id,
                command.ProcessedBy.Value,
                System.Text.Json.JsonSerializer.Serialize(new { 
                    Action = "Capture",
                    Success = false,
                    Error = gatewayResult.ErrorMessage 
                }),
                $"Card payment capture failed: {gatewayResult.ErrorMessage}",
                correlationId: Guid.NewGuid());

            await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

            return new CaptureCardPaymentResult
            {
                Success = false,
                ErrorMessage = gatewayResult.ErrorMessage
            };
        }

        // Update payment - capture
        creditCardPayment.Capture();

        // Update reference number if provided (using internal method)
        if (!string.IsNullOrEmpty(gatewayResult.ReferenceNumber))
        {
            var updateRefMethod = typeof(CreditCardPayment).GetMethod("UpdateReferenceNumber", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            updateRefMethod?.Invoke(creditCardPayment, new object[] { gatewayResult.ReferenceNumber });
        }

        // Save payment
        await _paymentRepository.UpdateAsync(creditCardPayment, cancellationToken);

        // Create audit event
        var successAuditEvent = AuditEvent.Create(
            AuditEventType.PaymentProcessed,
            nameof(Payment),
            payment.Id,
            command.ProcessedBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new { 
                Action = "Capture",
                Success = true,
                CaptureCode = gatewayResult.CaptureCode,
                ReferenceNumber = gatewayResult.ReferenceNumber
            }),
            $"Card payment captured: {gatewayResult.CaptureCode}",
            correlationId: Guid.NewGuid());

        await _auditEventRepository.AddAsync(successAuditEvent, cancellationToken);

        return new CaptureCardPaymentResult
        {
            Success = true,
            CaptureCode = gatewayResult.CaptureCode,
            ReferenceNumber = gatewayResult.ReferenceNumber
        };
    }
}


using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using AuditEventType = Magidesk.Domain.Enumerations.AuditEventType;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for AuthorizeCardPaymentCommand.
/// </summary>
public class AuthorizeCardPaymentCommandHandler : ICommandHandler<AuthorizeCardPaymentCommand, AuthorizeCardPaymentResult>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IAuditEventRepository _auditEventRepository;

    public AuthorizeCardPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway,
        IAuditEventRepository auditEventRepository)
    {
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<AuthorizeCardPaymentResult> HandleAsync(
        AuthorizeCardPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        // Get payment
        var payment = await _paymentRepository.GetByIdAsync(command.PaymentId, cancellationToken);
        if (payment == null)
        {
            return new AuthorizeCardPaymentResult
            {
                Success = false,
                ErrorMessage = $"Payment {command.PaymentId} not found."
            };
        }

        // Validate payment type
        if (payment is not CreditCardPayment creditCardPayment)
        {
            return new AuthorizeCardPaymentResult
            {
                Success = false,
                ErrorMessage = $"Payment {command.PaymentId} is not a credit card payment."
            };
        }

        // Validate payment state
        if (creditCardPayment.IsVoided)
        {
            return new AuthorizeCardPaymentResult
            {
                Success = false,
                ErrorMessage = "Cannot authorize a voided payment."
            };
        }

        if (creditCardPayment.IsCaptured)
        {
            return new AuthorizeCardPaymentResult
            {
                Success = false,
                ErrorMessage = "Payment is already captured."
            };
        }

        // Call payment gateway
        var gatewayResult = await _paymentGateway.AuthorizeAsync(
            creditCardPayment,
            command.CardNumber,
            command.CardHolderName,
            command.ExpirationDate,
            command.Cvv,
            cancellationToken);

        if (!gatewayResult.Success)
        {
            // Create audit event for failed authorization
            var auditEvent = AuditEvent.Create(
                AuditEventType.PaymentProcessed, // Use existing type, could add PaymentAuthorizationFailed
                nameof(Payment),
                payment.Id,
                command.ProcessedBy.Value,
                System.Text.Json.JsonSerializer.Serialize(new { 
                    Action = "Authorize",
                    Success = false,
                    Error = gatewayResult.ErrorMessage 
                }),
                $"Card payment authorization failed: {gatewayResult.ErrorMessage}",
                correlationId: Guid.NewGuid());

            await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

            return new AuthorizeCardPaymentResult
            {
                Success = false,
                ErrorMessage = gatewayResult.ErrorMessage
            };
        }

        // Update payment with authorization details
        creditCardPayment.Authorize(
            gatewayResult.AuthorizationCode!,
            gatewayResult.ReferenceNumber,
            gatewayResult.CardType);

        // Save payment
        await _paymentRepository.UpdateAsync(creditCardPayment, cancellationToken);

        // Create audit event
        var successAuditEvent = AuditEvent.Create(
            AuditEventType.PaymentProcessed,
            nameof(Payment),
            payment.Id,
            command.ProcessedBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new { 
                Action = "Authorize",
                Success = true,
                AuthorizationCode = gatewayResult.AuthorizationCode,
                ReferenceNumber = gatewayResult.ReferenceNumber
            }),
            $"Card payment authorized: {gatewayResult.AuthorizationCode}",
            correlationId: Guid.NewGuid());

        await _auditEventRepository.AddAsync(successAuditEvent, cancellationToken);

        return new AuthorizeCardPaymentResult
        {
            Success = true,
            AuthorizationCode = gatewayResult.AuthorizationCode,
            ReferenceNumber = gatewayResult.ReferenceNumber,
            CardType = gatewayResult.CardType,
            LastFourDigits = gatewayResult.LastFourDigits
        };
    }
}


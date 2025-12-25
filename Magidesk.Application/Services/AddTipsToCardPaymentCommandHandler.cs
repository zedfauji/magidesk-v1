using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.DomainServices;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using AuditEventType = Magidesk.Domain.Enumerations.AuditEventType;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for AddTipsToCardPaymentCommand.
/// </summary>
public class AddTipsToCardPaymentCommandHandler : ICommandHandler<AddTipsToCardPaymentCommand, AddTipsToCardPaymentResult>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly PaymentDomainService _paymentDomainService;
    private readonly IAuditEventRepository _auditEventRepository;

    public AddTipsToCardPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway,
        PaymentDomainService paymentDomainService,
        IAuditEventRepository auditEventRepository)
    {
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
        _paymentDomainService = paymentDomainService;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<AddTipsToCardPaymentResult> HandleAsync(
        AddTipsToCardPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        // Get payment
        var payment = await _paymentRepository.GetByIdAsync(command.PaymentId, cancellationToken);
        if (payment == null)
        {
            return new AddTipsToCardPaymentResult
            {
                Success = false,
                ErrorMessage = $"Payment {command.PaymentId} not found."
            };
        }

        // Validate payment type
        if (payment is not CreditCardPayment creditCardPayment)
        {
            return new AddTipsToCardPaymentResult
            {
                Success = false,
                ErrorMessage = $"Payment {command.PaymentId} is not a credit card payment."
            };
        }

        // Validate tips can be added
        if (!_paymentDomainService.CanAddTips(payment, command.TipsAmount))
        {
            return new AddTipsToCardPaymentResult
            {
                Success = false,
                ErrorMessage = "Tips cannot be added to this payment."
            };
        }

        // Validate payment state
        if (string.IsNullOrEmpty(creditCardPayment.AuthorizationCode))
        {
            return new AddTipsToCardPaymentResult
            {
                Success = false,
                ErrorMessage = "Payment must be authorized before adding tips."
            };
        }

        if (creditCardPayment.IsCaptured)
        {
            return new AddTipsToCardPaymentResult
            {
                Success = false,
                ErrorMessage = "Cannot add tips to a captured payment."
            };
        }

        // Call payment gateway to add tips
        var gatewayResult = await _paymentGateway.AddTipsAsync(
            creditCardPayment,
            command.TipsAmount,
            cancellationToken);

        if (!gatewayResult.Success)
        {
            // Create audit event for failed tip addition
            var auditEvent = AuditEvent.Create(
                AuditEventType.PaymentProcessed,
                nameof(Payment),
                payment.Id,
                command.ProcessedBy.Value,
                System.Text.Json.JsonSerializer.Serialize(new { 
                    Action = "AddTips",
                    Success = false,
                    Error = gatewayResult.ErrorMessage 
                }),
                $"Add tips to card payment failed: {gatewayResult.ErrorMessage}",
                correlationId: Guid.NewGuid());

            await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

            return new AddTipsToCardPaymentResult
            {
                Success = false,
                ErrorMessage = gatewayResult.ErrorMessage
            };
        }

        // Add tips to payment (inherited from Payment base class)
        creditCardPayment.AddTips(command.TipsAmount);

        // Note: If gateway provides a new authorization code for tips adjustment,
        // we would update it here. For now, the original authorization code remains.

        // Save payment
        await _paymentRepository.UpdateAsync(creditCardPayment, cancellationToken);

        // Create audit event
        var successAuditEvent = AuditEvent.Create(
            AuditEventType.PaymentProcessed,
            nameof(Payment),
            payment.Id,
            command.ProcessedBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new { 
                Action = "AddTips",
                Success = true,
                TipsAmount = command.TipsAmount,
                AuthorizationCode = gatewayResult.AuthorizationCode
            }),
            $"Tips added to card payment: {command.TipsAmount}",
            correlationId: Guid.NewGuid());

        await _auditEventRepository.AddAsync(successAuditEvent, cancellationToken);

        return new AddTipsToCardPaymentResult
        {
            Success = true,
            AuthorizationCode = gatewayResult.AuthorizationCode
        };
    }
}


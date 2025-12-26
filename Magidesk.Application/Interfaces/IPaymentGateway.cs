using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Interface for external payment gateway integration.
/// </summary>
public interface IPaymentGateway
{
    /// <summary>
    /// Authorizes a card payment (reserves funds but doesn't capture).
    /// </summary>
    Task<AuthorizationResult> AuthorizeAsync(
        CreditCardPayment payment,
        string cardNumber,
        string? cardHolderName = null,
        string? expirationDate = null,
        string? cvv = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Captures a previously authorized payment.
    /// </summary>
    Task<CaptureResult> CaptureAsync(
        CreditCardPayment payment,
        Money? amount = null, // If null, captures full authorized amount
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Voids a payment (cancels before capture).
    /// </summary>
    Task<VoidResult> VoidAsync(
        CreditCardPayment payment,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refunds a captured payment.
    /// </summary>
    Task<RefundResult> RefundAsync(
        CreditCardPayment payment,
        Money refundAmount,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds tips to an authorized payment.
    /// </summary>
    Task<AddTipsResult> AddTipsAsync(
        CreditCardPayment payment,
        Money tipsAmount,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes the current batch on the gateway.
    /// </summary>
    Task<BatchCloseResult> CloseBatchAsync(Guid terminalId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of an authorization operation.
/// </summary>
public class AuthorizationResult
{
    public bool Success { get; set; }
    public string? AuthorizationCode { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? ErrorMessage { get; set; }
    public string? CardType { get; set; }
    public string? LastFourDigits { get; set; }
}

/// <summary>
/// Result of a capture operation.
/// </summary>
public class CaptureResult
{
    public bool Success { get; set; }
    public string? CaptureCode { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Result of a void operation.
/// </summary>
public class VoidResult
{
    public bool Success { get; set; }
    public string? VoidCode { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Result of a refund operation.
/// </summary>
public class RefundResult
{
    public bool Success { get; set; }
    public string? RefundCode { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Result of adding tips to a payment.
/// </summary>
public class AddTipsResult
{
    public bool Success { get; set; }
    public string? AuthorizationCode { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Result of a batch close operation.
/// </summary>
public class BatchCloseResult
{
    public bool Success { get; set; }
    public string? GatewayBatchId { get; set; }
    public string? ErrorMessage { get; set; }
    public Money TotalAmount { get; set; } = Money.Zero();
    public int TransactionCount { get; set; }
}


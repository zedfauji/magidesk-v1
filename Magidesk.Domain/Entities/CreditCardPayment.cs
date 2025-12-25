using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a credit card payment transaction.
/// </summary>
public class CreditCardPayment : Payment
{
    public string? CardNumber { get; private set; }
    public string? CardHolderName { get; private set; }
    public string? AuthorizationCode { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public string? CardType { get; private set; } // Visa, MasterCard, Amex, etc.
    public DateTime? AuthorizationTime { get; private set; }
    public DateTime? CaptureTime { get; private set; }

    protected CreditCardPayment()
    {
    }

    protected CreditCardPayment(
        Guid ticketId,
        Money amount,
        UserId processedBy,
        Guid terminalId,
        string? cardNumber = null,
        string? cardHolderName = null,
        string? authorizationCode = null,
        string? referenceNumber = null,
        string? cardType = null,
        string? globalId = null)
        : base(ticketId, PaymentType.CreditCard, amount, processedBy, terminalId, globalId)
    {
        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
        AuthorizationCode = authorizationCode;
        ReferenceNumber = referenceNumber;
        CardType = cardType;
        IsAuthorizable = true; // Credit cards can be authorized
    }

    /// <summary>
    /// Creates a new credit card payment.
    /// </summary>
    public static CreditCardPayment Create(
        Guid ticketId,
        Money amount,
        UserId processedBy,
        Guid terminalId,
        string? cardNumber = null,
        string? cardHolderName = null,
        string? authorizationCode = null,
        string? referenceNumber = null,
        string? cardType = null,
        string? globalId = null)
    {
        return new CreditCardPayment(
            ticketId,
            amount,
            processedBy,
            terminalId,
            cardNumber,
            cardHolderName,
            authorizationCode,
            referenceNumber,
            cardType,
            globalId);
    }

    /// <summary>
    /// Authorizes the payment (for later capture).
    /// </summary>
    public void Authorize(string authorizationCode, string? referenceNumber = null, string? cardType = null)
    {
        if (IsVoided)
        {
            throw new Exceptions.InvalidOperationException("Cannot authorize a voided payment.");
        }

        if (IsCaptured)
        {
            throw new Exceptions.InvalidOperationException("Payment is already captured.");
        }

        AuthorizationCode = authorizationCode;
        if (referenceNumber != null)
        {
            ReferenceNumber = referenceNumber;
        }
        if (cardType != null)
        {
            CardType = cardType;
        }
        AuthorizationTime = DateTime.UtcNow;
        IsAuthorizable = true;
    }

    /// <summary>
    /// Updates the reference number (used after capture).
    /// </summary>
    internal void UpdateReferenceNumber(string referenceNumber)
    {
        ReferenceNumber = referenceNumber;
    }

    /// <summary>
    /// Captures a previously authorized payment.
    /// </summary>
    public new void Capture()
    {
        if (!IsAuthorizable)
        {
            throw new Exceptions.InvalidOperationException("Payment cannot be captured (not authorizable).");
        }

        if (IsCaptured)
        {
            throw new Exceptions.InvalidOperationException("Payment is already captured.");
        }

        if (string.IsNullOrEmpty(AuthorizationCode))
        {
            throw new Exceptions.InvalidOperationException("Payment must be authorized before capture.");
        }

        IsCaptured = true;
        CaptureTime = DateTime.UtcNow;
    }

    // AddTips is inherited from Payment base class
}


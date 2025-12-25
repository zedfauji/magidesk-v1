using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a debit card payment transaction.
/// </summary>
public class DebitCardPayment : Payment
{
    public string? CardNumber { get; protected set; }
    public string? CardHolderName { get; protected set; }
    public string? AuthorizationCode { get; protected set; }
    public string? ReferenceNumber { get; protected set; }
    public string? CardType { get; protected set; }
    public DateTime? AuthorizationTime { get; protected set; }
    public DateTime? CaptureTime { get; protected set; }
    public string? PinNumber { get; protected set; } // Encrypted PIN

    protected DebitCardPayment()
    {
    }

    protected DebitCardPayment(
        Guid ticketId,
        Money amount,
        UserId processedBy,
        Guid terminalId,
        string? cardNumber = null,
        string? cardHolderName = null,
        string? authorizationCode = null,
        string? referenceNumber = null,
        string? cardType = null,
        string? pinNumber = null,
        string? globalId = null)
        : base(ticketId, PaymentType.DebitCard, amount, processedBy, terminalId, globalId)
    {
        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
        AuthorizationCode = authorizationCode;
        ReferenceNumber = referenceNumber;
        CardType = cardType;
        PinNumber = pinNumber;
        IsAuthorizable = true; // Debit cards can be authorized
    }

    /// <summary>
    /// Creates a new debit card payment.
    /// </summary>
    public static DebitCardPayment Create(
        Guid ticketId,
        Money amount,
        UserId processedBy,
        Guid terminalId,
        string? cardNumber = null,
        string? cardHolderName = null,
        string? authorizationCode = null,
        string? referenceNumber = null,
        string? cardType = null,
        string? pinNumber = null,
        string? globalId = null)
    {
        return new DebitCardPayment(
            ticketId,
            amount,
            processedBy,
            terminalId,
            cardNumber,
            cardHolderName,
            authorizationCode,
            referenceNumber,
            cardType,
            pinNumber,
            globalId);
    }

    /// <summary>
    /// Authorizes the payment (for later capture).
    /// </summary>
    public void Authorize(string authorizationCode, string? referenceNumber = null)
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
        ReferenceNumber = referenceNumber;
        AuthorizationTime = DateTime.UtcNow;
        IsAuthorizable = true;
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


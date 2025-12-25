using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a cash payment transaction.
/// </summary>
public class CashPayment : Payment
{
    protected CashPayment()
    {
    }

    internal CashPayment(
        Guid ticketId,
        Money amount,
        UserId processedBy,
        Guid terminalId,
        string? globalId = null)
        : base(ticketId, PaymentType.Cash, amount, processedBy, terminalId, globalId)
    {
        IsAuthorizable = false; // Cash payments are not authorizable
    }

    /// <summary>
    /// Creates a new cash payment.
    /// </summary>
    public static CashPayment Create(
        Guid ticketId,
        Money amount,
        UserId processedBy,
        Guid terminalId,
        string? globalId = null)
    {
        return new CashPayment(ticketId, amount, processedBy, terminalId, globalId);
    }
}


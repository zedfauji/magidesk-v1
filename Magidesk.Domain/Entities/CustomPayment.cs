using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a custom payment transaction (e.g., store credit, loyalty points, etc.).
/// </summary>
public class CustomPayment : Payment
{
    public string PaymentName { get; protected set; } = null!;
    public string? ReferenceNumber { get; protected set; }
    public Dictionary<string, string> Properties { get; protected set; } = new();

    protected CustomPayment()
    {
    }

    protected CustomPayment(
        Guid ticketId,
        Money amount,
        UserId processedBy,
        Guid terminalId,
        string paymentName,
        string? referenceNumber = null,
        Dictionary<string, string>? properties = null,
        string? globalId = null)
        : base(ticketId, PaymentType.CustomPayment, amount, processedBy, terminalId, globalId)
    {
        if (string.IsNullOrWhiteSpace(paymentName))
        {
            throw new ArgumentException("Payment name cannot be null or empty.", nameof(paymentName));
        }

        PaymentName = paymentName;
        ReferenceNumber = referenceNumber;
        Properties = properties ?? new Dictionary<string, string>();
        IsAuthorizable = false; // Custom payments are typically not authorizable
    }

    /// <summary>
    /// Creates a new custom payment.
    /// </summary>
    public static CustomPayment Create(
        Guid ticketId,
        Money amount,
        UserId processedBy,
        Guid terminalId,
        string paymentName,
        string? referenceNumber = null,
        Dictionary<string, string>? properties = null,
        string? globalId = null)
    {
        return new CustomPayment(
            ticketId,
            amount,
            processedBy,
            terminalId,
            paymentName,
            referenceNumber,
            properties,
            globalId);
    }

    /// <summary>
    /// Sets a property value.
    /// </summary>
    public void SetProperty(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Property key cannot be null or empty.", nameof(key));
        }

        Properties[key] = value;
    }

    /// <summary>
    /// Gets a property value.
    /// </summary>
    public string? GetProperty(string key)
    {
        return Properties.TryGetValue(key, out var value) ? value : null;
    }
}


using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to capture a previously authorized card payment.
/// </summary>
public class CaptureCardPaymentCommand
{
    public Guid PaymentId { get; set; }
    public Money? Amount { get; set; } // If null, captures full authorized amount
    public UserId ProcessedBy { get; set; } = null!;
}

/// <summary>
/// Result of card payment capture.
/// </summary>
public class CaptureCardPaymentResult
{
    public bool Success { get; set; }
    public string? CaptureCode { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? ErrorMessage { get; set; }
}


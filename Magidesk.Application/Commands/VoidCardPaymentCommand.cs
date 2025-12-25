using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to void a card payment (cancel before capture).
/// </summary>
public class VoidCardPaymentCommand
{
    public Guid PaymentId { get; set; }
    public UserId ProcessedBy { get; set; } = null!;
}

/// <summary>
/// Result of card payment void.
/// </summary>
public class VoidCardPaymentResult
{
    public bool Success { get; set; }
    public string? VoidCode { get; set; }
    public string? ErrorMessage { get; set; }
}


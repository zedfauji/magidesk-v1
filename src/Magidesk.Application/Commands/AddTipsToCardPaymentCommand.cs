using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to add tips to a card payment.
/// </summary>
public class AddTipsToCardPaymentCommand
{
    public Guid PaymentId { get; set; }
    public Money TipsAmount { get; set; } = null!;
    public UserId ProcessedBy { get; set; } = null!;
}

/// <summary>
/// Result of adding tips to card payment.
/// </summary>
public class AddTipsToCardPaymentResult
{
    public bool Success { get; set; }
    public string? AuthorizationCode { get; set; }
    public string? ErrorMessage { get; set; }
}


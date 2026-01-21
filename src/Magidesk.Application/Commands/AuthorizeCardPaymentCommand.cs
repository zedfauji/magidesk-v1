using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to authorize a card payment.
/// </summary>
public class AuthorizeCardPaymentCommand
{
    public Guid PaymentId { get; set; }
    public string CardNumber { get; set; } = null!;
    public string? CardHolderName { get; set; }
    public string? ExpirationDate { get; set; }
    public string? Cvv { get; set; }
    public UserId ProcessedBy { get; set; } = null!;
    public string? ManualAuthCode { get; set; } // F-0017: Manual Auth Code Bypass
}

/// <summary>
/// Result of card payment authorization.
/// </summary>
public class AuthorizeCardPaymentResult
{
    public bool Success { get; set; }
    public string? AuthorizationCode { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? ErrorMessage { get; set; }
    public string? CardType { get; set; }
    public string? LastFourDigits { get; set; }
}


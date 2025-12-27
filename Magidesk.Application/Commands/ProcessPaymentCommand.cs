using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to process a payment for a ticket.
/// </summary>
public class ProcessPaymentCommand
{
    public Guid TicketId { get; set; }
    public PaymentType PaymentType { get; set; }
    public Money Amount { get; set; } = null!;
    public Money? TenderAmount { get; set; } // For cash payments
    public Money? TipsAmount { get; set; }
    public UserId ProcessedBy { get; set; } = null!;
    public Guid TerminalId { get; set; }
    public Guid? CashSessionId { get; set; } // For cash payments
    public string? Note { get; set; }
    public string? GlobalId { get; set; }
    
    // Card Properties (Simulated for Phase 4)
    public string? CardType { get; set; } // Visa, MasterCard, etc.
    public string? Last4 { get; set; }
    public string? AuthCode { get; set; }
    public string? GiftCardNumber { get; set; }
}

/// <summary>
/// Result of processing a payment.
/// </summary>
public class ProcessPaymentResult
{
    public Guid PaymentId { get; set; }
    public Money ChangeAmount { get; set; } = null!;
    public bool TicketIsPaid { get; set; }
}


using System;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to refund a payment (partial or full).
/// </summary>
public class RefundPaymentCommand
{
    public Guid OriginalPaymentId { get; set; }
    public Money RefundAmount { get; set; } = null!;
    public UserId ProcessedBy { get; set; } = null!;
    public Guid TerminalId { get; set; }
    public string? Reason { get; set; }
    public string? GlobalId { get; set; }
}

/// <summary>
/// Result of refunding a payment.
/// </summary>
public class RefundPaymentResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid RefundPaymentId { get; set; }
    public Guid TicketId { get; set; }
    public bool TicketFullyRefunded { get; set; }
}


using System;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to refund an entire ticket (all payments).
/// </summary>
public class RefundTicketCommand
{
    public Guid TicketId { get; set; }
    public UserId ProcessedBy { get; set; } = null!;
    public Guid TerminalId { get; set; }
    public string? Reason { get; set; }
}

/// <summary>
/// Result of refunding a ticket.
/// </summary>
public class RefundTicketResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int RefundPaymentsCreated { get; set; }
}


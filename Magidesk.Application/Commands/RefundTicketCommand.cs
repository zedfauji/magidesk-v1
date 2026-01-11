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

    /// <summary>
    /// Mode: Full, Partial, or Specific.
    /// </summary>
    public Queries.RefundMode Mode { get; set; }

    /// <summary>
    /// If Partial mode, the total amount to refund.
    /// </summary>
    public Money? PartialAmount { get; set; }

    /// <summary>
    /// If Specific mode, list of specific payment IDs to refund.
    /// </summary>
    public List<Guid> SpecificPaymentIds { get; set; } = new();
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


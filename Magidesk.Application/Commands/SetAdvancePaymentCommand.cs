using System;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to set advance payment amount on a ticket.
/// </summary>
public class SetAdvancePaymentCommand
{
    public Guid TicketId { get; set; }
    public Money Amount { get; set; } = null!;
    public UserId ProcessedBy { get; set; } = null!;
}

/// <summary>
/// Result of setting advance payment.
/// </summary>
public class SetAdvancePaymentResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Money NewTotalAmount { get; set; } = null!;
}


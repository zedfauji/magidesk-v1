using System;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to set adjustment amount on a ticket (can be positive or negative).
/// </summary>
public class SetAdjustmentCommand
{
    public Guid TicketId { get; set; }
    public Money Amount { get; set; } = null!;
    public UserId ProcessedBy { get; set; } = null!;
    public string? Reason { get; set; }
}

/// <summary>
/// Result of setting adjustment.
/// </summary>
public class SetAdjustmentResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Money NewTotalAmount { get; set; } = null!;
}


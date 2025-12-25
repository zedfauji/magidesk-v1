using System;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to set delivery charge on a ticket.
/// </summary>
public class SetDeliveryChargeCommand
{
    public Guid TicketId { get; set; }
    public Money Amount { get; set; } = null!;
    public UserId ProcessedBy { get; set; } = null!;
}

/// <summary>
/// Result of setting delivery charge.
/// </summary>
public class SetDeliveryChargeResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Money NewTotalAmount { get; set; } = null!;
}


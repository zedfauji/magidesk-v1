using System;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to calculate and set service charge based on a percentage rate.
/// </summary>
public class CalculateServiceChargeCommand
{
    public Guid TicketId { get; set; }
    public decimal ServiceChargeRate { get; set; } // e.g., 0.15 for 15%
    public UserId ProcessedBy { get; set; } = null!;
}

/// <summary>
/// Result of calculating service charge.
/// </summary>
public class CalculateServiceChargeResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Money CalculatedAmount { get; set; } = null!;
    public Money NewTotalAmount { get; set; } = null!;
}


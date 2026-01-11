using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to apply gratuity to a ticket.
/// </summary>
public class ApplyGratuityCommand
{
    public Guid TicketId { get; set; }
    public Money Amount { get; set; } = null!;
    public UserId? ServerId { get; set; }
    public UserId ProcessedBy { get; set; } = null!;
}

/// <summary>
/// Result of applying gratuity.
/// </summary>
public class ApplyGratuityResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid? GratuityId { get; set; }
}

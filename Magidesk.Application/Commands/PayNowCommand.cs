using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to process an immediate "Pay Now" action (e.g. Quick Pay Cash).
/// Orchestrates validation, total calculation, and payment processing.
/// </summary>
public class PayNowCommand
{
    public Guid TicketId { get; set; }
    public decimal Amount { get; set; } // If 0, assumes full due amount
    public string TenderType { get; set; } = "CASH"; // Default to Cash
    public bool AutoPrintReceipt { get; set; } = true;
    public UserId ProcessedBy { get; set; } = null!;
    public Guid TerminalId { get; set; }
}

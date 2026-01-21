using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to refund a ticket (full or partial).
/// REQ-5.4, REQ-5.5, REQ-5.6: Refund requires manager authorization, reason, and amount validation.
/// </summary>
public class RefundTicketCommand
{
    /// <summary>
    /// Gets or sets the ID of the ticket to refund.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Gets or sets the refund amount.
    /// REQ-5.5: For partial refunds, this is less than the paid amount.
    /// REQ-5.4: For full refunds, this equals the paid amount.
    /// </summary>
    public Money Amount { get; set; } = null!;

    /// <summary>
    /// Gets or sets the reason for the refund.
    /// REQ-5.6: Reason is required.
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user requesting the refund operation.
    /// </summary>
    public UserId RefundedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the manager who authorized the refund operation.
    /// REQ-5.6: Manager authorization is required for refund operations.
    /// </summary>
    public UserId AuthorizedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets whether this is a partial refund.
    /// True for partial refunds, false for full refunds.
    /// </summary>
    public bool IsPartial { get; set; }
}

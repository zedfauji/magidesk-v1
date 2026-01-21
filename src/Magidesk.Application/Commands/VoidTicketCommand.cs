using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to void a ticket.
/// REQ-5.1, REQ-5.2: Void requires manager authorization and reason.
/// </summary>
public class VoidTicketCommand
{
    /// <summary>
    /// Gets or sets the ID of the ticket to void.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Gets or sets the user requesting the void operation.
    /// </summary>
    public UserId VoidedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the manager who authorized the void operation.
    /// REQ-5.2: Manager authorization is required for void operations.
    /// </summary>
    public UserId AuthorizedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the reason for voiding the ticket.
    /// REQ-5.2: Reason is required.
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}



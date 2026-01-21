using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to close a ticket.
/// </summary>
public class CloseTicketCommand
{
    public Guid TicketId { get; set; }
    public UserId ClosedBy { get; set; } = null!;
}


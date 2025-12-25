using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to void a ticket.
/// </summary>
public class VoidTicketCommand
{
    public Guid TicketId { get; set; }
    public UserId VoidedBy { get; set; } = null!;
}


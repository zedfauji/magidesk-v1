namespace Magidesk.Application.Commands;

/// <summary>
/// Command to settle a ticket explicitly.
/// </summary>
public class SettleTicketCommand
{
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }
}

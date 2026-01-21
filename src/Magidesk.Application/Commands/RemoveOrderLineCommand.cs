namespace Magidesk.Application.Commands;

/// <summary>
/// Command to remove an order line from a ticket.
/// </summary>
public class RemoveOrderLineCommand
{
    public Guid TicketId { get; set; }
    public Guid OrderLineId { get; set; }
}


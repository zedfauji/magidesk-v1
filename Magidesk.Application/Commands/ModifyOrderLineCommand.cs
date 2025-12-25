using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to modify an order line (update quantity).
/// </summary>
public class ModifyOrderLineCommand
{
    public Guid TicketId { get; set; }
    public Guid OrderLineId { get; set; }
    public decimal Quantity { get; set; }
}


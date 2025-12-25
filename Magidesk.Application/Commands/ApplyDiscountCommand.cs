namespace Magidesk.Application.Commands;

/// <summary>
/// Command to apply a discount to a ticket or order line.
/// </summary>
public class ApplyDiscountCommand
{
    public Guid TicketId { get; set; }
    public Guid? OrderLineId { get; set; } // If null, applies to entire ticket
    public Guid DiscountId { get; set; }
}


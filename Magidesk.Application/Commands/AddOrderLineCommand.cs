using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to add an order line to a ticket.
/// </summary>
public class AddOrderLineCommand
{
    public Guid TicketId { get; set; }
    public Guid MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public Money UnitPrice { get; set; } = null!;
    public decimal TaxRate { get; set; }
    public string? CategoryName { get; set; }
    public string? GroupName { get; set; }
}

/// <summary>
/// Result of adding an order line.
/// </summary>
public class AddOrderLineResult
{
    public Guid OrderLineId { get; set; }
}


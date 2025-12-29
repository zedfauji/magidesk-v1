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
    public string? Instructions { get; set; } // F-0036
    public List<Application.DTOs.OrderLineModifierDto>? Modifiers { get; set; } // F-0037
}


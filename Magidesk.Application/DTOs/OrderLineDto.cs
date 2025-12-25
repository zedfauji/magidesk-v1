namespace Magidesk.Application.DTOs;

/// <summary>
/// Data transfer object for OrderLine entity.
/// </summary>
public class OrderLineDto
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public Guid MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public string? GroupName { get; set; }
    public decimal Quantity { get; set; }
    public int ItemCount { get; set; }
    public string? ItemUnitName { get; set; }
    public bool IsFractionalUnit { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsBeverage { get; set; }
    public bool ShouldPrintToKitchen { get; set; }
    public bool PrintedToKitchen { get; set; }
    public int? SeatNumber { get; set; }
    public bool TreatAsSeat { get; set; }
    public List<OrderLineModifierDto> Modifiers { get; set; } = new();
    public List<OrderLineModifierDto> AddOns { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}


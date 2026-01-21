namespace Magidesk.Application.Commands;

/// <summary>
/// Command to update an order type.
/// </summary>
public class UpdateOrderTypeCommand
{
    public Guid OrderTypeId { get; set; }
    public string? Name { get; set; }
    public bool? CloseOnPaid { get; set; }
    public bool? AllowSeatBasedOrder { get; set; }
    public bool? AllowToAddTipsLater { get; set; }
    public bool? IsBarTab { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// Result of updating an order type.
/// </summary>
public class UpdateOrderTypeResult
{
    public Guid OrderTypeId { get; set; }
    public string Name { get; set; } = null!;
}


namespace Magidesk.Application.Commands;

/// <summary>
/// Command to create a new order type.
/// </summary>
public class CreateOrderTypeCommand
{
    public string Name { get; set; } = null!;
    public bool CloseOnPaid { get; set; }
    public bool AllowSeatBasedOrder { get; set; }
    public bool AllowToAddTipsLater { get; set; }
    public bool IsBarTab { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Result of creating an order type.
/// </summary>
public class CreateOrderTypeResult
{
    public Guid OrderTypeId { get; set; }
    public string Name { get; set; } = null!;
}


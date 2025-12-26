using System.Collections.Generic;

namespace Magidesk.Application.DTOs;

/// <summary>
/// DTO for OrderType entity.
/// </summary>
public class OrderTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool CloseOnPaid { get; set; }
    public bool AllowSeatBasedOrder { get; set; }
    public bool AllowToAddTipsLater { get; set; }
    public bool IsBarTab { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, string> Properties { get; set; } = new();
}


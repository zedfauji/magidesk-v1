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

    /// <summary>
    /// Gets a property value from the Properties dictionary.
    /// </summary>
    public string? GetProperty(string key)
    {
        return Properties.TryGetValue(key, out var value) ? value : null;
    }

    // FloreantPOS-aligned helpers
    public bool RequiresTable => GetProperty("RequiresTable")?.ToLower() == "true";
    public bool RequiresCustomer => GetProperty("RequiresCustomer")?.ToLower() == "true";
}


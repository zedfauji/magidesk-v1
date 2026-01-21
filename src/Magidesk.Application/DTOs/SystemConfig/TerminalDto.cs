using System;

namespace Magidesk.Application.DTOs.SystemConfig;

public class TerminalDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TerminalKey { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public Guid? FloorId { get; set; }
    
    public bool HasCashDrawer { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    
    public bool AutoLogOut { get; set; }
    public int AutoLogOutTime { get; set; }
    public bool ShowGuestSelection { get; set; }
    public bool ShowTableSelection { get; set; }
    public bool KitchenMode { get; set; }
    public string DefaultFontSize { get; set; } = "14";
    public string DefaultFontFamily { get; set; } = "Segoe UI";
}

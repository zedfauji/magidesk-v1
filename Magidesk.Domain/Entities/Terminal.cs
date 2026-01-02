using System;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a specific POS terminal/workstation in the restaurant.
/// Carries per-terminal settings and operational state (cash drawer balance).
/// </summary>
public class Terminal
{
    public Guid Id { get; private set; }
    public string Name { get; set; } = string.Empty;
    public string TerminalKey { get; private set; } = string.Empty; // MachineName or unique HW ID
    public string Location { get; set; } = string.Empty;
    public Guid? FloorId { get; set; }
    
    // Operational State
    public bool HasCashDrawer { get; set; } = true;
    public decimal OpeningBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    
    // Terminal Specific Config (Parity with FloreantPOS TerminalConfig)
    public bool AutoLogOut { get; set; }
    public int AutoLogOutTime { get; set; } = 30; // Seconds
    public bool ShowGuestSelection { get; set; } = true;
    public bool ShowTableSelection { get; set; } = true;
    public bool KitchenMode { get; set; }
    public string DefaultFontSize { get; set; } = "14";
    public string DefaultFontFamily { get; set; } = "Segoe UI";

    // Private constructor for EF Core
    private Terminal()
    {
    }

    /// <summary>
    /// Creates a new terminal entry.
    /// </summary>
    public static Terminal Create(string name, string terminalKey)
    {
        if (string.IsNullOrWhiteSpace(terminalKey))
        {
            throw new ArgumentException("Terminal key is required.");
        }

        return new Terminal
        {
            Id = Guid.NewGuid(),
            Name = name ?? terminalKey,
            TerminalKey = terminalKey,
            Location = "Default",
            HasCashDrawer = true,
            OpeningBalance = 0,
            CurrentBalance = 0,
            AutoLogOut = false,
            AutoLogOutTime = 30,
            ShowGuestSelection = true,
            ShowTableSelection = true,
            KitchenMode = false
        };
    }
}

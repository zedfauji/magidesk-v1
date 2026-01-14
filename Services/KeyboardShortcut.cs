using System.Windows.Input;
using Windows.System;

namespace Magidesk.Presentation.Services;

/// <summary>
/// Represents a keyboard shortcut with key, modifiers, and associated command.
/// </summary>
public class KeyboardShortcut
{
    public VirtualKey Key { get; set; }
    public VirtualKeyModifiers Modifiers { get; set; }
    public string ActionName { get; set; } = string.Empty;
    public ICommand Command { get; set; } = null!;

    /// <summary>
    /// Gets a string representation of the shortcut (e.g., "Ctrl+P", "F12").
    /// </summary>
    public string DisplayText
    {
        get
        {
            var parts = new List<string>();

            if (Modifiers.HasFlag(VirtualKeyModifiers.Control))
                parts.Add("Ctrl");
            if (Modifiers.HasFlag(VirtualKeyModifiers.Shift))
                parts.Add("Shift");
            if (Modifiers.HasFlag(VirtualKeyModifiers.Menu))
                parts.Add("Alt");
            if (Modifiers.HasFlag(VirtualKeyModifiers.Windows))
                parts.Add("Win");

            parts.Add(Key.ToString());

            return string.Join("+", parts);
        }
    }
}

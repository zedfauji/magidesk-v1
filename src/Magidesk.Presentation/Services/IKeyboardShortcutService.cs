using System.Windows.Input;
using Windows.System;

namespace Magidesk.Presentation.Services;

/// <summary>
/// Service for managing keyboard shortcuts across the application.
/// </summary>
public interface IKeyboardShortcutService
{
    /// <summary>
    /// Registers a keyboard shortcut with an associated command.
    /// </summary>
    void RegisterShortcut(VirtualKey key, VirtualKeyModifiers modifiers, string actionName, ICommand command);

    /// <summary>
    /// Registers a keyboard shortcut with a simple key (no modifiers).
    /// </summary>
    void RegisterShortcut(VirtualKey key, string actionName, ICommand command);

    /// <summary>
    /// Unregisters a keyboard shortcut.
    /// </summary>
    void UnregisterShortcut(VirtualKey key, VirtualKeyModifiers modifiers);

    /// <summary>
    /// Handles a key press event and executes the associated command if registered.
    /// </summary>
    bool HandleKeyPress(VirtualKey key, VirtualKeyModifiers modifiers);

    /// <summary>
    /// Gets all registered shortcuts.
    /// </summary>
    IReadOnlyDictionary<string, KeyboardShortcut> GetAllShortcuts();
}

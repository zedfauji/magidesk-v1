using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Windows.System;

namespace Magidesk.Presentation.Services;

/// <summary>
/// Service for managing and executing keyboard shortcuts.
/// </summary>
public class KeyboardShortcutService : IKeyboardShortcutService
{
    private readonly Dictionary<string, KeyboardShortcut> _shortcuts = new();
    private readonly ILogger<KeyboardShortcutService>? _logger;

    public KeyboardShortcutService(ILogger<KeyboardShortcutService>? logger = null)
    {
        _logger = logger;
    }

    public void RegisterShortcut(VirtualKey key, VirtualKeyModifiers modifiers, string actionName, ICommand command)
    {
        var shortcutKey = GetShortcutKey(key, modifiers);

        if (_shortcuts.ContainsKey(shortcutKey))
        {
            var existingAction = _shortcuts[shortcutKey].ActionName;
            _logger?.LogWarning(
                "Keyboard shortcut conflict: {Key} already registered for {ExistingAction}, cannot register for {NewAction}",
                shortcutKey,
                existingAction,
                actionName
            );

            throw new InvalidOperationException(
                $"Keyboard shortcut {shortcutKey} is already registered for {existingAction}"
            );
        }

        var shortcut = new KeyboardShortcut
        {
            Key = key,
            Modifiers = modifiers,
            ActionName = actionName,
            Command = command
        };

        _shortcuts[shortcutKey] = shortcut;
        _logger?.LogInformation("Registered keyboard shortcut: {Key} for {Action}", shortcutKey, actionName);
    }

    public void RegisterShortcut(VirtualKey key, string actionName, ICommand command)
    {
        RegisterShortcut(key, VirtualKeyModifiers.None, actionName, command);
    }

    public void UnregisterShortcut(VirtualKey key, VirtualKeyModifiers modifiers)
    {
        var shortcutKey = GetShortcutKey(key, modifiers);
        if (_shortcuts.Remove(shortcutKey))
        {
            _logger?.LogInformation("Unregistered keyboard shortcut: {Key}", shortcutKey);
        }
    }

    public bool HandleKeyPress(VirtualKey key, VirtualKeyModifiers modifiers)
    {
        var shortcutKey = GetShortcutKey(key, modifiers);

        if (_shortcuts.TryGetValue(shortcutKey, out var shortcut))
        {
            if (shortcut.Command.CanExecute(null))
            {
                _logger?.LogDebug("Executing keyboard shortcut: {Key} for {Action}", shortcutKey, shortcut.ActionName);
                shortcut.Command.Execute(null);
                return true;
            }
            else
            {
                _logger?.LogDebug("Keyboard shortcut {Key} cannot execute at this time", shortcutKey);
            }
        }

        return false;
    }

    public IReadOnlyDictionary<string, KeyboardShortcut> GetAllShortcuts()
    {
        return _shortcuts;
    }

    private static string GetShortcutKey(VirtualKey key, VirtualKeyModifiers modifiers)
    {
        return $"{modifiers}+{key}";
    }
}

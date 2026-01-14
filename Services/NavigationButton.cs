using CommunityToolkit.Mvvm.ComponentModel;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Presentation.Services;

/// <summary>
/// Represents a navigation button on the Switchboard with properties for display, routing, and permission-based visibility.
/// </summary>
public partial class NavigationButton : ObservableObject
{
    [ObservableProperty]
    private string _label = string.Empty;

    [ObservableProperty]
    private string _icon = string.Empty; // Segoe Fluent Icons glyph

    [ObservableProperty]
    private string _route = string.Empty;

    [ObservableProperty]
    private string _category = string.Empty; // Operations, Management, Reports, Settings

    [ObservableProperty]
    private bool _isEnabled = true;

    [ObservableProperty]
    private UserPermission _requiredPermission = UserPermission.None;

    [ObservableProperty]
    private string _keyboardShortcut = string.Empty;

    /// <summary>
    /// Determines if the button should be visible based on the user's permissions.
    /// </summary>
    /// <param name="userPermissions">The current user's permissions.</param>
    /// <returns>True if the button should be visible, false otherwise.</returns>
    public bool IsVisibleForUser(UserPermission userPermissions)
    {
        // If no permission is required, the button is always visible
        if (RequiredPermission == UserPermission.None)
        {
            return true;
        }

        // Check if the user has the required permission
        return userPermissions.HasFlag(RequiredPermission);
    }

    /// <summary>
    /// Determines if the button should be enabled based on the user's permissions.
    /// </summary>
    /// <param name="userPermissions">The current user's permissions.</param>
    /// <returns>True if the button should be enabled, false otherwise.</returns>
    public bool IsEnabledForUser(UserPermission userPermissions)
    {
        // Button must be visible and enabled
        return IsEnabled && IsVisibleForUser(userPermissions);
    }
}

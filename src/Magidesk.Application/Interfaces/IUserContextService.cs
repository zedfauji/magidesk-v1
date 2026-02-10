namespace Magidesk.Application.Interfaces;

/// <summary>
/// Provides the current user context for the application.
/// This is the single source of truth for logged-in user identity.
/// </summary>
public interface IUserContextService
{
    /// <summary>
    /// Gets the current logged-in user's ID.
    /// Returns Guid.Empty if no user is logged in.
    /// </summary>
    Guid GetCurrentUserId();
    
    /// <summary>
    /// Checks if the current user has the specified role.
    /// </summary>
    /// <param name="role">The role name to check</param>
    /// <returns>True if the current user has the specified role, false otherwise</returns>
    bool IsInRole(string role);
    
    /// <summary>
    /// Requires a manager override for a sensitive operation.
    /// Shows the manager override dialog and returns true if override was successful.
    /// </summary>
    /// <param name="reason">The reason for requiring manager override</param>
    /// <returns>True if manager override was successful, false otherwise</returns>
    Task<bool> RequireManagerOverrideAsync(string reason);
}

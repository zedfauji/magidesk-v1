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
    /// Triggers a manager override process. 
    /// Returns a result containing whether it was successful and the authorizing manager's ID.
    /// </summary>
    /// <param name="reason">The reason for requiring manager override</param>
    Task<ManagerOverrideResult> RequireManagerOverrideAsync(string reason);
}

public record ManagerOverrideResult(bool Success, Guid? ManagerId);

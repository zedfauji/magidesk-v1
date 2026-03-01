using Magidesk.Application.DTOs;

namespace Magidesk.Application.Interfaces;

public interface IUserService
{
    UserDto? CurrentUser { get; set; }
    
    event EventHandler<UserDto?>? UserChanged;
    
    Task UpdatePreferredLanguageAsync(string languageCode);
    
    /// <summary>
    /// Gets the current logged-in user's ID, or Guid.Empty if no user is logged in
    /// </summary>
    Guid GetCurrentUserId();
    
    /// <summary>
    /// Checks if the current user has the specified role
    /// </summary>
    bool IsInRole(string role);
    
    /// <summary>
    /// Shows a manager override dialog and returns the result
    /// </summary>
    Task<ManagerOverrideResult> RequireManagerOverrideAsync(string reason);
}

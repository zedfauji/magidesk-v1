using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using System.Security.Claims;

namespace Magidesk.Api.Infrastructure;

public class HttpUserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private UserDto? _currentUser;

    public HttpUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UserDto? CurrentUser
    {
        get
        {
            if (_currentUser != null) return _currentUser;
            
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true) return null;

            // Extract claims
            // Assuming AuthController puts Id in NameIdentifier and Role in Role
            var idStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var name = user.FindFirst(ClaimTypes.Name)?.Value;
            var role = user.FindFirst(ClaimTypes.Role)?.Value;

            if (Guid.TryParse(idStr, out var userId))
            {
                _currentUser = new UserDto
                {
                    Id = userId,
                    Username = name ?? "Unknown",
                    RoleName = role ?? "Server"
                };
            }

            return _currentUser;
        }
        set
        {
            // In HTTP Context, setting the user manually is rare but might be used by login flow
            _currentUser = value;
            UserChanged?.Invoke(this, value);
        }
    }

    public event EventHandler<UserDto?>? UserChanged;

    public Task UpdatePreferredLanguageAsync(string languageCode)
    {
        // No-op for API currently, or could write to User Profile in DB
        return Task.CompletedTask;
    }

    public Guid GetCurrentUserId()
    {
        return CurrentUser?.Id ?? Guid.Empty;
    }

    public bool IsInRole(string role)
    {
        if (CurrentUser == null || string.IsNullOrWhiteSpace(role))
            return false;

        return CurrentUser.RoleName?.Equals(role, StringComparison.OrdinalIgnoreCase) ?? false;
    }

    public Task<ManagerOverrideResult> RequireManagerOverrideAsync(string reason)
    {
        // Manager override is not applicable in API context
        // This would need to be handled by the client application
        return Task.FromResult(new ManagerOverrideResult(false, null));
    }
}

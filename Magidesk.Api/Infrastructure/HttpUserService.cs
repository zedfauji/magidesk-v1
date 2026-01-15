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
                    Id = idStr,
                    Username = name ?? "Unknown",
                    Role = role ?? "Server"
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
}

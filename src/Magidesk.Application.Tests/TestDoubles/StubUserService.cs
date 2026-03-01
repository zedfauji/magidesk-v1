using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Tests.TestDoubles;

public class StubUserService : IUserService
{
    public UserDto? CurrentUser { get; set; }

    public event EventHandler<UserDto?>? UserChanged;

    public StubUserService()
    {
        CurrentUser = new UserDto
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Username = "testuser"
        };
    }

    public StubUserService(UserDto user)
    {
        CurrentUser = user;
    }

    public Task UpdatePreferredLanguageAsync(string languageCode)
    {
        return Task.CompletedTask;
    }

    public Guid GetCurrentUserId()
    {
        return CurrentUser?.Id ?? Guid.Empty;
    }

    public bool IsInRole(string role)
    {
        return CurrentUser?.RoleName?.Equals(role, StringComparison.OrdinalIgnoreCase) ?? false;
    }

    public Task<ManagerOverrideResult> RequireManagerOverrideAsync(string reason)
    {
        return Task.FromResult(new ManagerOverrideResult(true, Guid.NewGuid())); // Stub always succeeds
    }
}

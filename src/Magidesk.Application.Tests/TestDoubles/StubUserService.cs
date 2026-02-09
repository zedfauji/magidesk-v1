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
}

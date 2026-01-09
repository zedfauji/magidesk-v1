using System;
using System.Threading.Tasks;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;

namespace Magidesk.Api.Services;

public class StubUserService : IUserService
{
    public UserDto? CurrentUser { get; set; } = new UserDto { Id = Guid.NewGuid(), FirstName = "Migration", LastName = "User" };

    public event EventHandler<UserDto?>? UserChanged;

    public Task UpdatePreferredLanguageAsync(string languageCode)
    {
        return Task.CompletedTask;
    }
}

using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Magidesk.Presentation.Services;

public class UserService : IUserService
{
    private UserDto? _currentUser;

    public UserDto? CurrentUser 
    { 
        get => _currentUser;
        set
        {
            if (_currentUser != value)
            {
                _currentUser = value;
                UserChanged?.Invoke(this, _currentUser);
            }
        }
    }

    public event EventHandler<UserDto?>? UserChanged;

    private readonly Microsoft.Extensions.DependencyInjection.IServiceScopeFactory _scopeFactory;

    public UserService(Microsoft.Extensions.DependencyInjection.IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task UpdatePreferredLanguageAsync(string languageCode)
    {
        if (_currentUser == null) return;

        using (var scope = _scopeFactory.CreateScope())
        {
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            // var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>(); // Assuming UoW or just repo save
            
            var user = await userRepo.GetByIdAsync(_currentUser.Id);
            if (user != null)
            {
                user.SetPreferredLanguage(languageCode);
                await userRepo.UpdateAsync(user);
                // await unitOfWork.CommitAsync(); // If needed, but usually repo.Update/Save handles it or is auto-commit in EF simple implementation
            }
        }

        // Update local state
        if (_currentUser != null)
        {
            _currentUser.PreferredLanguage = languageCode;
            UserChanged?.Invoke(this, _currentUser);
        }
    }
}

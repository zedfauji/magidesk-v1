using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Magidesk.Presentation.Services;

public class UserService : IUserService, IUserContextService
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
    private readonly IServiceProvider _serviceProvider;

    public UserService(
        Microsoft.Extensions.DependencyInjection.IServiceScopeFactory scopeFactory,
        IServiceProvider serviceProvider)
    {
        _scopeFactory = scopeFactory;
        _serviceProvider = serviceProvider;
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

    public Guid GetCurrentUserId()
    {
        return _currentUser?.Id ?? Guid.Empty;
    }

    public bool IsInRole(string role)
    {
        if (_currentUser == null || string.IsNullOrWhiteSpace(role))
            return false;

        return _currentUser.RoleName?.Equals(role, StringComparison.OrdinalIgnoreCase) ?? false;
    }

    public async Task<ManagerOverrideResult> RequireManagerOverrideAsync(string reason)
    {
        try
        {
            // Use the existing ManagerPinDialog for authorization
            var dialog = new Views.Dialogs.ManagerPinDialog();
            
            // Set XamlRoot from the main window
            if (App.MainWindowInstance?.Content is Microsoft.UI.Xaml.FrameworkElement element)
            {
                dialog.XamlRoot = element.XamlRoot;
            }
            
            var result = await dialog.ShowForOperationAsync(reason);
            
            return new ManagerOverrideResult(
                result?.Authorized ?? false, 
                result?.AuthorizingUserId);
        }
        catch
        {
            // If dialog fails to show or any error occurs, deny the override
            return new ManagerOverrideResult(false, null);
        }
    }
}


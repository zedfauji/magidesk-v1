using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;

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
}

using Magidesk.Application.DTOs;

namespace Magidesk.Application.Interfaces;

public interface IUserService
{
    UserDto? CurrentUser { get; set; }
    
    event EventHandler<UserDto?>? UserChanged;
}

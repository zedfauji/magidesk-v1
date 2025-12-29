using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;

namespace Magidesk.Presentation.Services;

public class UserService : IUserService
{
    public UserDto? CurrentUser { get; set; }
}

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to log out the current user.
/// </summary>
public class LogoutCommand
{
    public Guid UserId { get; set; }
}

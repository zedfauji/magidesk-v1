namespace Magidesk.Api.Dtos.Auth;

public class LoginRequest
{
    public string Pin { get; set; } = string.Empty;
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class AuthSessionDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
    public string TerminalId { get; set; } = string.Empty;
    public string StartedAt { get; set; } = string.Empty;
}

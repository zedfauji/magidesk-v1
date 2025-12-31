using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public Guid? TerminalId { get; set; }
    
    public string FullName => $"{FirstName} {LastName}";
}

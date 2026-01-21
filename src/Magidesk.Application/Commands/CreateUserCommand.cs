using System;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Commands;

public class CreateUserCommand
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty; // For internal use
    public string Pin { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
}

public class CreateUserResult
{
    public Guid UserId { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}

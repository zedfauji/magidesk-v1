using System;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Commands;

public class UpdateUserCommand
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty; // Optional: empty means no change
    public Guid RoleId { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateUserResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}

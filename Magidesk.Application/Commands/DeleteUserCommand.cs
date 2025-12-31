using System;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Commands;

public class DeleteUserCommand
{
    public Guid UserId { get; set; }
}

public class DeleteUserResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}

using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to clock in a user.
/// </summary>
public class ClockInCommand
{
    public UserId UserId { get; set; }
}

/// <summary>
/// Command to clock out a user.
/// </summary>
public class ClockOutCommand
{
    public UserId UserId { get; set; }
}

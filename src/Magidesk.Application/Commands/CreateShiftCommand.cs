namespace Magidesk.Application.Commands;

/// <summary>
/// Command to create a new shift.
/// </summary>
public class CreateShiftCommand
{
    public string Name { get; set; } = null!;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Result of creating a shift.
/// </summary>
public class CreateShiftResult
{
    public Guid ShiftId { get; set; }
    public string Name { get; set; } = null!;
}


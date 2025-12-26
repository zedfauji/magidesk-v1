namespace Magidesk.Application.Commands;

/// <summary>
/// Command to update a shift.
/// </summary>
public class UpdateShiftCommand
{
    public Guid ShiftId { get; set; }
    public string? Name { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// Result of updating a shift.
/// </summary>
public class UpdateShiftResult
{
    public Guid ShiftId { get; set; }
    public string Name { get; set; } = null!;
}


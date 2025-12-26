using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries;

/// <summary>
/// Query to get the current active shift based on current time.
/// </summary>
public class GetCurrentShiftQuery
{
    // No parameters - uses current time
}

/// <summary>
/// Result of getting current shift.
/// </summary>
public class GetCurrentShiftResult
{
    public ShiftDto? Shift { get; set; }
}


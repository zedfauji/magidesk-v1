using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries;

/// <summary>
/// Query to get a shift by ID.
/// </summary>
public class GetShiftQuery
{
    public Guid ShiftId { get; set; }
}

/// <summary>
/// Result of getting a shift.
/// </summary>
public class GetShiftResult
{
    public ShiftDto? Shift { get; set; }
}


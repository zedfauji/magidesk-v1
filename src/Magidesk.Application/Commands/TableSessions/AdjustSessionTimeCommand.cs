using System;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Command to adjust the session time (manager override).
/// </summary>
public record AdjustSessionTimeCommand(
    Guid SessionId,
    TimeSpan AdjustmentAmount,
    string Reason
);

/// <summary>
/// Result of adjusting session time.
/// </summary>
public record AdjustSessionTimeResult(
    Guid SessionId,
    TimeSpan NewBillableTime,
    TimeSpan TotalManualAdjustment
);

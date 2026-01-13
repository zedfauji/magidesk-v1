using System;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Enhanced command to resume a paused table session with validation.
/// </summary>
public record EnhancedResumeSessionCommand(
    Guid SessionId,
    Guid? StaffId = null
);

/// <summary>
/// Enhanced result of resuming a session with detailed information.
/// </summary>
public record EnhancedResumeSessionResult(
    Guid SessionId,
    DateTime ResumedAt,
    TimeSpan TotalPausedDuration,
    decimal CurrentCharge,
    string Status
);
using System;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Enhanced command to pause a table session with validation and reason tracking.
/// </summary>
public record EnhancedPauseSessionCommand(
    Guid SessionId,
    string Reason,
    Guid? StaffId = null
);

/// <summary>
/// Enhanced result of pausing a session with detailed information.
/// </summary>
public record EnhancedPauseSessionResult(
    Guid SessionId,
    DateTime PausedAt,
    TimeSpan TotalPausedDuration,
    decimal CurrentCharge,
    string Status
);
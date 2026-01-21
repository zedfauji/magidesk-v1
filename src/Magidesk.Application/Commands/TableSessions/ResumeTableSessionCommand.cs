using System;
using MediatR;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Command to resume a paused table session.
/// </summary>
/// <param name="SessionId">The ID of the session to resume</param>
public record ResumeTableSessionCommand(Guid SessionId);

/// <summary>
/// Result of resuming a session.
/// </summary>
/// <param name="SessionId">The ID of the resumed session</param>
/// <param name="ResumedAt">When the session was resumed</param>
/// <param name="TotalPausedDuration">Total duration the session was paused</param>
public record ResumeTableSessionResult(Guid SessionId, DateTime ResumedAt, TimeSpan TotalPausedDuration);

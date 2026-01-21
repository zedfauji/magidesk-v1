using System;
using MediatR;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Command to pause a table session.
/// </summary>
public record PauseTableSessionCommand(Guid SessionId);

/// <summary>
/// Result of pausing a session.
/// </summary>
/// <param name="SessionId">The ID of the paused session</param>
/// <param name="PausedAt">When the session was paused</param>
public record PauseTableSessionResult(Guid SessionId, DateTime PausedAt);

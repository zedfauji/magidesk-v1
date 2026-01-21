using System;

namespace Magidesk.Application.Commands.ManagerOverrides;

/// <summary>
/// Command for manager to force end a session for emergency situations.
/// </summary>
public record ForceEndSessionCommand(
    Guid SessionId,
    string Reason,
    string ManagerPin,
    Guid ManagerId
);

/// <summary>
/// Result of forcing session end.
/// </summary>
public record ForceEndSessionResult(
    Guid SessionId,
    string OriginalStatus,
    decimal FinalCharge,
    TimeSpan TotalDuration,
    string Reason,
    Guid ManagerId,
    DateTime EndedAt
);
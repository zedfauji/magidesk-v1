using System;

namespace Magidesk.Application.Commands.ManagerOverrides;

/// <summary>
/// Command for manager to apply time adjustment override with authorization.
/// </summary>
public record ApplyTimeAdjustmentCommand(
    Guid SessionId,
    TimeSpan AdjustmentAmount,
    string Reason,
    string ManagerPin,
    Guid ManagerId
);

/// <summary>
/// Result of applying time adjustment override.
/// </summary>
public record ApplyTimeAdjustmentResult(
    Guid SessionId,
    TimeSpan OriginalBillableTime,
    TimeSpan NewBillableTime,
    TimeSpan AdjustmentApplied,
    decimal OriginalCharge,
    decimal NewCharge,
    Guid ManagerId,
    DateTime AppliedAt
);
using System;

namespace Magidesk.Application.Commands.ManagerOverrides;

/// <summary>
/// Command for manager to apply pricing override with reason code requirements.
/// </summary>
public record ApplyPricingOverrideCommand(
    Guid SessionId,
    decimal OverrideAmount,
    string Reason,
    string ManagerPin,
    Guid ManagerId
);

/// <summary>
/// Result of applying pricing override.
/// </summary>
public record ApplyPricingOverrideResult(
    Guid SessionId,
    decimal OriginalCharge,
    decimal NewCharge,
    decimal OverrideAmount,
    string Reason,
    Guid ManagerId,
    DateTime AppliedAt
);
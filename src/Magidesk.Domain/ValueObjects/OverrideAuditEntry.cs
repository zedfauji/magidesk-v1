using System;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Represents an immutable audit entry for manager override operations.
/// Used for compliance and tracking of all override activities.
/// </summary>
public sealed record OverrideAuditEntry(
    Guid Id,
    Guid SessionId,
    OverrideType OverrideType,
    string OriginalValue,
    string NewValue,
    string Reason,
    Guid ManagerId,
    DateTime Timestamp
)
{
    /// <summary>
    /// Creates a new override audit entry.
    /// </summary>
    /// <param name="sessionId">ID of the session that was overridden</param>
    /// <param name="overrideType">Type of override performed</param>
    /// <param name="originalValue">Original value before override</param>
    /// <param name="newValue">New value after override</param>
    /// <param name="reason">Reason for the override</param>
    /// <param name="managerId">ID of the manager who performed the override</param>
    /// <returns>New OverrideAuditEntry instance</returns>
    public static OverrideAuditEntry Create(
        Guid sessionId,
        OverrideType overrideType,
        string originalValue,
        string newValue,
        string reason,
        Guid managerId)
    {
        if (sessionId == Guid.Empty)
        {
            throw new ArgumentException("Session ID cannot be empty.", nameof(sessionId));
        }

        if (managerId == Guid.Empty)
        {
            throw new ArgumentException("Manager ID cannot be empty.", nameof(managerId));
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason cannot be empty.", nameof(reason));
        }

        return new OverrideAuditEntry(
            Id: Guid.NewGuid(),
            SessionId: sessionId,
            OverrideType: overrideType,
            OriginalValue: originalValue ?? string.Empty,
            NewValue: newValue ?? string.Empty,
            Reason: reason.Trim(),
            ManagerId: managerId,
            Timestamp: DateTime.UtcNow
        );
    }

    /// <summary>
    /// Gets a human-readable description of the override operation.
    /// </summary>
    /// <returns>Description of the override</returns>
    public string GetDescription()
    {
        return OverrideType switch
        {
            OverrideType.TimeAdjustment => $"Time adjusted from {OriginalValue} to {NewValue}",
            OverrideType.PricingOverride => $"Price overridden from {OriginalValue} to {NewValue}",
            OverrideType.ForceEndSession => $"Session force ended (was {OriginalValue})",
            OverrideType.GuestCountOverride => $"Guest count changed from {OriginalValue} to {NewValue}",
            OverrideType.RateOverride => $"Hourly rate changed from {OriginalValue} to {NewValue}",
            _ => $"Override: {OriginalValue} → {NewValue}"
        };
    }

    /// <summary>
    /// Checks if this override represents a significant change.
    /// </summary>
    /// <returns>True if the change is considered significant</returns>
    public bool IsSignificantChange()
    {
        return OverrideType switch
        {
            OverrideType.PricingOverride => true,
            OverrideType.ForceEndSession => true,
            OverrideType.RateOverride => true,
            OverrideType.TimeAdjustment => !string.Equals(OriginalValue, NewValue, StringComparison.OrdinalIgnoreCase),
            OverrideType.GuestCountOverride => !string.Equals(OriginalValue, NewValue, StringComparison.OrdinalIgnoreCase),
            _ => true
        };
    }
}
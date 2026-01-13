using System;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Represents an alert for a session that requires attention.
/// </summary>
public record SessionAlert(
    Guid SessionId,
    Guid TableId,
    SessionAlertType AlertType,
    string Message,
    DateTime CreatedAt,
    SessionAlertSeverity Severity = SessionAlertSeverity.Medium
);

/// <summary>
/// Types of session alerts.
/// </summary>
public enum SessionAlertType
{
    /// <summary>
    /// Session has been paused for an extended period.
    /// </summary>
    LongPause,
    
    /// <summary>
    /// Table capacity may be exceeded.
    /// </summary>
    CapacityIssue,
    
    /// <summary>
    /// Session has been running for an unusually long time.
    /// </summary>
    LongSession,
    
    /// <summary>
    /// Equipment maintenance is required.
    /// </summary>
    EquipmentMaintenance,
    
    /// <summary>
    /// General session issue requiring attention.
    /// </summary>
    General
}

/// <summary>
/// Severity levels for session alerts.
/// </summary>
public enum SessionAlertSeverity
{
    /// <summary>
    /// Low priority alert.
    /// </summary>
    Low,
    
    /// <summary>
    /// Medium priority alert.
    /// </summary>
    Medium,
    
    /// <summary>
    /// High priority alert requiring immediate attention.
    /// </summary>
    High,
    
    /// <summary>
    /// Critical alert requiring urgent action.
    /// </summary>
    Critical
}
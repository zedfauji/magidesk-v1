using System;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.DTOs;

/// <summary>
/// DTO for session alerts in monitoring dashboard.
/// </summary>
public class SessionAlertDto
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public Guid TableId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public SessionAlertType AlertType { get; set; }
    public string AlertMessage { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public SessionAlertSeverity Severity { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public Guid? AcknowledgedBy { get; set; }
}

/// <summary>
/// Types of session alerts.
/// </summary>
public enum SessionAlertType
{
    LongPause,
    LongRunning,
    EquipmentIssue,
    CapacityWarning,
    MaintenanceRequired,
    SystemIssue
}

/// <summary>
/// Session alert severity levels.
/// </summary>
public enum SessionAlertSeverity
{
    Info,
    Warning,
    Error,
    Critical
}
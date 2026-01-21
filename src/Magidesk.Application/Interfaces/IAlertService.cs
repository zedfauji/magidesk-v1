using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Service interface for generating and managing alerts and notifications.
/// </summary>
public interface IAlertService
{
    /// <summary>
    /// Generates an alert for a long-paused session.
    /// </summary>
    /// <param name="sessionId">ID of the paused session</param>
    /// <param name="pauseDuration">How long the session has been paused</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the operation</returns>
    Task GenerateLongPauseAlertAsync(Guid sessionId, TimeSpan pauseDuration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates an alert for equipment maintenance.
    /// </summary>
    /// <param name="equipmentId">ID of the equipment</param>
    /// <param name="maintenanceType">Type of maintenance required</param>
    /// <param name="dueDate">When maintenance is due</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the operation</returns>
    Task GenerateEquipmentMaintenanceAlertAsync(
        Guid equipmentId, 
        string maintenanceType, 
        DateTime dueDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates an alert for capacity issues.
    /// </summary>
    /// <param name="message">Alert message</param>
    /// <param name="severity">Alert severity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the operation</returns>
    Task GenerateCapacityAlertAsync(
        string message, 
        AlertSeverity severity, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a system performance alert.
    /// </summary>
    /// <param name="metric">Performance metric name</param>
    /// <param name="currentValue">Current metric value</param>
    /// <param name="threshold">Alert threshold</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the operation</returns>
    Task GeneratePerformanceAlertAsync(
        string metric, 
        double currentValue, 
        double threshold, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active alerts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active alerts</returns>
    Task<IEnumerable<Alert>> GetActiveAlertsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets alerts by severity level.
    /// </summary>
    /// <param name="severity">Alert severity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of alerts with the specified severity</returns>
    Task<IEnumerable<Alert>> GetAlertsBySeverityAsync(AlertSeverity severity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Acknowledges an alert.
    /// </summary>
    /// <param name="alertId">ID of the alert</param>
    /// <param name="userId">ID of the user acknowledging the alert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the operation</returns>
    Task AcknowledgeAlertAsync(Guid alertId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves an alert.
    /// </summary>
    /// <param name="alertId">ID of the alert</param>
    /// <param name="userId">ID of the user resolving the alert</param>
    /// <param name="resolution">Resolution notes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the operation</returns>
    Task ResolveAlertAsync(
        Guid alertId, 
        Guid userId, 
        string resolution, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears expired alerts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of alerts cleared</returns>
    Task<int> ClearExpiredAlertsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents an alert in the system.
/// </summary>
public record Alert(
    Guid Id,
    AlertType Type,
    AlertSeverity Severity,
    string Title,
    string Message,
    Guid? EntityId,
    string? EntityType,
    DateTime CreatedAt,
    DateTime? AcknowledgedAt,
    Guid? AcknowledgedBy,
    DateTime? ResolvedAt,
    Guid? ResolvedBy,
    string? Resolution,
    DateTime? ExpiresAt,
    bool IsActive
);

/// <summary>
/// Types of alerts in the system.
/// </summary>
public enum AlertType
{
    SessionPause,
    EquipmentMaintenance,
    Capacity,
    Performance,
    System,
    Security
}

/// <summary>
/// Alert severity levels.
/// </summary>
public enum AlertSeverity
{
    Info,
    Warning,
    Error,
    Critical
}
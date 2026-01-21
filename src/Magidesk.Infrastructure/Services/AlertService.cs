using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Magidesk.Infrastructure.Services;

/// <summary>
/// Service implementation for generating and managing alerts and notifications.
/// </summary>
public class AlertService : IAlertService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<AlertService> _logger;

    public AlertService(ApplicationDbContext dbContext, ILogger<AlertService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task GenerateLongPauseAlertAsync(Guid sessionId, TimeSpan pauseDuration, CancellationToken cancellationToken = default)
    {
        var alert = new AlertEntity
        {
            Id = Guid.NewGuid(),
            Type = AlertType.SessionPause,
            Severity = pauseDuration.TotalHours > 4 ? AlertSeverity.Error : AlertSeverity.Warning,
            Title = "Long Session Pause",
            Message = $"Session has been paused for {pauseDuration.TotalHours:F1} hours. Please check table status.",
            EntityId = sessionId,
            EntityType = "TableSession",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            IsActive = true
        };

        _dbContext.Set<AlertEntity>().Add(alert);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogWarning("Generated long pause alert for session {SessionId}, paused for {Duration}", 
            sessionId, pauseDuration);
    }

    public async Task GenerateEquipmentMaintenanceAlertAsync(
        Guid equipmentId, 
        string maintenanceType, 
        DateTime dueDate, 
        CancellationToken cancellationToken = default)
    {
        var daysUntilDue = (dueDate - DateTime.UtcNow).TotalDays;
        var severity = daysUntilDue <= 1 ? AlertSeverity.Critical : 
                      daysUntilDue <= 3 ? AlertSeverity.Error : AlertSeverity.Warning;

        var alert = new AlertEntity
        {
            Id = Guid.NewGuid(),
            Type = AlertType.EquipmentMaintenance,
            Severity = severity,
            Title = "Equipment Maintenance Due",
            Message = $"Equipment maintenance ({maintenanceType}) is due on {dueDate:yyyy-MM-dd}",
            EntityId = equipmentId,
            EntityType = "Equipment",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = dueDate.AddDays(7),
            IsActive = true
        };

        _dbContext.Set<AlertEntity>().Add(alert);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Generated equipment maintenance alert for equipment {EquipmentId}, due {DueDate}", 
            equipmentId, dueDate);
    }

    public async Task GenerateCapacityAlertAsync(
        string message, 
        AlertSeverity severity, 
        CancellationToken cancellationToken = default)
    {
        var alert = new AlertEntity
        {
            Id = Guid.NewGuid(),
            Type = AlertType.Capacity,
            Severity = severity,
            Title = "Capacity Alert",
            Message = message,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(4),
            IsActive = true
        };

        _dbContext.Set<AlertEntity>().Add(alert);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Generated capacity alert: {Message}", message);
    }

    public async Task GeneratePerformanceAlertAsync(
        string metric, 
        double currentValue, 
        double threshold, 
        CancellationToken cancellationToken = default)
    {
        var severity = currentValue > threshold * 2 ? AlertSeverity.Critical : AlertSeverity.Warning;

        var alert = new AlertEntity
        {
            Id = Guid.NewGuid(),
            Type = AlertType.Performance,
            Severity = severity,
            Title = "Performance Alert",
            Message = $"Performance metric '{metric}' is {currentValue:F2}, exceeding threshold of {threshold:F2}",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(2),
            IsActive = true
        };

        _dbContext.Set<AlertEntity>().Add(alert);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogWarning("Generated performance alert for metric {Metric}: {CurrentValue} > {Threshold}", 
            metric, currentValue, threshold);
    }

    public async Task<IEnumerable<Alert>> GetActiveAlertsAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Set<AlertEntity>()
            .Where(a => a.IsActive && (!a.ExpiresAt.HasValue || a.ExpiresAt > DateTime.UtcNow))
            .OrderByDescending(a => a.Severity)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToAlert);
    }

    public async Task<IEnumerable<Alert>> GetAlertsBySeverityAsync(AlertSeverity severity, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Set<AlertEntity>()
            .Where(a => a.Severity == severity && a.IsActive)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToAlert);
    }

    public async Task AcknowledgeAlertAsync(Guid alertId, Guid userId, CancellationToken cancellationToken = default)
    {
        var alert = await _dbContext.Set<AlertEntity>()
            .FirstOrDefaultAsync(a => a.Id == alertId, cancellationToken);

        if (alert != null && !alert.AcknowledgedAt.HasValue)
        {
            alert.AcknowledgedAt = DateTime.UtcNow;
            alert.AcknowledgedBy = userId;

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Alert {AlertId} acknowledged by user {UserId}", alertId, userId);
        }
    }

    public async Task ResolveAlertAsync(
        Guid alertId, 
        Guid userId, 
        string resolution, 
        CancellationToken cancellationToken = default)
    {
        var alert = await _dbContext.Set<AlertEntity>()
            .FirstOrDefaultAsync(a => a.Id == alertId, cancellationToken);

        if (alert != null && alert.IsActive)
        {
            alert.ResolvedAt = DateTime.UtcNow;
            alert.ResolvedBy = userId;
            alert.Resolution = resolution;
            alert.IsActive = false;

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Alert {AlertId} resolved by user {UserId}: {Resolution}", 
                alertId, userId, resolution);
        }
    }

    public async Task<int> ClearExpiredAlertsAsync(CancellationToken cancellationToken = default)
    {
        var expiredAlerts = await _dbContext.Set<AlertEntity>()
            .Where(a => a.IsActive && a.ExpiresAt.HasValue && a.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var alert in expiredAlerts)
        {
            alert.IsActive = false;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cleared {Count} expired alerts", expiredAlerts.Count);

        return expiredAlerts.Count;
    }

    private static Alert MapToAlert(AlertEntity entity)
    {
        return new Alert(
            entity.Id,
            entity.Type,
            entity.Severity,
            entity.Title,
            entity.Message,
            entity.EntityId,
            entity.EntityType,
            entity.CreatedAt,
            entity.AcknowledgedAt,
            entity.AcknowledgedBy,
            entity.ResolvedAt,
            entity.ResolvedBy,
            entity.Resolution,
            entity.ExpiresAt,
            entity.IsActive
        );
    }
}

/// <summary>
/// Entity for storing alerts in the database.
/// </summary>
public class AlertEntity
{
    public Guid Id { get; set; }
    public AlertType Type { get; set; }
    public AlertSeverity Severity { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string? EntityType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public Guid? AcknowledgedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public Guid? ResolvedBy { get; set; }
    public string? Resolution { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}
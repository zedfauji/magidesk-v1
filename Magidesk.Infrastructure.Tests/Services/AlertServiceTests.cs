using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Magidesk.Application.Interfaces;
using Magidesk.Infrastructure.Data;
using Magidesk.Infrastructure.Services;

namespace Magidesk.Infrastructure.Tests.Services;

/// <summary>
/// Integration tests for AlertService.
/// </summary>
[Collection("Database Tests")]
public class AlertServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly AlertService _alertService;
    private readonly Mock<ILogger<AlertService>> _mockLogger;

    public AlertServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<AlertService>>();
        _alertService = new AlertService(_context, _mockLogger.Object);
    }

    [Fact]
    public async Task GenerateLongPauseAlertAsync_ShouldCreateAlert()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var pauseDuration = TimeSpan.FromHours(3);

        // Act
        await _alertService.GenerateLongPauseAlertAsync(sessionId, pauseDuration);

        // Assert
        var alerts = await _alertService.GetActiveAlertsAsync();
        var alert = alerts.FirstOrDefault();
        
        Assert.NotNull(alert);
        Assert.Equal(AlertType.SessionPause, alert.Type);
        Assert.Equal(AlertSeverity.Warning, alert.Severity);
        Assert.Equal(sessionId, alert.EntityId);
        Assert.Contains("3.0 hours", alert.Message);
        Assert.True(alert.IsActive);
    }

    [Fact]
    public async Task GenerateEquipmentMaintenanceAlertAsync_ShouldCreateAlertWithCorrectSeverity()
    {
        // Arrange
        var equipmentId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddHours(12); // Less than 1 day - should be Critical

        // Act
        await _alertService.GenerateEquipmentMaintenanceAlertAsync(equipmentId, "Routine Cleaning", dueDate);

        // Assert
        var alerts = await _alertService.GetActiveAlertsAsync();
        var alert = alerts.FirstOrDefault();
        
        Assert.NotNull(alert);
        Assert.Equal(AlertType.EquipmentMaintenance, alert.Type);
        Assert.Equal(AlertSeverity.Critical, alert.Severity);
        Assert.Equal(equipmentId, alert.EntityId);
        Assert.Contains("Routine Cleaning", alert.Message);
    }

    [Fact]
    public async Task GenerateCapacityAlertAsync_ShouldCreateAlert()
    {
        // Arrange
        var message = "All tables are occupied";
        var severity = AlertSeverity.Warning;

        // Act
        await _alertService.GenerateCapacityAlertAsync(message, severity);

        // Assert
        var alerts = await _alertService.GetActiveAlertsAsync();
        var alert = alerts.FirstOrDefault();
        
        Assert.NotNull(alert);
        Assert.Equal(AlertType.Capacity, alert.Type);
        Assert.Equal(severity, alert.Severity);
        Assert.Equal(message, alert.Message);
        Assert.Null(alert.EntityId);
    }

    [Fact]
    public async Task GeneratePerformanceAlertAsync_ShouldCreateAlertWithCorrectSeverity()
    {
        // Arrange
        var metric = "response_time";
        var currentValue = 500.0;
        var threshold = 200.0;

        // Act
        await _alertService.GeneratePerformanceAlertAsync(metric, currentValue, threshold);

        // Assert
        var alerts = await _alertService.GetActiveAlertsAsync();
        var alert = alerts.FirstOrDefault();
        
        Assert.NotNull(alert);
        Assert.Equal(AlertType.Performance, alert.Type);
        Assert.Equal(AlertSeverity.Warning, alert.Severity); // 500 is not > 2 * 200
        Assert.Contains("response_time", alert.Message);
        Assert.Contains("500", alert.Message);
        Assert.Contains("200", alert.Message);
    }

    [Fact]
    public async Task GetAlertsBySeverityAsync_ShouldReturnCorrectAlerts()
    {
        // Arrange
        await _alertService.GenerateCapacityAlertAsync("Warning message", AlertSeverity.Warning);
        await _alertService.GenerateCapacityAlertAsync("Error message", AlertSeverity.Error);
        await _alertService.GenerateCapacityAlertAsync("Critical message", AlertSeverity.Critical);

        // Act
        var warningAlerts = await _alertService.GetAlertsBySeverityAsync(AlertSeverity.Warning);
        var errorAlerts = await _alertService.GetAlertsBySeverityAsync(AlertSeverity.Error);

        // Assert
        Assert.Single(warningAlerts);
        Assert.Single(errorAlerts);
        Assert.Contains("Warning message", warningAlerts.First().Message);
        Assert.Contains("Error message", errorAlerts.First().Message);
    }

    [Fact]
    public async Task AcknowledgeAlertAsync_ShouldUpdateAlert()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        await _alertService.GenerateLongPauseAlertAsync(sessionId, TimeSpan.FromHours(2));
        var alerts = await _alertService.GetActiveAlertsAsync();
        var alert = alerts.First();

        // Act
        await _alertService.AcknowledgeAlertAsync(alert.Id, userId);

        // Assert
        var updatedAlerts = await _alertService.GetActiveAlertsAsync();
        var updatedAlert = updatedAlerts.First(a => a.Id == alert.Id);
        
        Assert.NotNull(updatedAlert.AcknowledgedAt);
        Assert.Equal(userId, updatedAlert.AcknowledgedBy);
    }

    [Fact]
    public async Task ResolveAlertAsync_ShouldDeactivateAlert()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var resolution = "Session resumed by customer";
        
        await _alertService.GenerateLongPauseAlertAsync(sessionId, TimeSpan.FromHours(2));
        var alerts = await _alertService.GetActiveAlertsAsync();
        var alert = alerts.First();

        // Act
        await _alertService.ResolveAlertAsync(alert.Id, userId, resolution);

        // Assert
        var activeAlerts = await _alertService.GetActiveAlertsAsync();
        Assert.Empty(activeAlerts);
        
        // Verify the alert was resolved but not deleted
        var allAlerts = await _context.Set<AlertEntity>().ToListAsync();
        var resolvedAlert = allAlerts.First(a => a.Id == alert.Id);
        
        Assert.False(resolvedAlert.IsActive);
        Assert.NotNull(resolvedAlert.ResolvedAt);
        Assert.Equal(userId, resolvedAlert.ResolvedBy);
        Assert.Equal(resolution, resolvedAlert.Resolution);
    }

    [Fact]
    public async Task ClearExpiredAlertsAsync_ShouldDeactivateExpiredAlerts()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        
        await _alertService.GenerateLongPauseAlertAsync(sessionId, TimeSpan.FromHours(2));
        
        // Manually set expiration to past
        var alertEntity = await _context.Set<AlertEntity>().FirstAsync();
        alertEntity.ExpiresAt = DateTime.UtcNow.AddHours(-1);
        await _context.SaveChangesAsync();

        // Act
        var clearedCount = await _alertService.ClearExpiredAlertsAsync();

        // Assert
        Assert.Equal(1, clearedCount);
        
        var activeAlerts = await _alertService.GetActiveAlertsAsync();
        Assert.Empty(activeAlerts);
    }

    [Fact]
    public async Task GetActiveAlertsAsync_ShouldNotReturnExpiredAlerts()
    {
        // Arrange
        var sessionId1 = Guid.NewGuid();
        var sessionId2 = Guid.NewGuid();
        
        await _alertService.GenerateLongPauseAlertAsync(sessionId1, TimeSpan.FromHours(2));
        await _alertService.GenerateLongPauseAlertAsync(sessionId2, TimeSpan.FromHours(3));
        
        // Manually expire one alert
        var alertEntities = await _context.Set<AlertEntity>().ToListAsync();
        alertEntities.First().ExpiresAt = DateTime.UtcNow.AddHours(-1);
        await _context.SaveChangesAsync();

        // Act
        var activeAlerts = await _alertService.GetActiveAlertsAsync();

        // Assert
        Assert.Single(activeAlerts);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
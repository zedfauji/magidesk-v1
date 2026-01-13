using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.TableSessions;
using Magidesk.Domain.Enumerations;
using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magidesk.Presentation.Tests.ViewModels;

/// <summary>
/// Integration tests for RealTimeSessionMonitoringViewModel.
/// Tests real-time display updates, filtering, and status indicators.
/// </summary>
public class RealTimeSessionMonitoringViewModelTests : IDisposable
{
    private readonly Mock<IQueryHandler<GetActiveSessionsQuery, IEnumerable<ActiveSessionDto>>> _mockGetActiveSessionsHandler;
    private readonly Mock<IQueryHandler<GetSessionAlertsQuery, IEnumerable<SessionAlertDto>>> _mockGetSessionAlertsHandler;
    private readonly Mock<ILogger<RealTimeSessionMonitoringViewModel>> _mockLogger;
    private readonly RealTimeSessionMonitoringViewModel _viewModel;

    public RealTimeSessionMonitoringViewModelTests()
    {
        _mockGetActiveSessionsHandler = new Mock<IQueryHandler<GetActiveSessionsQuery, IEnumerable<ActiveSessionDto>>>();
        _mockGetSessionAlertsHandler = new Mock<IQueryHandler<GetSessionAlertsQuery, IEnumerable<SessionAlertDto>>>();
        _mockLogger = new Mock<ILogger<RealTimeSessionMonitoringViewModel>>();

        _viewModel = new RealTimeSessionMonitoringViewModel(
            _mockGetActiveSessionsHandler.Object,
            _mockGetSessionAlertsHandler.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task RefreshDataAsync_LoadsSessionsAndAlertsCorrectly()
    {
        // Arrange
        var sessions = new List<ActiveSessionDto>
        {
            CreateActiveSession(Guid.NewGuid(), 1, TableSessionStatus.Active, 25.00m, TimeSpan.FromHours(2)),
            CreateActiveSession(Guid.NewGuid(), 2, TableSessionStatus.Paused, 15.00m, TimeSpan.FromHours(1)),
            CreateActiveSession(Guid.NewGuid(), 3, TableSessionStatus.Active, 60.00m, TimeSpan.FromHours(4))
        };

        var alerts = new List<SessionAlertDto>
        {
            CreateSessionAlert(sessions[0].SessionId, SessionAlertType.LongRunning),
            CreateSessionAlert(sessions[2].SessionId, SessionAlertType.LongPause)
        };

        _mockGetActiveSessionsHandler.Setup(h => h.HandleAsync(It.IsAny<GetActiveSessionsQuery>()))
            .ReturnsAsync(sessions);
        _mockGetSessionAlertsHandler.Setup(h => h.HandleAsync(It.IsAny<GetSessionAlertsQuery>()))
            .ReturnsAsync(alerts);

        // Act
        await _viewModel.RefreshDataAsync();

        // Assert
        _viewModel.ActiveSessions.Should().HaveCount(3);
        _viewModel.SessionAlerts.Should().HaveCount(2);
        _viewModel.TotalActiveSessions.Should().Be(3);
        _viewModel.PausedSessions.Should().Be(1);
        _viewModel.LongRunningSessions.Should().Be(1); // Session with 4 hours
        _viewModel.AlertCount.Should().Be(2);
        _viewModel.TotalRevenue.Should().Be(100.00m);
        _viewModel.HasActiveSessions.Should().BeTrue();
        _viewModel.HasAlerts.Should().BeTrue();
    }

    [Fact]
    public async Task RefreshDataAsync_WithNoSessions_UpdatesStatisticsCorrectly()
    {
        // Arrange
        _mockGetActiveSessionsHandler.Setup(h => h.HandleAsync(It.IsAny<GetActiveSessionsQuery>()))
            .ReturnsAsync(new List<ActiveSessionDto>());
        _mockGetSessionAlertsHandler.Setup(h => h.HandleAsync(It.IsAny<GetSessionAlertsQuery>()))
            .ReturnsAsync(new List<SessionAlertDto>());

        // Act
        await _viewModel.RefreshDataAsync();

        // Assert
        _viewModel.TotalActiveSessions.Should().Be(0);
        _viewModel.PausedSessions.Should().Be(0);
        _viewModel.LongRunningSessions.Should().Be(0);
        _viewModel.AlertCount.Should().Be(0);
        _viewModel.TotalRevenue.Should().Be(0m);
        _viewModel.AverageSessionDuration.Should().Be(TimeSpan.Zero);
        _viewModel.HasActiveSessions.Should().BeFalse();
        _viewModel.HasAlerts.Should().BeFalse();
    }

    [Fact]
    public async Task ApplyFilterAndSort_ActiveFilter_ShowsOnlyActiveSessions()
    {
        // Arrange
        var sessions = new List<ActiveSessionDto>
        {
            CreateActiveSession(Guid.NewGuid(), 1, TableSessionStatus.Active, 25.00m, TimeSpan.FromHours(2)),
            CreateActiveSession(Guid.NewGuid(), 2, TableSessionStatus.Paused, 15.00m, TimeSpan.FromHours(1)),
            CreateActiveSession(Guid.NewGuid(), 3, TableSessionStatus.Active, 30.00m, TimeSpan.FromHours(1.5))
        };

        _mockGetActiveSessionsHandler.Setup(h => h.HandleAsync(It.IsAny<GetActiveSessionsQuery>()))
            .ReturnsAsync(sessions);
        _mockGetSessionAlertsHandler.Setup(h => h.HandleAsync(It.IsAny<GetSessionAlertsQuery>()))
            .ReturnsAsync(new List<SessionAlertDto>());

        await _viewModel.RefreshDataAsync();

        // Act
        _viewModel.CurrentFilter = SessionMonitoringFilter.Active;

        // Assert
        _viewModel.FilteredSessions.Should().HaveCount(2);
        _viewModel.FilteredSessions.Should().OnlyContain(s => s.Status == TableSessionStatus.Active);
    }

    [Fact]
    public async Task ApplyFilterAndSort_PausedFilter_ShowsOnlyPausedSessions()
    {
        // Arrange
        var sessions = new List<ActiveSessionDto>
        {
            CreateActiveSession(Guid.NewGuid(), 1, TableSessionStatus.Active, 25.00m, TimeSpan.FromHours(2)),
            CreateActiveSession(Guid.NewGuid(), 2, TableSessionStatus.Paused, 15.00m, TimeSpan.FromHours(1)),
            CreateActiveSession(Guid.NewGuid(), 3, TableSessionStatus.Paused, 30.00m, TimeSpan.FromHours(1.5))
        };

        _mockGetActiveSessionsHandler.Setup(h => h.HandleAsync(It.IsAny<GetActiveSessionsQuery>()))
            .ReturnsAsync(sessions);
        _mockGetSessionAlertsHandler.Setup(h => h.HandleAsync(It.IsAny<GetSessionAlertsQuery>()))
            .ReturnsAsync(new List<SessionAlertDto>());

        await _viewModel.RefreshDataAsync();

        // Act
        _viewModel.CurrentFilter = SessionMonitoringFilter.Paused;

        // Assert
        _viewModel.FilteredSessions.Should().HaveCount(2);
        _viewModel.FilteredSessions.Should().OnlyContain(s => s.Status == TableSessionStatus.Paused);
    }

    [Fact]
    public async Task ApplyFilterAndSort_LongRunningFilter_ShowsOnlyLongRunningSessions()
    {
        // Arrange
        var sessions = new List<ActiveSessionDto>
        {
            CreateActiveSession(Guid.NewGuid(), 1, TableSessionStatus.Active, 25.00m, TimeSpan.FromHours(2)),
            CreateActiveSession(Guid.NewGuid(), 2, TableSessionStatus.Active, 15.00m, TimeSpan.FromHours(4)),
            CreateActiveSession(Guid.NewGuid(), 3, TableSessionStatus.Active, 30.00m, TimeSpan.FromHours(1.5))
        };

        _mockGetActiveSessionsHandler.Setup(h => h.HandleAsync(It.IsAny<GetActiveSessionsQuery>()))
            .ReturnsAsync(sessions);
        _mockGetSessionAlertsHandler.Setup(h => h.HandleAsync(It.IsAny<GetSessionAlertsQuery>()))
            .ReturnsAsync(new List<SessionAlertDto>());

        await _viewModel.RefreshDataAsync();

        // Act
        _viewModel.CurrentFilter = SessionMonitoringFilter.LongRunning;

        // Assert
        _viewModel.FilteredSessions.Should().HaveCount(1);
        _viewModel.FilteredSessions.Should().OnlyContain(s => s.ElapsedTime > TimeSpan.FromHours(3));
    }

    [Fact]
    public async Task ApplyFilterAndSort_HighValueFilter_ShowsOnlyHighValueSessions()
    {
        // Arrange
        var sessions = new List<ActiveSessionDto>
        {
            CreateActiveSession(Guid.NewGuid(), 1, TableSessionStatus.Active, 25.00m, TimeSpan.FromHours(2)),
            CreateActiveSession(Guid.NewGuid(), 2, TableSessionStatus.Active, 75.00m, TimeSpan.FromHours(4)),
            CreateActiveSession(Guid.NewGuid(), 3, TableSessionStatus.Active, 30.00m, TimeSpan.FromHours(1.5))
        };

        _mockGetActiveSessionsHandler.Setup(h => h.HandleAsync(It.IsAny<GetActiveSessionsQuery>()))
            .ReturnsAsync(sessions);
        _mockGetSessionAlertsHandler.Setup(h => h.HandleAsync(It.IsAny<GetSessionAlertsQuery>()))
            .ReturnsAsync(new List<SessionAlertDto>());

        await _viewModel.RefreshDataAsync();

        // Act
        _viewModel.CurrentFilter = SessionMonitoringFilter.HighValue;

        // Assert
        _viewModel.FilteredSessions.Should().HaveCount(1);
        _viewModel.FilteredSessions.Should().OnlyContain(s => s.CurrentCharge > 50m);
    }

    [Fact]
    public async Task ApplyFilterAndSort_SortByChargeDescending_OrdersCorrectly()
    {
        // Arrange
        var sessions = new List<ActiveSessionDto>
        {
            CreateActiveSession(Guid.NewGuid(), 1, TableSessionStatus.Active, 25.00m, TimeSpan.FromHours(2)),
            CreateActiveSession(Guid.NewGuid(), 2, TableSessionStatus.Active, 75.00m, TimeSpan.FromHours(4)),
            CreateActiveSession(Guid.NewGuid(), 3, TableSessionStatus.Active, 30.00m, TimeSpan.FromHours(1.5))
        };

        _mockGetActiveSessionsHandler.Setup(h => h.HandleAsync(It.IsAny<GetActiveSessionsQuery>()))
            .ReturnsAsync(sessions);
        _mockGetSessionAlertsHandler.Setup(h => h.HandleAsync(It.IsAny<GetSessionAlertsQuery>()))
            .ReturnsAsync(new List<SessionAlertDto>());

        await _viewModel.RefreshDataAsync();

        // Act
        _viewModel.SortOrder = SessionSortOrder.ChargeDescending;

        // Assert
        _viewModel.FilteredSessions.Should().HaveCount(3);
        _viewModel.FilteredSessions[0].CurrentCharge.Should().Be(75.00m);
        _viewModel.FilteredSessions[1].CurrentCharge.Should().Be(30.00m);
        _viewModel.FilteredSessions[2].CurrentCharge.Should().Be(25.00m);
    }

    [Fact]
    public async Task ApplyFilterAndSort_SortByTableNumber_OrdersCorrectly()
    {
        // Arrange
        var sessions = new List<ActiveSessionDto>
        {
            CreateActiveSession(Guid.NewGuid(), 5, TableSessionStatus.Active, 25.00m, TimeSpan.FromHours(2)),
            CreateActiveSession(Guid.NewGuid(), 2, TableSessionStatus.Active, 75.00m, TimeSpan.FromHours(4)),
            CreateActiveSession(Guid.NewGuid(), 8, TableSessionStatus.Active, 30.00m, TimeSpan.FromHours(1.5))
        };

        _mockGetActiveSessionsHandler.Setup(h => h.HandleAsync(It.IsAny<GetActiveSessionsQuery>()))
            .ReturnsAsync(sessions);
        _mockGetSessionAlertsHandler.Setup(h => h.HandleAsync(It.IsAny<GetSessionAlertsQuery>()))
            .ReturnsAsync(new List<SessionAlertDto>());

        await _viewModel.RefreshDataAsync();

        // Act
        _viewModel.SortOrder = SessionSortOrder.TableNumber;

        // Assert
        _viewModel.FilteredSessions.Should().HaveCount(3);
        _viewModel.FilteredSessions[0].TableNumber.Should().Be(2);
        _viewModel.FilteredSessions[1].TableNumber.Should().Be(5);
        _viewModel.FilteredSessions[2].TableNumber.Should().Be(8);
    }

    [Fact]
    public void SelectSession_RaisesSessionSelectedEvent()
    {
        // Arrange
        var session = CreateActiveSession(Guid.NewGuid(), 1, TableSessionStatus.Active, 25.00m, TimeSpan.FromHours(2));
        SessionSelectedEventArgs? eventArgs = null;
        _viewModel.SessionSelected += (s, e) => eventArgs = e;

        // Act
        _viewModel.SelectSessionCommand.Execute(session);

        // Assert
        eventArgs.Should().NotBeNull();
        eventArgs!.Session.Should().Be(session);
    }

    [Fact]
    public void ToggleRealTime_ChangesRealTimeEnabledState()
    {
        // Arrange
        var initialState = _viewModel.IsRealTimeEnabled;

        // Act
        _viewModel.ToggleRealTimeCommand.Execute(null);

        // Assert
        _viewModel.IsRealTimeEnabled.Should().Be(!initialState);
    }

    [Theory]
    [InlineData(SessionMonitoringFilter.All, "All Sessions")]
    [InlineData(SessionMonitoringFilter.Active, "Active Only")]
    [InlineData(SessionMonitoringFilter.Paused, "Paused Only")]
    [InlineData(SessionMonitoringFilter.LongRunning, "Long Running (>3h)")]
    [InlineData(SessionMonitoringFilter.HighValue, "High Value (>$50)")]
    public void FilterDisplay_ReturnsCorrectText(SessionMonitoringFilter filter, string expectedDisplay)
    {
        // Arrange
        _viewModel.CurrentFilter = filter;

        // Act & Assert
        _viewModel.FilterDisplay.Should().Be(expectedDisplay);
    }

    [Theory]
    [InlineData(SessionSortOrder.StartTimeDescending, "Newest First")]
    [InlineData(SessionSortOrder.StartTimeAscending, "Oldest First")]
    [InlineData(SessionSortOrder.DurationDescending, "Longest Duration")]
    [InlineData(SessionSortOrder.DurationAscending, "Shortest Duration")]
    [InlineData(SessionSortOrder.ChargeDescending, "Highest Charge")]
    [InlineData(SessionSortOrder.ChargeAscending, "Lowest Charge")]
    [InlineData(SessionSortOrder.TableNumber, "Table Number")]
    public void SortOrderDisplay_ReturnsCorrectText(SessionSortOrder sortOrder, string expectedDisplay)
    {
        // Arrange
        _viewModel.SortOrder = sortOrder;

        // Act & Assert
        _viewModel.SortOrderDisplay.Should().Be(expectedDisplay);
    }

    [Fact]
    public async Task RefreshDataAsync_HandlerThrowsException_ShowsError()
    {
        // Arrange
        var exception = new InvalidOperationException("Database error");
        _mockGetActiveSessionsHandler.Setup(h => h.HandleAsync(It.IsAny<GetActiveSessionsQuery>()))
            .ThrowsAsync(exception);

        // Act
        await _viewModel.RefreshDataAsync();

        // Assert
        _viewModel.HasError.Should().BeTrue();
        _viewModel.ErrorMessage.Should().Contain("Database error");
    }

    [Fact]
    public async Task ClearAlertsAsync_ClearsAlertsAndUpdatesCount()
    {
        // Arrange
        var alerts = new List<SessionAlertDto>
        {
            CreateSessionAlert(Guid.NewGuid(), SessionAlertType.LongRunning),
            CreateSessionAlert(Guid.NewGuid(), SessionAlertType.LongPause)
        };

        _mockGetActiveSessionsHandler.Setup(h => h.HandleAsync(It.IsAny<GetActiveSessionsQuery>()))
            .ReturnsAsync(new List<ActiveSessionDto>());
        _mockGetSessionAlertsHandler.Setup(h => h.HandleAsync(It.IsAny<GetSessionAlertsQuery>()))
            .ReturnsAsync(alerts);

        await _viewModel.RefreshDataAsync();
        _viewModel.SessionAlerts.Should().HaveCount(2);

        // Act
        await _viewModel.ClearAlertsCommand.ExecuteAsync(null);

        // Assert
        _viewModel.SessionAlerts.Should().BeEmpty();
        _viewModel.AlertCount.Should().Be(0);
        _viewModel.HasAlerts.Should().BeFalse();
    }

    private static ActiveSessionDto CreateActiveSession(Guid sessionId, int tableNumber, TableSessionStatus status, decimal charge, TimeSpan elapsedTime)
    {
        var startTime = DateTime.UtcNow - elapsedTime;
        var hourlyRate = charge / (decimal)elapsedTime.TotalHours;

        return new ActiveSessionDto
        {
            SessionId = sessionId,
            TableId = Guid.NewGuid(),
            TableNumber = tableNumber,
            TableName = $"Table {tableNumber}",
            StartTime = startTime,
            Status = status,
            HourlyRate = hourlyRate,
            PausedDuration = TimeSpan.Zero
        };
    }

    private static SessionAlertDto CreateSessionAlert(Guid sessionId, SessionAlertType alertType)
    {
        return new SessionAlertDto
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            TableId = Guid.NewGuid(),
            TableName = "Test Table",
            AlertType = alertType,
            AlertMessage = $"Test alert: {alertType}",
            CreatedAt = DateTime.UtcNow,
            Severity = SessionAlertSeverity.Warning,
            IsAcknowledged = false
        };
    }

    public void Dispose()
    {
        _viewModel?.Dispose();
    }
}
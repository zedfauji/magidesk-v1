using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Services;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Moq;
using Xunit;

namespace Magidesk.Domain.Tests.Services;

/// <summary>
/// Unit tests for SessionControlService covering Property 4: Pause/Resume Time Accuracy.
/// Feature: table-game-management
/// </summary>
public class SessionControlServiceTests
{
    private readonly Mock<ITableSessionRepository> _sessionRepositoryMock;
    private readonly Mock<ITableRepository> _tableRepositoryMock;
    private readonly Mock<ITableTypeRepository> _tableTypeRepositoryMock;
    private readonly Mock<IAuditEventRepository> _auditEventRepositoryMock;
    private readonly Mock<Domain.Services.IPricingService> _pricingServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IUserContextService> _userContextServiceMock;
    private readonly SessionControlService _sessionControlService;

    public SessionControlServiceTests()
    {
        _sessionRepositoryMock = new Mock<ITableSessionRepository>();
        _tableRepositoryMock = new Mock<ITableRepository>();
        _tableTypeRepositoryMock = new Mock<ITableTypeRepository>();
        _auditEventRepositoryMock = new Mock<IAuditEventRepository>();
        _pricingServiceMock = new Mock<Domain.Services.IPricingService>();
        _userServiceMock = new Mock<IUserService>();
        _userContextServiceMock = new Mock<IUserContextService>();
        _userContextServiceMock.Setup(x => x.GetCurrentUserId()).Returns(new Guid("11111111-1111-1111-1111-111111111111"));

        _sessionControlService = new SessionControlService(
            _sessionRepositoryMock.Object,
            _tableRepositoryMock.Object,
            _tableTypeRepositoryMock.Object,
            _auditEventRepositoryMock.Object,
            _pricingServiceMock.Object,
            _userServiceMock.Object,
            _userContextServiceMock.Object);
    }

    /// <summary>
    /// Property 4: Pause/Resume Time Accuracy
    /// Tests that paused time is excluded from billable time calculations.
    /// Validates: Requirements 2.1, 2.2, 2.3
    /// </summary>
    [Fact]
    public async Task PauseResumeSession_ShouldExcludePausedTimeFromBillableTime()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        var tableTypeId = Guid.NewGuid();
        var startTime = DateTime.UtcNow.AddHours(-1); // Session started 1 hour ago
        
        var session = TableSession.Start(tableId, tableTypeId, 10.0m, 2);
        
        // Use reflection to set the start time to simulate a session that has been running
        var startTimeProperty = typeof(TableSession).GetProperty("StartTime");
        startTimeProperty?.SetValue(session, startTime);
        
        var initialBillableTime = session.GetBillableTime();
        
        SetupMocks(sessionId, session);

        // Act - Pause the session
        var pauseResult = await _sessionControlService.PauseSessionAsync(sessionId, "Customer break");
        
        // Simulate some time passing while paused
        await Task.Delay(50); // 50ms pause
        
        // Resume the session
        var resumeResult = await _sessionControlService.ResumeSessionAsync(sessionId);

        // Assert
        pauseResult.IsSuccessful.Should().BeTrue();
        resumeResult.IsSuccessful.Should().BeTrue();
        
        // The session should be active after resume
        session.Status.Should().Be(TableSessionStatus.Active);
        
        // The total paused duration should be greater than zero
        session.TotalPausedDuration.Should().BeGreaterThan(TimeSpan.Zero);
        
        // The billable time should be less than the total elapsed time
        var finalBillableTime = session.GetBillableTime();
        var totalElapsedTime = DateTime.UtcNow - startTime;
        finalBillableTime.Should().BeLessThan(totalElapsedTime);
        
        // Verify audit events were logged
        _auditEventRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AuditEvent>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    /// <summary>
    /// Property 4: Multiple Pause/Resume Cycles
    /// Tests that multiple pause/resume cycles accumulate paused time correctly.
    /// Validates: Requirements 2.1, 2.2, 2.3
    /// </summary>
    [Fact]
    public async Task MultiplePauseResumeCycles_ShouldAccumulatePausedTimeCorrectly()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        var tableTypeId = Guid.NewGuid();
        
        var session = TableSession.Start(tableId, tableTypeId, 10.0m, 2);
        var initialTotalPausedDuration = session.TotalPausedDuration;
        
        SetupMocks(sessionId, session);

        // Act - Perform multiple pause/resume cycles
        for (int i = 0; i < 3; i++)
        {
            var pauseResult = await _sessionControlService.PauseSessionAsync(sessionId, $"Break {i + 1}");
            pauseResult.IsSuccessful.Should().BeTrue();
            
            await Task.Delay(10); // Small delay to simulate pause time
            
            var resumeResult = await _sessionControlService.ResumeSessionAsync(sessionId);
            resumeResult.IsSuccessful.Should().BeTrue();
        }

        // Assert
        session.Status.Should().Be(TableSessionStatus.Active);
        session.TotalPausedDuration.Should().BeGreaterThan(initialTotalPausedDuration);
        
        // Each pause/resume cycle should have added to the total paused duration
        session.TotalPausedDuration.Should().BeGreaterThan(TimeSpan.FromMilliseconds(20)); // At least 3 cycles * ~10ms each
        
        // Verify audit events were logged for all operations
        _auditEventRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AuditEvent>(), It.IsAny<CancellationToken>()), Times.Exactly(6)); // 3 pauses + 3 resumes
    }

    /// <summary>
    /// Property 4: Pause Invalid States
    /// Tests that pause operations fail appropriately for non-active sessions.
    /// Validates: Requirements 2.1, 2.2
    /// </summary>
    [Fact]
    public async Task PauseEndedSession_ShouldReturnInvalidStateError()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        var tableTypeId = Guid.NewGuid();
        
        var session = TableSession.Start(tableId, tableTypeId, 10.0m, 2);
        session.End(new Money(25.0m)); // End the session
        
        _sessionRepositoryMock.Setup(x => x.GetByIdAsync(sessionId))
            .ReturnsAsync(session);

        // Act
        var result = await _sessionControlService.PauseSessionAsync(sessionId, "Test pause");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessage.Should().Contain("must be active to pause");
    }

    /// <summary>
    /// Property 4: Resume Invalid States
    /// Tests that resume operations fail appropriately for non-paused sessions.
    /// Validates: Requirements 2.2, 2.3
    /// </summary>
    [Fact]
    public async Task ResumeActiveSession_ShouldReturnInvalidStateError()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        var tableTypeId = Guid.NewGuid();
        
        var session = TableSession.Start(tableId, tableTypeId, 10.0m, 2);
        // Session is active, not paused
        
        _sessionRepositoryMock.Setup(x => x.GetByIdAsync(sessionId))
            .ReturnsAsync(session);

        // Act
        var result = await _sessionControlService.ResumeSessionAsync(sessionId);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Can only resume a paused session");
    }

    /// <summary>
    /// Tests guest count updates with proper validation.
    /// Validates: Requirements 4.1, 4.2, 4.3
    /// </summary>
    [Fact]
    public async Task UpdateGuestCount_WithValidInput_ShouldSucceed()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        var tableTypeId = Guid.NewGuid();
        var staffId = Guid.NewGuid();
        
        var session = TableSession.Start(tableId, tableTypeId, 10.0m, 2);
        
        SetupMocks(sessionId, session);

        // Act
        var result = await _sessionControlService.UpdateGuestCountAsync(sessionId, 4, staffId);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        session.GuestCount.Should().Be(4);
        
        // Verify audit event was logged
        _auditEventRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AuditEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests guest count validation boundaries.
    /// Validates: Requirements 4.1, 4.2
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(21)]
    [InlineData(100)]
    public async Task UpdateGuestCount_WithInvalidCount_ShouldReturnValidationError(int invalidGuestCount)
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var staffId = Guid.NewGuid();

        // Act
        var result = await _sessionControlService.UpdateGuestCountAsync(sessionId, invalidGuestCount, staffId);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Guest count must be between 1 and 20");
    }

    /// <summary>
    /// Tests session transfer functionality.
    /// Validates: Requirements 11.1, 11.2, 11.4
    /// </summary>
    [Fact]
    public async Task TransferSession_WithValidInput_ShouldSucceed()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var sourceTableId = Guid.NewGuid();
        var targetTableId = Guid.NewGuid();
        var tableTypeId = Guid.NewGuid();
        
        var session = TableSession.Start(sourceTableId, tableTypeId, 10.0m, 2);
        
        // Create target table with explicit parameters to avoid expression tree issues
        // Create target table with factory method to ensure validity, then set ID via reflection
        var targetTable = Table.Create(1, 4);
        typeof(Table).GetProperty("Id")?.SetValue(targetTable, targetTableId);
        
        _sessionRepositoryMock.Setup(x => x.GetByIdAsync(sessionId))
            .ReturnsAsync(session);
        _tableRepositoryMock.Setup(x => x.GetByIdAsync(targetTableId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetTable);
        _sessionRepositoryMock.Setup(x => x.GetActiveSessionByTableIdAsync(targetTableId))
            .ReturnsAsync((TableSession?)null); // No existing session
        _sessionRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TableSession>()))
            .Returns(Task.CompletedTask);
        _sessionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<TableSession>()))
            .ReturnsAsync((TableSession session) => session);
        _auditEventRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AuditEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _tableTypeRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(TableType.Create("Test Table", 10.0m, ""));
        _pricingServiceMock.Setup(x => x.CalculateTimeCharge(It.IsAny<TimeSpan>(), It.IsAny<TableType>()))
            .Returns(new Money(50.0m));
        _userServiceMock.Setup(x => x.CurrentUser)
            .Returns(new UserDto { Id = Guid.NewGuid() });

        // Act
        var result = await _sessionControlService.TransferSessionAsync(sessionId, targetTableId, "Customer request");

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().NotBeNull();
        
        // Verify the original session was ended
        session.Status.Should().Be(TableSessionStatus.Ended);
        
        // Verify audit events were logged
        _auditEventRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AuditEvent>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    /// <summary>
    /// Tests session alerts generation for long-paused sessions.
    /// Validates: Requirements 2.5, 12.2
    /// </summary>
    [Fact]
    public async Task GetSessionAlerts_WithLongPausedSession_ShouldGenerateAlert()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        var tableTypeId = Guid.NewGuid();
        
        var session = TableSession.Start(tableId, tableTypeId, 10.0m, 2);
        session.Pause();
        
        // Use reflection to set pause time to 3 hours ago
        var pausedAtProperty = typeof(TableSession).GetProperty("PausedAt");
        pausedAtProperty?.SetValue(session, DateTime.UtcNow.AddHours(-3));
        
        _sessionRepositoryMock.Setup(x => x.GetActiveSessionsAsync())
            .ReturnsAsync(new List<TableSession>());
        _sessionRepositoryMock.Setup(x => x.GetSessionsByStatusAsync(TableSessionStatus.Paused))
            .ReturnsAsync(new List<TableSession> { session });

        // Act
        var alerts = await _sessionControlService.GetSessionAlertsAsync();

        // Assert
        alerts.Should().NotBeEmpty();
        alerts.Should().Contain(a => a.AlertType == Magidesk.Domain.ValueObjects.SessionAlertType.LongPause);
        alerts.Should().Contain(a => a.Message.Contains("3.0 hours"));
    }

    private void SetupMocks(Guid sessionId, TableSession session)
    {
        _sessionRepositoryMock.Setup(x => x.GetByIdAsync(sessionId))
            .ReturnsAsync(session);
        _sessionRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TableSession>()))
            .Returns(Task.CompletedTask);
        _auditEventRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AuditEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _tableTypeRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(TableType.Create("Test Table", 10.0m, ""));
        _pricingServiceMock.Setup(x => x.CalculateTimeCharge(It.IsAny<TimeSpan>(), It.IsAny<TableType>()))
            .Returns(new Money(50.0m));
        _userServiceMock.Setup(x => x.CurrentUser)
            .Returns(new UserDto { Id = Guid.NewGuid() });
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Handlers;

public class EnhancedSessionCommandHandlerTests
{
    private readonly Mock<ISessionControlService> _sessionControlServiceMock;
    private readonly EnhancedPauseSessionCommandHandler _pauseHandler;
    private readonly EnhancedResumeSessionCommandHandler _resumeHandler;

    public EnhancedSessionCommandHandlerTests()
    {
        _sessionControlServiceMock = new Mock<ISessionControlService>();
        _pauseHandler = new EnhancedPauseSessionCommandHandler(_sessionControlServiceMock.Object);
        _resumeHandler = new EnhancedResumeSessionCommandHandler(_sessionControlServiceMock.Object);
    }

    [Fact]
    public async Task PauseHandler_ShouldPauseSession_WhenValidCommand()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var reason = "Customer break";
        var staffId = Guid.NewGuid();
        var pausedAt = DateTime.UtcNow;
        
        var command = new EnhancedPauseSessionCommand(sessionId, reason, staffId);
        
        var sessionControlData = new SessionControlData(
            SessionId: sessionId,
            Status: TableSessionStatus.Paused,
            PausedAt: pausedAt,
            TotalPausedDuration: TimeSpan.FromMinutes(10),
            CurrentCharge: new Money(25.50m)
        );
        
        var sessionControlResult = SessionControlResult.Success(sessionControlData);
        
        _sessionControlServiceMock
            .Setup(s => s.PauseSessionAsync(sessionId, reason))
            .ReturnsAsync(sessionControlResult);

        // Act
        var result = await _pauseHandler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.SessionId.Should().Be(sessionId);
        result.PausedAt.Should().BeCloseTo(pausedAt, TimeSpan.FromSeconds(1));
        result.TotalPausedDuration.Should().Be(TimeSpan.FromMinutes(10));
        result.CurrentCharge.Should().Be(25.50m);
        result.Status.Should().Be("Paused");
        
        _sessionControlServiceMock.Verify(s => s.PauseSessionAsync(sessionId, reason), Times.Once);
    }

    [Fact]
    public async Task PauseHandler_ShouldThrowArgumentException_WhenSessionIdIsEmpty()
    {
        // Arrange
        var command = new EnhancedPauseSessionCommand(Guid.Empty, "reason", Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _pauseHandler.HandleAsync(command));
    }

    [Fact]
    public async Task PauseHandler_ShouldThrowArgumentException_WhenReasonIsEmpty()
    {
        // Arrange
        var command = new EnhancedPauseSessionCommand(Guid.NewGuid(), "", Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _pauseHandler.HandleAsync(command));
    }

    [Fact]
    public async Task PauseHandler_ShouldThrowInvalidOperationException_WhenServiceFails()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var command = new EnhancedPauseSessionCommand(sessionId, "reason", Guid.NewGuid());
        
        var failureResult = SessionControlResult.InvalidState("Session is not active");
        
        _sessionControlServiceMock
            .Setup(s => s.PauseSessionAsync(sessionId, "reason"))
            .ReturnsAsync(failureResult);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _pauseHandler.HandleAsync(command));
    }

    [Fact]
    public async Task ResumeHandler_ShouldResumeSession_WhenValidCommand()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var staffId = Guid.NewGuid();
        
        var command = new EnhancedResumeSessionCommand(sessionId, staffId);
        
        var sessionControlData = new SessionControlData(
            SessionId: sessionId,
            Status: TableSessionStatus.Active,
            PausedAt: null,
            TotalPausedDuration: TimeSpan.FromMinutes(15),
            CurrentCharge: new Money(30.75m)
        );
        
        var sessionControlResult = SessionControlResult.Success(sessionControlData);
        
        _sessionControlServiceMock
            .Setup(s => s.ResumeSessionAsync(sessionId))
            .ReturnsAsync(sessionControlResult);

        // Act
        var result = await _resumeHandler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.SessionId.Should().Be(sessionId);
        result.ResumedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.TotalPausedDuration.Should().Be(TimeSpan.FromMinutes(15));
        result.CurrentCharge.Should().Be(30.75m);
        result.Status.Should().Be("Active");
        
        _sessionControlServiceMock.Verify(s => s.ResumeSessionAsync(sessionId), Times.Once);
    }

    [Fact]
    public async Task ResumeHandler_ShouldThrowArgumentException_WhenSessionIdIsEmpty()
    {
        // Arrange
        var command = new EnhancedResumeSessionCommand(Guid.Empty, Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _resumeHandler.HandleAsync(command));
    }

    [Fact]
    public async Task ResumeHandler_ShouldThrowInvalidOperationException_WhenServiceFails()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var command = new EnhancedResumeSessionCommand(sessionId, Guid.NewGuid());
        
        var failureResult = SessionControlResult.InvalidState("Session is not paused");
        
        _sessionControlServiceMock
            .Setup(s => s.ResumeSessionAsync(sessionId))
            .ReturnsAsync(failureResult);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _resumeHandler.HandleAsync(command));
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Handlers;

public class TransferSessionCommandHandlerTests
{
    private readonly Mock<ISessionControlService> _sessionControlServiceMock;
    private readonly Mock<ITableSessionRepository> _sessionRepositoryMock;
    private readonly TransferSessionCommandHandler _handler;

    public TransferSessionCommandHandlerTests()
    {
        _sessionControlServiceMock = new Mock<ISessionControlService>();
        _sessionRepositoryMock = new Mock<ITableSessionRepository>();
        _handler = new TransferSessionCommandHandler(_sessionControlServiceMock.Object, _sessionRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldTransferSession_WhenValidCommand()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var originalTableId = Guid.NewGuid();
        var targetTableId = Guid.NewGuid();
        var staffId = Guid.NewGuid();
        var reason = "Customer requested different table";
        var newSessionId = Guid.NewGuid();
        
        var command = new TransferSessionCommand(sessionId, targetTableId, reason, staffId);
        
        var originalSession = TableSession.Start(originalTableId, Guid.NewGuid(), 20m, 3);
        var sessionType = typeof(TableSession);
        var sessionProp = sessionType.GetProperty("Id");
        sessionProp?.SetValue(originalSession, sessionId);
        
        // Set total charge using reflection
        var totalChargeProp = sessionType.GetProperty("TotalCharge");
        totalChargeProp?.SetValue(originalSession, new Money(60.00m));
        
        var sessionControlData = new SessionControlData(
            SessionId: newSessionId,
            Status: TableSessionStatus.Active,
            PausedAt: null,
            TotalPausedDuration: TimeSpan.Zero,
            CurrentCharge: new Money(60.00m)
        );
        
        var sessionControlResult = SessionControlResult.Success(sessionControlData);
        
        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(originalSession);
            
        _sessionControlServiceMock
            .Setup(s => s.TransferSessionAsync(sessionId, targetTableId, reason))
            .ReturnsAsync(sessionControlResult);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.OriginalSessionId.Should().Be(sessionId);
        result.NewSessionId.Should().Be(newSessionId);
        result.OriginalTableId.Should().Be(originalTableId);
        result.NewTableId.Should().Be(targetTableId);
        result.PreservedCharge.Should().Be(60.00m);
        result.PreservedDuration.Should().BeGreaterThan(TimeSpan.Zero);
        result.TransferredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        
        _sessionControlServiceMock.Verify(s => s.TransferSessionAsync(sessionId, targetTableId, reason), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowArgumentException_WhenSessionIdIsEmpty()
    {
        // Arrange
        var command = new TransferSessionCommand(Guid.Empty, Guid.NewGuid(), "reason", Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowArgumentException_WhenTargetTableIdIsEmpty()
    {
        // Arrange
        var command = new TransferSessionCommand(Guid.NewGuid(), Guid.Empty, "reason", Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowArgumentException_WhenReasonIsEmpty()
    {
        // Arrange
        var command = new TransferSessionCommand(Guid.NewGuid(), Guid.NewGuid(), "", Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowArgumentException_WhenStaffIdIsEmpty()
    {
        // Arrange
        var command = new TransferSessionCommand(Guid.NewGuid(), Guid.NewGuid(), "reason", Guid.Empty);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInvalidOperationException_WhenSessionNotFound()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var command = new TransferSessionCommand(sessionId, Guid.NewGuid(), "reason", Guid.NewGuid());
        
        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync((TableSession?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInvalidOperationException_WhenTransferFails()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var command = new TransferSessionCommand(sessionId, Guid.NewGuid(), "reason", Guid.NewGuid());
        
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 20m, 3);
        var sessionType = typeof(TableSession);
        var sessionProp = sessionType.GetProperty("Id");
        sessionProp?.SetValue(session, sessionId);
        
        var failureResult = SessionControlResult.InvalidState("Target table is occupied");
        
        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);
            
        _sessionControlServiceMock
            .Setup(s => s.TransferSessionAsync(sessionId, It.IsAny<Guid>(), "reason"))
            .ReturnsAsync(failureResult);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
    }
}
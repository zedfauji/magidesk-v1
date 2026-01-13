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

public class UpdateGuestCountCommandHandlerTests
{
    private readonly Mock<ISessionControlService> _sessionControlServiceMock;
    private readonly Mock<ITableSessionRepository> _sessionRepositoryMock;
    private readonly UpdateGuestCountCommandHandler _handler;

    public UpdateGuestCountCommandHandlerTests()
    {
        _sessionControlServiceMock = new Mock<ISessionControlService>();
        _sessionRepositoryMock = new Mock<ITableSessionRepository>();
        _handler = new UpdateGuestCountCommandHandler(_sessionControlServiceMock.Object, _sessionRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateGuestCount_WhenValidCommand()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var staffId = Guid.NewGuid();
        var newGuestCount = 4;
        var previousGuestCount = 2;
        
        var command = new UpdateGuestCountCommand(sessionId, newGuestCount, staffId, "More players joined");
        
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15m, previousGuestCount);
        var sessionType = typeof(TableSession);
        var sessionProp = sessionType.GetProperty("Id");
        sessionProp?.SetValue(session, sessionId);
        
        var sessionControlData = new SessionControlData(
            SessionId: sessionId,
            Status: TableSessionStatus.Active,
            PausedAt: null,
            TotalPausedDuration: TimeSpan.Zero,
            CurrentCharge: new Money(45.00m)
        );
        
        var sessionControlResult = SessionControlResult.Success(sessionControlData);
        
        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);
            
        _sessionControlServiceMock
            .Setup(s => s.UpdateGuestCountAsync(sessionId, newGuestCount, staffId))
            .ReturnsAsync(sessionControlResult);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.SessionId.Should().Be(sessionId);
        result.PreviousGuestCount.Should().Be(previousGuestCount);
        result.NewGuestCount.Should().Be(newGuestCount);
        result.CurrentCharge.Should().Be(45.00m);
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        
        _sessionControlServiceMock.Verify(s => s.UpdateGuestCountAsync(sessionId, newGuestCount, staffId), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(21)]
    [InlineData(25)]
    public async Task HandleAsync_ShouldThrowArgumentException_WhenGuestCountIsInvalid(int invalidGuestCount)
    {
        // Arrange
        var command = new UpdateGuestCountCommand(Guid.NewGuid(), invalidGuestCount, Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowArgumentException_WhenSessionIdIsEmpty()
    {
        // Arrange
        var command = new UpdateGuestCountCommand(Guid.Empty, 4, Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowArgumentException_WhenStaffIdIsEmpty()
    {
        // Arrange
        var command = new UpdateGuestCountCommand(Guid.NewGuid(), 4, Guid.Empty);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInvalidOperationException_WhenSessionNotFound()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var command = new UpdateGuestCountCommand(sessionId, 4, Guid.NewGuid());
        
        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync((TableSession?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInvalidOperationException_WhenServiceFails()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var command = new UpdateGuestCountCommand(sessionId, 4, Guid.NewGuid());
        
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 15m, 2);
        var sessionType = typeof(TableSession);
        var sessionProp = sessionType.GetProperty("Id");
        sessionProp?.SetValue(session, sessionId);
        
        var failureResult = SessionControlResult.ValidationError("Guest count validation failed");
        
        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);
            
        _sessionControlServiceMock
            .Setup(s => s.UpdateGuestCountAsync(sessionId, 4, It.IsAny<Guid>()))
            .ReturnsAsync(failureResult);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
    }
}
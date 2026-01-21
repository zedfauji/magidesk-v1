using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.Commands.ManagerOverrides;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Handlers;

public class ManagerOverrideCommandHandlerTests
{
    private readonly Mock<IManagerOverrideService> _managerOverrideServiceMock;
    private readonly Mock<ITableSessionRepository> _sessionRepositoryMock;
    private readonly Mock<ITableTypeRepository> _tableTypeRepositoryMock;
    private readonly Mock<Domain.Services.IPricingService> _pricingServiceMock;
    private readonly ApplyTimeAdjustmentCommandHandler _timeAdjustmentHandler;
    private readonly ApplyPricingOverrideCommandHandler _pricingOverrideHandler;
    private readonly ForceEndSessionCommandHandler _forceEndHandler;

    public ManagerOverrideCommandHandlerTests()
    {
        _managerOverrideServiceMock = new Mock<IManagerOverrideService>();
        _sessionRepositoryMock = new Mock<ITableSessionRepository>();
        _tableTypeRepositoryMock = new Mock<ITableTypeRepository>();
        _pricingServiceMock = new Mock<Domain.Services.IPricingService>();
        
        _timeAdjustmentHandler = new ApplyTimeAdjustmentCommandHandler(
            _managerOverrideServiceMock.Object,
            _sessionRepositoryMock.Object,
            _tableTypeRepositoryMock.Object,
            _pricingServiceMock.Object);
            
        _pricingOverrideHandler = new ApplyPricingOverrideCommandHandler(
            _managerOverrideServiceMock.Object,
            _sessionRepositoryMock.Object,
            _tableTypeRepositoryMock.Object,
            _pricingServiceMock.Object);
            
        _forceEndHandler = new ForceEndSessionCommandHandler(
            _managerOverrideServiceMock.Object,
            _sessionRepositoryMock.Object);
    }

    [Fact]
    public async Task ApplyTimeAdjustment_ShouldAdjustTime_WhenValidCommand()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var tableTypeId = Guid.NewGuid();
        var adjustment = TimeSpan.FromMinutes(30);
        var managerPin = "1234";
        var reason = "Customer complaint compensation";
        
        var command = new ApplyTimeAdjustmentCommand(sessionId, adjustment, reason, managerPin, managerId);
        
        var session = TableSession.Start(Guid.NewGuid(), tableTypeId, 20m, 2);
        var sessionType = typeof(TableSession);
        var sessionProp = sessionType.GetProperty("Id");
        sessionProp?.SetValue(session, sessionId);
        
        var tableType = TableType.Create("Premium", 20m);
        var originalCharge = new Money(40m);
        var newCharge = new Money(50m);
        
        var authResult = OverrideResult.Success();
        var overrideResult = OverrideResult.Success();
        
        _managerOverrideServiceMock
            .Setup(s => s.ValidateManagerAuthorizationAsync(managerPin, managerId))
            .ReturnsAsync(authResult);
            
        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);
            
        _tableTypeRepositoryMock
            .Setup(r => r.GetByIdAsync(tableTypeId))
            .ReturnsAsync(tableType);
            
        _pricingServiceMock
            .Setup(s => s.CalculateTimeCharge(It.IsAny<TimeSpan>(), tableType))
            .Returns(originalCharge)
            .Callback<TimeSpan, TableType>((duration, tt) =>
            {
                // Return different charge for adjusted time
                if (duration > TimeSpan.FromHours(1))
                    _pricingServiceMock.Setup(s => s.CalculateTimeCharge(It.IsAny<TimeSpan>(), tt)).Returns(newCharge);
            });
            
        _managerOverrideServiceMock
            .Setup(s => s.ApplyTimeAdjustmentAsync(sessionId, adjustment, reason, managerId))
            .ReturnsAsync(overrideResult);

        // Act
        var result = await _timeAdjustmentHandler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.SessionId.Should().Be(sessionId);
        result.AdjustmentApplied.Should().Be(adjustment);
        result.ManagerId.Should().Be(managerId);
        result.AppliedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        
        _managerOverrideServiceMock.Verify(s => s.ValidateManagerAuthorizationAsync(managerPin, managerId), Times.Once);
        _managerOverrideServiceMock.Verify(s => s.ApplyTimeAdjustmentAsync(sessionId, adjustment, reason, managerId), Times.Once);
    }

    [Fact]
    public async Task ApplyTimeAdjustment_ShouldThrowUnauthorizedException_WhenAuthorizationFails()
    {
        // Arrange
        var command = new ApplyTimeAdjustmentCommand(Guid.NewGuid(), TimeSpan.FromMinutes(30), "reason", "1234", Guid.NewGuid());
        
        var authResult = OverrideResult.Unauthorized();
        
        _managerOverrideServiceMock
            .Setup(s => s.ValidateManagerAuthorizationAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(authResult);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _timeAdjustmentHandler.HandleAsync(command));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task ApplyTimeAdjustment_ShouldThrowArgumentException_WhenReasonIsInvalid(string invalidReason)
    {
        // Arrange
        var command = new ApplyTimeAdjustmentCommand(Guid.NewGuid(), TimeSpan.FromMinutes(30), invalidReason, "1234", Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _timeAdjustmentHandler.HandleAsync(command));
    }

    [Fact]
    public async Task ApplyPricingOverride_ShouldOverridePrice_WhenValidCommand()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var tableTypeId = Guid.NewGuid();
        var overrideAmount = 25.00m;
        var managerPin = "1234";
        var reason = "Manager discount";
        
        var command = new ApplyPricingOverrideCommand(sessionId, overrideAmount, reason, managerPin, managerId);
        
        var session = TableSession.Start(Guid.NewGuid(), tableTypeId, 20m, 2);
        var sessionType = typeof(TableSession);
        var sessionProp = sessionType.GetProperty("Id");
        sessionProp?.SetValue(session, sessionId);
        
        var tableType = TableType.Create("Premium", 20m);
        var originalCharge = new Money(40m);
        
        var authResult = OverrideResult.Success();
        var overrideResult = OverrideResult.Success();
        
        _managerOverrideServiceMock
            .Setup(s => s.ValidateManagerAuthorizationAsync(managerPin, managerId))
            .ReturnsAsync(authResult);
            
        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);
            
        _tableTypeRepositoryMock
            .Setup(r => r.GetByIdAsync(tableTypeId))
            .ReturnsAsync(tableType);
            
        _pricingServiceMock
            .Setup(s => s.CalculateTimeCharge(It.IsAny<TimeSpan>(), tableType))
            .Returns(originalCharge);
            
        _managerOverrideServiceMock
            .Setup(s => s.ApplyPricingOverrideAsync(sessionId, It.IsAny<Money>(), reason, managerId))
            .ReturnsAsync(overrideResult);

        // Act
        var result = await _pricingOverrideHandler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.SessionId.Should().Be(sessionId);
        result.OriginalCharge.Should().Be(40m);
        result.NewCharge.Should().Be(overrideAmount);
        result.OverrideAmount.Should().Be(overrideAmount);
        result.Reason.Should().Be(reason);
        result.ManagerId.Should().Be(managerId);
        
        _managerOverrideServiceMock.Verify(s => s.ApplyPricingOverrideAsync(sessionId, It.Is<Money>(m => m.Amount == overrideAmount), reason, managerId), Times.Once);
    }

    [Fact]
    public async Task ApplyPricingOverride_ShouldThrowArgumentException_WhenOverrideAmountIsNegative()
    {
        // Arrange
        var command = new ApplyPricingOverrideCommand(Guid.NewGuid(), -10m, "reason", "1234", Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _pricingOverrideHandler.HandleAsync(command));
    }

    [Fact]
    public async Task ForceEndSession_ShouldEndSession_WhenValidCommand()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var managerPin = "1234";
        var reason = "Emergency situation";
        
        var command = new ForceEndSessionCommand(sessionId, reason, managerPin, managerId);
        
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 20m, 2);
        var sessionType = typeof(TableSession);
        var sessionProp = sessionType.GetProperty("Id");
        sessionProp?.SetValue(session, sessionId);
        
        var authResult = OverrideResult.Success();
        var overrideResult = OverrideResult.Success();
        
        _managerOverrideServiceMock
            .Setup(s => s.ValidateManagerAuthorizationAsync(managerPin, managerId))
            .ReturnsAsync(authResult);
            
        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);
            
        _managerOverrideServiceMock
            .Setup(s => s.ForceEndSessionAsync(sessionId, reason, managerId))
            .ReturnsAsync(overrideResult);

        // Act
        var result = await _forceEndHandler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.SessionId.Should().Be(sessionId);
        result.Reason.Should().Be(reason);
        result.ManagerId.Should().Be(managerId);
        result.EndedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        
        _managerOverrideServiceMock.Verify(s => s.ForceEndSessionAsync(sessionId, reason, managerId), Times.Once);
    }
}
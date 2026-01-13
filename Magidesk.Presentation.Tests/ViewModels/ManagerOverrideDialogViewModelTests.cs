using System;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.ViewModels.Dialogs;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magidesk.Presentation.Tests.ViewModels;

/// <summary>
/// Integration tests for ManagerOverrideDialogViewModel.
/// Tests manager authorization workflow and override operations.
/// </summary>
public class ManagerOverrideDialogViewModelTests
{
    private readonly Mock<IManagerOverrideService> _mockManagerOverrideService;
    private readonly Mock<ILogger<ManagerOverrideDialogViewModel>> _mockLogger;
    private readonly ManagerOverrideDialogViewModel _viewModel;

    public ManagerOverrideDialogViewModelTests()
    {
        _mockManagerOverrideService = new Mock<IManagerOverrideService>();
        _mockLogger = new Mock<ILogger<ManagerOverrideDialogViewModel>>();

        _viewModel = new ManagerOverrideDialogViewModel(
            _mockManagerOverrideService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public void Initialize_SetsPropertiesCorrectly()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var tableName = "Table 5";
        var overrideType = ManagerOverrideType.TimeAdjustment;
        var currentSessionTime = TimeSpan.FromHours(2);
        var currentCharge = 30.00m;

        // Act
        _viewModel.Initialize(sessionId, tableName, overrideType, currentSessionTime, currentCharge);

        // Assert
        _viewModel.SessionId.Should().Be(sessionId);
        _viewModel.TableName.Should().Be(tableName);
        _viewModel.OverrideType.Should().Be(overrideType);
        _viewModel.CurrentSessionTime.Should().Be(currentSessionTime);
        _viewModel.CurrentCharge.Should().Be(currentCharge);
        _viewModel.PricingOverrideAmount.Should().Be(currentCharge);
        _viewModel.IsAuthorized.Should().BeFalse();
        _viewModel.HasError.Should().BeFalse();
    }

    [Theory]
    [InlineData(ManagerOverrideType.TimeAdjustment, "Time Adjustment")]
    [InlineData(ManagerOverrideType.PricingOverride, "Pricing Override")]
    [InlineData(ManagerOverrideType.ForceEnd, "Force End Session")]
    public void OverrideTypeDisplay_ReturnsCorrectText(ManagerOverrideType overrideType, string expectedDisplay)
    {
        // Arrange
        _viewModel.Initialize(Guid.NewGuid(), "Table 1", overrideType, TimeSpan.FromHours(1), 15.00m);

        // Act & Assert
        _viewModel.OverrideTypeDisplay.Should().Be(expectedDisplay);
    }

    [Fact]
    public void CurrentReasonCodes_ReturnsCorrectCollectionForOverrideType()
    {
        // Test Time Adjustment
        _viewModel.Initialize(Guid.NewGuid(), "Table 1", ManagerOverrideType.TimeAdjustment, TimeSpan.FromHours(1), 15.00m);
        _viewModel.CurrentReasonCodes.Should().BeSameAs(_viewModel.TimeAdjustmentReasons);

        // Test Pricing Override
        _viewModel.Initialize(Guid.NewGuid(), "Table 1", ManagerOverrideType.PricingOverride, TimeSpan.FromHours(1), 15.00m);
        _viewModel.CurrentReasonCodes.Should().BeSameAs(_viewModel.PricingOverrideReasons);

        // Test Force End
        _viewModel.Initialize(Guid.NewGuid(), "Table 1", ManagerOverrideType.ForceEnd, TimeSpan.FromHours(1), 15.00m);
        _viewModel.CurrentReasonCodes.Should().BeSameAs(_viewModel.ForceEndReasons);
    }

    [Fact]
    public async Task AuthorizeAsync_WithValidPin_SetsAuthorizedState()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var managerPin = "1234";
        var successResult = OverrideResult.Success();
        
        _mockManagerOverrideService.Setup(s => s.ValidateManagerAuthorizationAsync(managerPin, It.IsAny<Guid>()))
            .ReturnsAsync(successResult);

        _viewModel.Initialize(sessionId, "Table 1", ManagerOverrideType.TimeAdjustment, TimeSpan.FromHours(1), 15.00m);
        _viewModel.ManagerPin = managerPin;

        // Act
        await _viewModel.AuthorizeCommand.ExecuteAsync(null);

        // Assert
        _mockManagerOverrideService.Verify(s => s.ValidateManagerAuthorizationAsync(managerPin, It.IsAny<Guid>()), Times.Once);
        _viewModel.IsAuthorized.Should().BeTrue();
        _viewModel.HasError.Should().BeFalse();
    }

    [Fact]
    public async Task AuthorizeAsync_WithInvalidPin_ShowsErrorAndClearsPin()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var managerPin = "wrong";
        var failureResult = OverrideResult.Unauthorized();
        
        _mockManagerOverrideService.Setup(s => s.ValidateManagerAuthorizationAsync(managerPin, It.IsAny<Guid>()))
            .ReturnsAsync(failureResult);

        _viewModel.Initialize(sessionId, "Table 1", ManagerOverrideType.TimeAdjustment, TimeSpan.FromHours(1), 15.00m);
        _viewModel.ManagerPin = managerPin;

        // Act
        await _viewModel.AuthorizeCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsAuthorized.Should().BeFalse();
        _viewModel.HasError.Should().BeTrue();
        _viewModel.ManagerPin.Should().BeEmpty(); // PIN should be cleared for security
        _viewModel.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ApplyOverrideAsync_TimeAdjustment_CallsCorrectServiceMethod()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var adjustmentMinutes = 30;
        var reason = "Customer complaint resolution";
        var successResult = OverrideResult.Success();
        
        _mockManagerOverrideService.Setup(s => s.ValidateManagerAuthorizationAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(OverrideResult.Success());
        _mockManagerOverrideService.Setup(s => s.ApplyTimeAdjustmentAsync(sessionId, TimeSpan.FromMinutes(adjustmentMinutes), reason, It.IsAny<Guid>()))
            .ReturnsAsync(successResult);

        _viewModel.Initialize(sessionId, "Table 1", ManagerOverrideType.TimeAdjustment, TimeSpan.FromHours(1), 15.00m);
        _viewModel.ManagerPin = "1234";
        await _viewModel.AuthorizeCommand.ExecuteAsync(null); // Authorize first
        
        _viewModel.TimeAdjustmentMinutes = adjustmentMinutes;
        _viewModel.Reason = reason;

        bool overrideCompleted = false;
        bool requestCloseCalled = false;
        _viewModel.OverrideCompleted += (s, e) => overrideCompleted = true;
        _viewModel.RequestClose += (s, e) => requestCloseCalled = true;

        // Act
        await _viewModel.ApplyOverrideCommand.ExecuteAsync(null);

        // Assert
        _mockManagerOverrideService.Verify(s => s.ApplyTimeAdjustmentAsync(
            sessionId, TimeSpan.FromMinutes(adjustmentMinutes), reason, It.IsAny<Guid>()), Times.Once);
        _viewModel.HasError.Should().BeFalse();
        overrideCompleted.Should().BeTrue();
        requestCloseCalled.Should().BeTrue();
    }

    [Fact]
    public async Task ApplyOverrideAsync_PricingOverride_CallsCorrectServiceMethod()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var overrideAmount = 25.00m;
        var reason = "Customer loyalty discount";
        var successResult = OverrideResult.Success();
        
        _mockManagerOverrideService.Setup(s => s.ValidateManagerAuthorizationAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(OverrideResult.Success());
        _mockManagerOverrideService.Setup(s => s.ApplyPricingOverrideAsync(sessionId, It.IsAny<Money>(), reason, It.IsAny<Guid>()))
            .ReturnsAsync(successResult);

        _viewModel.Initialize(sessionId, "Table 1", ManagerOverrideType.PricingOverride, TimeSpan.FromHours(1), 30.00m);
        _viewModel.ManagerPin = "1234";
        await _viewModel.AuthorizeCommand.ExecuteAsync(null); // Authorize first
        
        _viewModel.PricingOverrideAmount = overrideAmount;
        _viewModel.Reason = reason;

        // Act
        await _viewModel.ApplyOverrideCommand.ExecuteAsync(null);

        // Assert
        _mockManagerOverrideService.Verify(s => s.ApplyPricingOverrideAsync(
            sessionId, It.Is<Money>(m => m.Amount == overrideAmount), reason, It.IsAny<Guid>()), Times.Once);
        _viewModel.HasError.Should().BeFalse();
    }

    [Fact]
    public async Task ApplyOverrideAsync_ForceEnd_CallsCorrectServiceMethod()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var reason = "Emergency situation";
        var successResult = OverrideResult.Success();
        
        _mockManagerOverrideService.Setup(s => s.ValidateManagerAuthorizationAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(OverrideResult.Success());
        _mockManagerOverrideService.Setup(s => s.ForceEndSessionAsync(sessionId, reason, It.IsAny<Guid>()))
            .ReturnsAsync(successResult);

        _viewModel.Initialize(sessionId, "Table 1", ManagerOverrideType.ForceEnd, TimeSpan.FromHours(1), 15.00m);
        _viewModel.ManagerPin = "1234";
        await _viewModel.AuthorizeCommand.ExecuteAsync(null); // Authorize first
        
        _viewModel.Reason = reason;

        // Act
        await _viewModel.ApplyOverrideCommand.ExecuteAsync(null);

        // Assert
        _mockManagerOverrideService.Verify(s => s.ForceEndSessionAsync(sessionId, reason, It.IsAny<Guid>()), Times.Once);
        _viewModel.HasError.Should().BeFalse();
    }

    [Fact]
    public async Task ApplyOverrideAsync_WithoutAuthorization_ShowsError()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        _viewModel.Initialize(sessionId, "Table 1", ManagerOverrideType.TimeAdjustment, TimeSpan.FromHours(1), 15.00m);
        _viewModel.TimeAdjustmentMinutes = 30;
        _viewModel.Reason = "Test reason";

        // Act
        await _viewModel.ApplyOverrideCommand.ExecuteAsync(null);

        // Assert
        _mockManagerOverrideService.Verify(s => s.ApplyTimeAdjustmentAsync(It.IsAny<Guid>(), It.IsAny<TimeSpan>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        _viewModel.HasError.Should().BeTrue();
        _viewModel.ErrorMessage.Should().Contain("authorization required");
    }

    [Fact]
    public async Task ApplyOverrideAsync_WithoutReason_ShowsError()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        _mockManagerOverrideService.Setup(s => s.ValidateManagerAuthorizationAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(OverrideResult.Success());

        _viewModel.Initialize(sessionId, "Table 1", ManagerOverrideType.TimeAdjustment, TimeSpan.FromHours(1), 15.00m);
        _viewModel.ManagerPin = "1234";
        await _viewModel.AuthorizeCommand.ExecuteAsync(null); // Authorize first
        
        _viewModel.TimeAdjustmentMinutes = 30;
        _viewModel.Reason = string.Empty;

        // Act
        await _viewModel.ApplyOverrideCommand.ExecuteAsync(null);

        // Assert
        _mockManagerOverrideService.Verify(s => s.ApplyTimeAdjustmentAsync(It.IsAny<Guid>(), It.IsAny<TimeSpan>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        _viewModel.HasError.Should().BeTrue();
        _viewModel.ErrorMessage.Should().Contain("reason");
    }

    [Theory]
    [InlineData(30, "Add 30 minutes")]
    [InlineData(-15, "Subtract 15 minutes")]
    [InlineData(0, "Add 0 minutes")]
    public void TimeAdjustmentDisplay_ShowsCorrectText(int minutes, string expectedDisplay)
    {
        // Arrange
        _viewModel.Initialize(Guid.NewGuid(), "Table 1", ManagerOverrideType.TimeAdjustment, TimeSpan.FromHours(1), 15.00m);
        _viewModel.TimeAdjustmentMinutes = minutes;

        // Act & Assert
        _viewModel.TimeAdjustmentDisplay.Should().Be(expectedDisplay);
    }

    [Fact]
    public void CommandCanExecute_ReflectsCorrectState()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        _viewModel.Initialize(sessionId, "Table 1", ManagerOverrideType.TimeAdjustment, TimeSpan.FromHours(1), 15.00m);

        // Test authorization command
        _viewModel.AuthorizeCommand.CanExecute(null).Should().BeFalse(); // No PIN
        _viewModel.ManagerPin = "1234";
        _viewModel.AuthorizeCommand.CanExecute(null).Should().BeTrue();

        // Test apply override command (not authorized)
        _viewModel.ApplyOverrideCommand.CanExecute(null).Should().BeFalse();

        // Simulate authorization
        _viewModel.IsAuthorized = true;
        _viewModel.TimeAdjustmentMinutes = 30;
        _viewModel.ApplyOverrideCommand.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public async Task AuthorizeAsync_ServiceThrowsException_ShowsErrorAndClearsPin()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var exception = new InvalidOperationException("Service error");
        
        _mockManagerOverrideService.Setup(s => s.ValidateManagerAuthorizationAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ThrowsAsync(exception);

        _viewModel.Initialize(sessionId, "Table 1", ManagerOverrideType.TimeAdjustment, TimeSpan.FromHours(1), 15.00m);
        _viewModel.ManagerPin = "1234";

        // Act
        await _viewModel.AuthorizeCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsAuthorized.Should().BeFalse();
        _viewModel.HasError.Should().BeTrue();
        _viewModel.ManagerPin.Should().BeEmpty(); // PIN should be cleared for security
        _viewModel.ErrorMessage.Should().Contain("Service error");
    }

    [Fact]
    public void ReasonCollections_ContainExpectedOptions()
    {
        // Time Adjustment Reasons
        _viewModel.TimeAdjustmentReasons.Should().Contain("Customer complaint resolution");
        _viewModel.TimeAdjustmentReasons.Should().Contain("Equipment malfunction");
        _viewModel.TimeAdjustmentReasons.Should().Contain("Staff error correction");

        // Pricing Override Reasons
        _viewModel.PricingOverrideReasons.Should().Contain("Customer loyalty discount");
        _viewModel.PricingOverrideReasons.Should().Contain("Service recovery");
        _viewModel.PricingOverrideReasons.Should().Contain("Promotional pricing");

        // Force End Reasons
        _viewModel.ForceEndReasons.Should().Contain("Emergency situation");
        _viewModel.ForceEndReasons.Should().Contain("Equipment failure");
        _viewModel.ForceEndReasons.Should().Contain("Safety concern");
    }
}
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Magidesk.Presentation.ViewModels.Dialogs;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magidesk.Presentation.Tests.ViewModels;

/// <summary>
/// Integration tests for SessionControlDialogViewModel.
/// Tests ViewModel interactions with application layer and UI workflow scenarios.
/// </summary>
public class SessionControlDialogViewModelTests
{
    private readonly Mock<ICommandHandler<PauseTableSessionCommand, PauseTableSessionResult>> _mockPauseHandler;
    private readonly Mock<ICommandHandler<ResumeTableSessionCommand, ResumeTableSessionResult>> _mockResumeHandler;
    private readonly Mock<ICommandHandler<UpdateGuestCountCommand, UpdateGuestCountResult>> _mockUpdateGuestCountHandler;
    private readonly Mock<ILogger<SessionControlDialogViewModel>> _mockLogger;
    private readonly SessionControlDialogViewModel _viewModel;

    public SessionControlDialogViewModelTests()
    {
        _mockPauseHandler = new Mock<ICommandHandler<PauseTableSessionCommand, PauseTableSessionResult>>();
        _mockResumeHandler = new Mock<ICommandHandler<ResumeTableSessionCommand, ResumeTableSessionResult>>();
        _mockUpdateGuestCountHandler = new Mock<ICommandHandler<UpdateGuestCountCommand, UpdateGuestCountResult>>();
        _mockLogger = new Mock<ILogger<SessionControlDialogViewModel>>();

        _viewModel = new SessionControlDialogViewModel(
            _mockPauseHandler.Object,
            _mockResumeHandler.Object,
            _mockUpdateGuestCountHandler.Object,
            _mockLogger.Object);
    }

    [Fact]
    public void Initialize_SetsPropertiesCorrectly()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var tableName = "Table 5";
        var sessionStatus = TableSessionStatus.Active;
        var guestCount = 4;
        var elapsedTime = TimeSpan.FromHours(2);
        var pausedDuration = TimeSpan.FromMinutes(15);
        var currentCharge = 30.00m;

        // Act
        _viewModel.Initialize(sessionId, tableName, sessionStatus, guestCount, elapsedTime, pausedDuration, currentCharge);

        // Assert
        _viewModel.SessionId.Should().Be(sessionId);
        _viewModel.TableName.Should().Be(tableName);
        _viewModel.SessionStatus.Should().Be(sessionStatus);
        _viewModel.CurrentGuestCount.Should().Be(guestCount);
        _viewModel.NewGuestCount.Should().Be(guestCount);
        _viewModel.ElapsedTime.Should().Be(elapsedTime);
        _viewModel.PausedDuration.Should().Be(pausedDuration);
        _viewModel.CurrentCharge.Should().Be(currentCharge);
        _viewModel.IsPaused.Should().BeFalse();
        _viewModel.HasError.Should().BeFalse();
    }

    [Fact]
    public void Initialize_WithPausedSession_SetsPausedStateCorrectly()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var sessionStatus = TableSessionStatus.Paused;

        // Act
        _viewModel.Initialize(sessionId, "Table 1", sessionStatus, 2, TimeSpan.FromHours(1), TimeSpan.Zero, 15.00m);

        // Assert
        _viewModel.IsPaused.Should().BeTrue();
        _viewModel.SessionStatus.Should().Be(TableSessionStatus.Paused);
    }

    [Fact]
    public async Task PauseSessionAsync_WithValidReason_CallsHandlerAndUpdatesState()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var pausedAt = DateTime.UtcNow;
        var expectedResult = new PauseTableSessionResult(sessionId, pausedAt);
        
        _mockPauseHandler.Setup(h => h.HandleAsync(It.IsAny<PauseTableSessionCommand>()))
            .ReturnsAsync(expectedResult);

        _viewModel.Initialize(sessionId, "Table 1", TableSessionStatus.Active, 2, TimeSpan.FromHours(1), TimeSpan.Zero, 15.00m);
        _viewModel.PauseReason = "Customer break";

        bool sessionControlCompleted = false;
        bool requestCloseCalled = false;
        _viewModel.SessionControlCompleted += (s, e) => sessionControlCompleted = true;
        _viewModel.RequestClose += (s, e) => requestCloseCalled = true;

        // Act
        await _viewModel.PauseSessionCommand.ExecuteAsync(null);

        // Assert
        _mockPauseHandler.Verify(h => h.HandleAsync(It.Is<PauseTableSessionCommand>(
            cmd => cmd.SessionId == sessionId && cmd.Reason == "Customer break")), Times.Once);
        
        _viewModel.IsPaused.Should().BeTrue();
        _viewModel.SessionStatus.Should().Be(TableSessionStatus.Paused);
        _viewModel.HasError.Should().BeFalse();
        sessionControlCompleted.Should().BeTrue();
        requestCloseCalled.Should().BeTrue();
    }

    [Fact]
    public async Task PauseSessionAsync_WithoutReason_ShowsError()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        _viewModel.Initialize(sessionId, "Table 1", TableSessionStatus.Active, 2, TimeSpan.FromHours(1), TimeSpan.Zero, 15.00m);
        _viewModel.PauseReason = string.Empty;

        // Act
        await _viewModel.PauseSessionCommand.ExecuteAsync(null);

        // Assert
        _mockPauseHandler.Verify(h => h.HandleAsync(It.IsAny<PauseTableSessionCommand>()), Times.Never);
        _viewModel.HasError.Should().BeTrue();
        _viewModel.ErrorMessage.Should().Contain("reason");
    }

    [Fact]
    public async Task ResumeSessionAsync_CallsHandlerAndUpdatesState()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var resumedAt = DateTime.UtcNow;
        var totalPausedDuration = TimeSpan.FromMinutes(30);
        var expectedResult = new ResumeTableSessionResult(sessionId, resumedAt, totalPausedDuration);
        
        _mockResumeHandler.Setup(h => h.HandleAsync(It.IsAny<ResumeTableSessionCommand>()))
            .ReturnsAsync(expectedResult);

        _viewModel.Initialize(sessionId, "Table 1", TableSessionStatus.Paused, 2, TimeSpan.FromHours(1), TimeSpan.FromMinutes(15), 15.00m);

        bool sessionControlCompleted = false;
        bool requestCloseCalled = false;
        _viewModel.SessionControlCompleted += (s, e) => sessionControlCompleted = true;
        _viewModel.RequestClose += (s, e) => requestCloseCalled = true;

        // Act
        await _viewModel.ResumeSessionCommand.ExecuteAsync(null);

        // Assert
        _mockResumeHandler.Verify(h => h.HandleAsync(It.Is<ResumeTableSessionCommand>(
            cmd => cmd.SessionId == sessionId)), Times.Once);
        
        _viewModel.IsPaused.Should().BeFalse();
        _viewModel.SessionStatus.Should().Be(TableSessionStatus.Active);
        _viewModel.PausedDuration.Should().Be(totalPausedDuration);
        _viewModel.HasError.Should().BeFalse();
        sessionControlCompleted.Should().BeTrue();
        requestCloseCalled.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateGuestCountAsync_WithValidCount_CallsHandlerAndUpdatesState()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var newGuestCount = 6;
        var updatedAt = DateTime.UtcNow;
        var expectedResult = new UpdateGuestCountResult(sessionId, 4, newGuestCount, updatedAt, null);
        
        _mockUpdateGuestCountHandler.Setup(h => h.HandleAsync(It.IsAny<UpdateGuestCountCommand>()))
            .ReturnsAsync(expectedResult);

        _viewModel.Initialize(sessionId, "Table 1", TableSessionStatus.Active, 4, TimeSpan.FromHours(1), TimeSpan.Zero, 15.00m);
        _viewModel.NewGuestCount = newGuestCount;

        bool sessionControlCompleted = false;
        bool requestCloseCalled = false;
        _viewModel.SessionControlCompleted += (s, e) => sessionControlCompleted = true;
        _viewModel.RequestClose += (s, e) => requestCloseCalled = true;

        // Act
        await _viewModel.UpdateGuestCountCommand.ExecuteAsync(null);

        // Assert
        _mockUpdateGuestCountHandler.Verify(h => h.HandleAsync(It.Is<UpdateGuestCountCommand>(
            cmd => cmd.SessionId == sessionId && cmd.NewGuestCount == newGuestCount)), Times.Once);
        
        _viewModel.CurrentGuestCount.Should().Be(newGuestCount);
        _viewModel.HasError.Should().BeFalse();
        sessionControlCompleted.Should().BeTrue();
        requestCloseCalled.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(21)]
    public async Task UpdateGuestCountAsync_WithInvalidCount_ShowsError(int invalidCount)
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        _viewModel.Initialize(sessionId, "Table 1", TableSessionStatus.Active, 4, TimeSpan.FromHours(1), TimeSpan.Zero, 15.00m);
        _viewModel.NewGuestCount = invalidCount;

        // Act
        await _viewModel.UpdateGuestCountCommand.ExecuteAsync(null);

        // Assert
        _mockUpdateGuestCountHandler.Verify(h => h.HandleAsync(It.IsAny<UpdateGuestCountCommand>()), Times.Never);
        _viewModel.HasError.Should().BeTrue();
        _viewModel.ErrorMessage.Should().Contain("between 1 and 20");
    }

    [Fact]
    public void CommandCanExecute_ReflectsCorrectState()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        
        // Test active session
        _viewModel.Initialize(sessionId, "Table 1", TableSessionStatus.Active, 4, TimeSpan.FromHours(1), TimeSpan.Zero, 15.00m);
        _viewModel.PauseReason = "Test reason";

        // Assert for active session
        _viewModel.PauseSessionCommand.CanExecute(null).Should().BeTrue();
        _viewModel.ResumeSessionCommand.CanExecute(null).Should().BeFalse();
        _viewModel.UpdateGuestCountCommand.CanExecute(null).Should().BeFalse(); // Same guest count

        // Test paused session
        _viewModel.Initialize(sessionId, "Table 1", TableSessionStatus.Paused, 4, TimeSpan.FromHours(1), TimeSpan.Zero, 15.00m);
        _viewModel.NewGuestCount = 6; // Different from current

        // Assert for paused session
        _viewModel.PauseSessionCommand.CanExecute(null).Should().BeFalse();
        _viewModel.ResumeSessionCommand.CanExecute(null).Should().BeTrue();
        _viewModel.UpdateGuestCountCommand.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public async Task PauseSessionAsync_HandlerThrowsException_ShowsError()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var exception = new InvalidOperationException("Session cannot be paused");
        
        _mockPauseHandler.Setup(h => h.HandleAsync(It.IsAny<PauseTableSessionCommand>()))
            .ThrowsAsync(exception);

        _viewModel.Initialize(sessionId, "Table 1", TableSessionStatus.Active, 2, TimeSpan.FromHours(1), TimeSpan.Zero, 15.00m);
        _viewModel.PauseReason = "Customer break";

        // Act
        await _viewModel.PauseSessionCommand.ExecuteAsync(null);

        // Assert
        _viewModel.HasError.Should().BeTrue();
        _viewModel.ErrorMessage.Should().Contain("Session cannot be paused");
        _viewModel.IsPaused.Should().BeFalse(); // State should not change on error
    }

    [Fact]
    public void PauseReasons_ContainsExpectedOptions()
    {
        // Assert
        _viewModel.PauseReasons.Should().Contain("Customer break");
        _viewModel.PauseReasons.Should().Contain("Equipment issue");
        _viewModel.PauseReasons.Should().Contain("Staff assistance needed");
        _viewModel.PauseReasons.Should().Contain("Customer request");
        _viewModel.PauseReasons.Should().Contain("Technical problem");
        _viewModel.PauseReasons.Should().Contain("Other");
    }

    [Fact]
    public void PropertyChanges_ClearErrorsWhenReasonProvided()
    {
        // Arrange
        _viewModel.Initialize(Guid.NewGuid(), "Table 1", TableSessionStatus.Active, 2, TimeSpan.FromHours(1), TimeSpan.Zero, 15.00m);
        _viewModel.HasError = true;
        _viewModel.ErrorMessage = "Test error";

        // Act
        _viewModel.PauseReason = "Customer break";

        // Assert
        _viewModel.HasError.Should().BeFalse();
        _viewModel.ErrorMessage.Should().BeNull();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.Commands.TableOperations;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Services;
using Magidesk.Presentation.ViewModels.Dialogs;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magidesk.Presentation.Tests.ViewModels;

/// <summary>
/// Integration tests for TableOperationsDialogViewModel.
/// Tests ViewModel interactions with application layer and table operation workflows.
/// </summary>
public class TableOperationsDialogViewModelTests
{
    private readonly Mock<ICommandHandler<MergeTablesCommand, MergeTablesResult>> _mockMergeTablesHandler;
    private readonly Mock<ICommandHandler<SplitTablesCommand, SplitTablesResult>> _mockSplitTablesHandler;
    private readonly Mock<ICommandHandler<TransferSessionCommand, TransferSessionResult>> _mockTransferSessionHandler;
    private readonly Mock<IQueryHandler<GetAvailableTablesQuery, IEnumerable<TableDto>>> _mockGetAvailableTablesHandler;
    private readonly Mock<ITableOperationsService> _mockTableOperationsService;
    private readonly Mock<ILogger<TableOperationsDialogViewModel>> _mockLogger;
    private readonly TableOperationsDialogViewModel _viewModel;

    public TableOperationsDialogViewModelTests()
    {
        _mockMergeTablesHandler = new Mock<ICommandHandler<MergeTablesCommand, MergeTablesResult>>();
        _mockSplitTablesHandler = new Mock<ICommandHandler<SplitTablesCommand, SplitTablesResult>>();
        _mockTransferSessionHandler = new Mock<ICommandHandler<TransferSessionCommand, TransferSessionResult>>();
        _mockGetAvailableTablesHandler = new Mock<IQueryHandler<GetAvailableTablesQuery, IEnumerable<TableDto>>>();
        _mockTableOperationsService = new Mock<ITableOperationsService>();
        _mockLogger = new Mock<ILogger<TableOperationsDialogViewModel>>();

        _viewModel = new TableOperationsDialogViewModel(
            _mockMergeTablesHandler.Object,
            _mockSplitTablesHandler.Object,
            _mockTransferSessionHandler.Object,
            _mockGetAvailableTablesHandler.Object,
            _mockTableOperationsService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task InitializeAsync_MergeOperation_SetsPropertiesCorrectly()
    {
        // Arrange
        var primaryTableId = Guid.NewGuid();
        var primaryTableName = "Table 5";
        var availableTables = CreateAvailableTables();

        _mockGetAvailableTablesHandler.Setup(h => h.HandleAsync(It.IsAny<GetAvailableTablesQuery>()))
            .ReturnsAsync(availableTables);

        // Act
        await _viewModel.InitializeAsync(
            TableOperationType.Merge,
            primaryTableId,
            primaryTableName);

        // Assert
        _viewModel.OperationType.Should().Be(TableOperationType.Merge);
        _viewModel.PrimaryTableId.Should().Be(primaryTableId);
        _viewModel.PrimaryTableName.Should().Be(primaryTableName);
        _viewModel.IsMergeOperation.Should().BeTrue();
        _viewModel.IsSplitOperation.Should().BeFalse();
        _viewModel.IsTransferOperation.Should().BeFalse();
        _viewModel.OperationTypeDisplay.Should().Be("Merge Tables");
        _viewModel.CurrentReasons.Should().BeSameAs(_viewModel.MergeReasons);
        _viewModel.AvailableTables.Should().HaveCount(2); // Excludes primary table
    }

    [Fact]
    public async Task InitializeAsync_SplitOperation_InitializesSplitAllocations()
    {
        // Arrange
        var primaryTableId = Guid.NewGuid();
        var primarySessionCharge = 60.00m;
        var primaryGuestCount = 6;
        var availableTables = CreateAvailableTables();

        _mockGetAvailableTablesHandler.Setup(h => h.HandleAsync(It.IsAny<GetAvailableTablesQuery>()))
            .ReturnsAsync(availableTables);

        // Act
        await _viewModel.InitializeAsync(
            TableOperationType.Split,
            primaryTableId,
            "Table 5",
            Guid.NewGuid(),
            primarySessionCharge,
            TimeSpan.FromHours(2),
            primaryGuestCount);

        // Assert
        _viewModel.OperationType.Should().Be(TableOperationType.Split);
        _viewModel.IsSplitOperation.Should().BeTrue();
        _viewModel.PrimarySessionCharge.Should().Be(primarySessionCharge);
        _viewModel.PrimaryGuestCount.Should().Be(primaryGuestCount);
        _viewModel.SplitAllocations.Should().HaveCount(1); // Primary table allocation
        _viewModel.SplitAllocations[0].TargetTableId.Should().Be(primaryTableId);
        _viewModel.SplitAllocations[0].AllocatedAmount.Should().Be(primarySessionCharge / 2);
        _viewModel.CurrentReasons.Should().BeSameAs(_viewModel.SplitReasons);
    }

    [Fact]
    public async Task InitializeAsync_TransferOperation_SetsPropertiesCorrectly()
    {
        // Arrange
        var primaryTableId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var availableTables = CreateAvailableTables();

        _mockGetAvailableTablesHandler.Setup(h => h.HandleAsync(It.IsAny<GetAvailableTablesQuery>()))
            .ReturnsAsync(availableTables);

        // Act
        await _viewModel.InitializeAsync(
            TableOperationType.Transfer,
            primaryTableId,
            "Table 5",
            sessionId,
            30.00m,
            TimeSpan.FromHours(1.5),
            4);

        // Assert
        _viewModel.OperationType.Should().Be(TableOperationType.Transfer);
        _viewModel.IsTransferOperation.Should().BeTrue();
        _viewModel.PrimarySessionId.Should().Be(sessionId);
        _viewModel.CurrentReasons.Should().BeSameAs(_viewModel.TransferReasons);
    }

    [Fact]
    public async Task ExecuteOperationAsync_MergeOperation_CallsHandlerAndCompletesSuccessfully()
    {
        // Arrange
        var primaryTableId = Guid.NewGuid();
        var selectedTableId = Guid.NewGuid();
        var mergedTableId = Guid.NewGuid();
        var reason = "Large group accommodation";

        var availableTables = CreateAvailableTables();
        var mergeResult = new MergeTablesResult(mergedTableId, "Merged Table", new[] { primaryTableId, selectedTableId });

        _mockGetAvailableTablesHandler.Setup(h => h.HandleAsync(It.IsAny<GetAvailableTablesQuery>()))
            .ReturnsAsync(availableTables);
        _mockTableOperationsService.Setup(s => s.ValidateTableMergeAsync(It.IsAny<IList<Guid>>()))
            .ReturnsAsync(new TableOperationValidationResult(true, null));
        _mockMergeTablesHandler.Setup(h => h.HandleAsync(It.IsAny<MergeTablesCommand>()))
            .ReturnsAsync(mergeResult);

        await _viewModel.InitializeAsync(TableOperationType.Merge, primaryTableId, "Table 5");
        
        // Select a table and set reason
        _viewModel.SelectedTables.Add(availableTables.First(t => t.Id == selectedTableId));
        _viewModel.OperationReason = reason;
        _viewModel.CanExecuteOperation = true;

        bool operationCompleted = false;
        bool requestCloseCalled = false;
        _viewModel.OperationCompleted += (s, e) => operationCompleted = true;
        _viewModel.RequestClose += (s, e) => requestCloseCalled = true;

        // Act
        await _viewModel.ExecuteOperationCommand.ExecuteAsync(null);

        // Assert
        _mockMergeTablesHandler.Verify(h => h.HandleAsync(It.Is<MergeTablesCommand>(
            cmd => cmd.TableIds.Contains(primaryTableId) && 
                   cmd.TableIds.Contains(selectedTableId) && 
                   cmd.Reason == reason)), Times.Once);
        
        _viewModel.HasError.Should().BeFalse();
        operationCompleted.Should().BeTrue();
        requestCloseCalled.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteOperationAsync_SplitOperation_CallsHandlerAndCompletesSuccessfully()
    {
        // Arrange
        var primaryTableId = Guid.NewGuid();
        var targetTableId = Guid.NewGuid();
        var sessionCharge = 60.00m;
        var reason = "Group size reduction";

        var availableTables = CreateAvailableTables();
        var splitResult = new SplitTablesResult(new[] { primaryTableId, targetTableId });

        _mockGetAvailableTablesHandler.Setup(h => h.HandleAsync(It.IsAny<GetAvailableTablesQuery>()))
            .ReturnsAsync(availableTables);
        _mockSplitTablesHandler.Setup(h => h.HandleAsync(It.IsAny<SplitTablesCommand>()))
            .ReturnsAsync(splitResult);

        await _viewModel.InitializeAsync(
            TableOperationType.Split, 
            primaryTableId, 
            "Table 5", 
            Guid.NewGuid(), 
            sessionCharge, 
            TimeSpan.FromHours(2), 
            6);

        // Add another split allocation
        _viewModel.AddSplitAllocationCommand.Execute(null);
        _viewModel.SplitAllocations[1].AllocatedAmount = sessionCharge / 2; // Make allocations valid
        _viewModel.OperationReason = reason;

        bool operationCompleted = false;
        _viewModel.OperationCompleted += (s, e) => operationCompleted = true;

        // Act
        await _viewModel.ExecuteOperationCommand.ExecuteAsync(null);

        // Assert
        _mockSplitTablesHandler.Verify(h => h.HandleAsync(It.Is<SplitTablesCommand>(
            cmd => cmd.OriginalTableId == primaryTableId && 
                   cmd.Reason == reason && 
                   cmd.Allocations.Count == 2)), Times.Once);
        
        _viewModel.HasError.Should().BeFalse();
        operationCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteOperationAsync_TransferOperation_CallsHandlerAndCompletesSuccessfully()
    {
        // Arrange
        var primaryTableId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var targetTableId = Guid.NewGuid();
        var reason = "Customer preference";

        var availableTables = CreateAvailableTables();
        var transferResult = new TransferSessionResult(sessionId, targetTableId, "Table 3");

        _mockGetAvailableTablesHandler.Setup(h => h.HandleAsync(It.IsAny<GetAvailableTablesQuery>()))
            .ReturnsAsync(availableTables);
        _mockTableOperationsService.Setup(s => s.ValidateSessionTransferAsync(sessionId, targetTableId))
            .ReturnsAsync(new TableOperationValidationResult(true, null));
        _mockTransferSessionHandler.Setup(h => h.HandleAsync(It.IsAny<TransferSessionCommand>()))
            .ReturnsAsync(transferResult);

        await _viewModel.InitializeAsync(
            TableOperationType.Transfer, 
            primaryTableId, 
            "Table 5", 
            sessionId, 
            30.00m, 
            TimeSpan.FromHours(1.5), 
            4);

        // Select target table
        _viewModel.SelectedTables.Add(availableTables.First(t => t.Id == targetTableId));
        _viewModel.OperationReason = reason;
        _viewModel.CanExecuteOperation = true;

        bool operationCompleted = false;
        _viewModel.OperationCompleted += (s, e) => operationCompleted = true;

        // Act
        await _viewModel.ExecuteOperationCommand.ExecuteAsync(null);

        // Assert
        _mockTransferSessionHandler.Verify(h => h.HandleAsync(It.Is<TransferSessionCommand>(
            cmd => cmd.SessionId == sessionId && 
                   cmd.TargetTableId == targetTableId && 
                   cmd.Reason == reason)), Times.Once);
        
        _viewModel.HasError.Should().BeFalse();
        operationCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateOperationAsync_MergeOperation_ValidatesCorrectly()
    {
        // Arrange
        var primaryTableId = Guid.NewGuid();
        var selectedTableId = Guid.NewGuid();
        var availableTables = CreateAvailableTables();

        _mockGetAvailableTablesHandler.Setup(h => h.HandleAsync(It.IsAny<GetAvailableTablesQuery>()))
            .ReturnsAsync(availableTables);
        _mockTableOperationsService.Setup(s => s.ValidateTableMergeAsync(It.IsAny<IList<Guid>>()))
            .ReturnsAsync(new TableOperationValidationResult(true, null));

        await _viewModel.InitializeAsync(TableOperationType.Merge, primaryTableId, "Table 5");
        _viewModel.SelectedTables.Add(availableTables.First(t => t.Id == selectedTableId));

        // Act
        await _viewModel.ValidateOperationCommand.ExecuteAsync(null);

        // Assert
        _mockTableOperationsService.Verify(s => s.ValidateTableMergeAsync(
            It.Is<IList<Guid>>(list => list.Contains(primaryTableId) && list.Contains(selectedTableId))), Times.Once);
        _viewModel.CanExecuteOperation.Should().BeTrue();
        _viewModel.HasError.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateOperationAsync_TransferOperation_ValidatesCorrectly()
    {
        // Arrange
        var primaryTableId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var targetTableId = Guid.NewGuid();
        var availableTables = CreateAvailableTables();

        _mockGetAvailableTablesHandler.Setup(h => h.HandleAsync(It.IsAny<GetAvailableTablesQuery>()))
            .ReturnsAsync(availableTables);
        _mockTableOperationsService.Setup(s => s.ValidateSessionTransferAsync(sessionId, targetTableId))
            .ReturnsAsync(new TableOperationValidationResult(true, null));

        await _viewModel.InitializeAsync(
            TableOperationType.Transfer, 
            primaryTableId, 
            "Table 5", 
            sessionId);

        _viewModel.SelectedTables.Add(availableTables.First(t => t.Id == targetTableId));

        // Act
        await _viewModel.ValidateOperationCommand.ExecuteAsync(null);

        // Assert
        _mockTableOperationsService.Verify(s => s.ValidateSessionTransferAsync(sessionId, targetTableId), Times.Once);
        _viewModel.CanExecuteOperation.Should().BeTrue();
        _viewModel.HasError.Should().BeFalse();
    }

    [Fact]
    public void AddSplitAllocation_AddsNewAllocationCorrectly()
    {
        // Arrange
        var primaryTableId = Guid.NewGuid();
        var availableTables = CreateAvailableTables();

        _mockGetAvailableTablesHandler.Setup(h => h.HandleAsync(It.IsAny<GetAvailableTablesQuery>()))
            .ReturnsAsync(availableTables);

        _viewModel.InitializeAsync(
            TableOperationType.Split, 
            primaryTableId, 
            "Table 5", 
            Guid.NewGuid(), 
            60.00m, 
            TimeSpan.FromHours(2), 
            6).Wait();

        var initialCount = _viewModel.SplitAllocations.Count;

        // Act
        _viewModel.AddSplitAllocationCommand.Execute(null);

        // Assert
        _viewModel.SplitAllocations.Should().HaveCount(initialCount + 1);
        var newAllocation = _viewModel.SplitAllocations.Last();
        newAllocation.TargetTableId.Should().NotBe(primaryTableId);
        newAllocation.AllocatedAmount.Should().BeGreaterThan(0);
        newAllocation.GuestCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public void RemoveSplitAllocation_RemovesAllocationCorrectly()
    {
        // Arrange
        var primaryTableId = Guid.NewGuid();
        var availableTables = CreateAvailableTables();

        _mockGetAvailableTablesHandler.Setup(h => h.HandleAsync(It.IsAny<GetAvailableTablesQuery>()))
            .ReturnsAsync(availableTables);

        _viewModel.InitializeAsync(
            TableOperationType.Split, 
            primaryTableId, 
            "Table 5", 
            Guid.NewGuid(), 
            60.00m, 
            TimeSpan.FromHours(2), 
            6).Wait();

        _viewModel.AddSplitAllocationCommand.Execute(null);
        var allocationToRemove = _viewModel.SplitAllocations.Last();
        var initialCount = _viewModel.SplitAllocations.Count;

        // Act
        _viewModel.RemoveSplitAllocationCommand.Execute(allocationToRemove);

        // Assert
        _viewModel.SplitAllocations.Should().HaveCount(initialCount - 1);
        _viewModel.SplitAllocations.Should().NotContain(allocationToRemove);
    }

    [Fact]
    public void RemoveSplitAllocation_CannotRemovePrimaryTableAllocation()
    {
        // Arrange
        var primaryTableId = Guid.NewGuid();
        var availableTables = CreateAvailableTables();

        _mockGetAvailableTablesHandler.Setup(h => h.HandleAsync(It.IsAny<GetAvailableTablesQuery>()))
            .ReturnsAsync(availableTables);

        _viewModel.InitializeAsync(
            TableOperationType.Split, 
            primaryTableId, 
            "Table 5", 
            Guid.NewGuid(), 
            60.00m, 
            TimeSpan.FromHours(2), 
            6).Wait();

        var primaryAllocation = _viewModel.SplitAllocations.First(a => a.TargetTableId == primaryTableId);
        var initialCount = _viewModel.SplitAllocations.Count;

        // Act
        _viewModel.RemoveSplitAllocationCommand.Execute(primaryAllocation);

        // Assert
        _viewModel.SplitAllocations.Should().HaveCount(initialCount); // No change
        _viewModel.SplitAllocations.Should().Contain(primaryAllocation);
    }

    [Fact]
    public void IsSplitAllocationValid_ReturnsTrueWhenAllocationsMatchCharge()
    {
        // Arrange
        var primaryTableId = Guid.NewGuid();
        var sessionCharge = 60.00m;
        var availableTables = CreateAvailableTables();

        _mockGetAvailableTablesHandler.Setup(h => h.HandleAsync(It.IsAny<GetAvailableTablesQuery>()))
            .ReturnsAsync(availableTables);

        _viewModel.InitializeAsync(
            TableOperationType.Split, 
            primaryTableId, 
            "Table 5", 
            Guid.NewGuid(), 
            sessionCharge, 
            TimeSpan.FromHours(2), 
            6).Wait();

        // Add second allocation to match total
        _viewModel.AddSplitAllocationCommand.Execute(null);
        _viewModel.SplitAllocations[1].AllocatedAmount = sessionCharge / 2;

        // Act & Assert
        _viewModel.TotalSplitAllocation.Should().Be(sessionCharge);
        _viewModel.IsSplitAllocationValid.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteOperationAsync_WithoutReason_ShowsError()
    {
        // Arrange
        var primaryTableId = Guid.NewGuid();
        var availableTables = CreateAvailableTables();

        _mockGetAvailableTablesHandler.Setup(h => h.HandleAsync(It.IsAny<GetAvailableTablesQuery>()))
            .ReturnsAsync(availableTables);

        await _viewModel.InitializeAsync(TableOperationType.Merge, primaryTableId, "Table 5");
        _viewModel.SelectedTables.Add(availableTables.First());
        _viewModel.OperationReason = string.Empty; // No reason provided

        // Act
        await _viewModel.ExecuteOperationCommand.ExecuteAsync(null);

        // Assert
        _mockMergeTablesHandler.Verify(h => h.HandleAsync(It.IsAny<MergeTablesCommand>()), Times.Never);
        _viewModel.HasError.Should().BeTrue();
        _viewModel.ErrorMessage.Should().Contain("reason");
    }

    [Fact]
    public async Task LoadAvailableTablesAsync_HandlerThrowsException_ShowsError()
    {
        // Arrange
        var exception = new InvalidOperationException("Database error");
        _mockGetAvailableTablesHandler.Setup(h => h.HandleAsync(It.IsAny<GetAvailableTablesQuery>()))
            .ThrowsAsync(exception);

        // Act
        await _viewModel.InitializeAsync(TableOperationType.Merge, Guid.NewGuid(), "Table 5");

        // Assert
        _viewModel.HasError.Should().BeTrue();
        _viewModel.ErrorMessage.Should().Contain("Database error");
        _viewModel.AvailableTables.Should().BeEmpty();
    }

    [Theory]
    [InlineData(TableOperationType.Merge, "Merge Tables")]
    [InlineData(TableOperationType.Split, "Split Tables")]
    [InlineData(TableOperationType.Transfer, "Transfer Session")]
    public void OperationTypeDisplay_ReturnsCorrectText(TableOperationType operationType, string expectedDisplay)
    {
        // Arrange
        _viewModel.OperationType = operationType;

        // Act & Assert
        _viewModel.OperationTypeDisplay.Should().Be(expectedDisplay);
    }

    [Fact]
    public void ReasonCollections_ContainExpectedOptions()
    {
        // Merge Reasons
        _viewModel.MergeReasons.Should().Contain("Large group accommodation");
        _viewModel.MergeReasons.Should().Contain("Customer request");
        _viewModel.MergeReasons.Should().Contain("Tournament setup");

        // Split Reasons
        _viewModel.SplitReasons.Should().Contain("Group size reduction");
        _viewModel.SplitReasons.Should().Contain("Separate billing request");
        _viewModel.SplitReasons.Should().Contain("Table availability");

        // Transfer Reasons
        _viewModel.TransferReasons.Should().Contain("Table maintenance required");
        _viewModel.TransferReasons.Should().Contain("Customer preference");
        _viewModel.TransferReasons.Should().Contain("Equipment issue");
    }

    private static List<TableDto> CreateAvailableTables()
    {
        return new List<TableDto>
        {
            new TableDto
            {
                Id = Guid.NewGuid(),
                TableNumber = 2,
                Name = "Table 2",
                StatusDisplay = "Available"
            },
            new TableDto
            {
                Id = Guid.NewGuid(),
                TableNumber = 3,
                Name = "Table 3",
                StatusDisplay = "Available"
            },
            new TableDto
            {
                Id = Guid.NewGuid(),
                TableNumber = 4,
                Name = "Table 4",
                StatusDisplay = "Available"
            }
        };
    }
}
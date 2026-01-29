using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.Commands.TableOperations;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Handlers;

public class TableOperationsCommandHandlerTests
{
    private readonly Mock<ITableOperationsService> _tableOperationsServiceMock;
    private readonly MergeTablesCommandHandler _mergeHandler;
    private readonly SplitTablesCommandHandler _splitHandler;

    public TableOperationsCommandHandlerTests()
    {
        _tableOperationsServiceMock = new Mock<ITableOperationsService>();
        _mergeHandler = new MergeTablesCommandHandler(_tableOperationsServiceMock.Object);
        _splitHandler = new SplitTablesCommandHandler(_tableOperationsServiceMock.Object);
    }

    [Fact]
    public async Task MergeHandler_ShouldMergeTables_WhenValidCommand()
    {
        // Arrange
        var primaryTableId = Guid.NewGuid();
        var secondaryTableId1 = Guid.NewGuid();
        var secondaryTableId2 = Guid.NewGuid();
        var staffId = Guid.NewGuid();
        var reason = "Large group needs more space";
        var mergedSessionId = Guid.NewGuid();
        
        var command = new MergeTablesCommand(
            primaryTableId, 
            new[] { secondaryTableId1, secondaryTableId2 }, 
            reason, 
            staffId);
        
        var operationData = new TableOperationData(
            OperationId: Guid.NewGuid(),
            OperationType: TableOperationType.Merge,
            TableIds: new[] { primaryTableId, secondaryTableId1, secondaryTableId2 },
            ResultingSessionId: mergedSessionId,
            ResultingSessionIds: null,
            TotalChargesBefore: new Money(60m),
            TotalChargesAfter: new Money(60m),
            OperationTimestamp: DateTime.UtcNow,
            StaffId: staffId,
            Reason: reason
        );
        
        var operationResult = TableOperationResult.Success(operationData);
        
        _tableOperationsServiceMock
            .Setup(s => s.MergeTablesAsync(primaryTableId, It.IsAny<IEnumerable<Guid>>(), reason, staffId))
            .ReturnsAsync(operationResult);

        // Act
        var result = await _mergeHandler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.MergedSessionId.Should().Be(mergedSessionId);
        result.MergedTableIds.Should().Contain(primaryTableId);
        result.MergedTableIds.Should().Contain(secondaryTableId1);
        result.MergedTableIds.Should().Contain(secondaryTableId2);
        result.TotalCharge.Should().Be(60m);
        result.StaffId.Should().Be(staffId);
        
        _tableOperationsServiceMock.Verify(s => s.MergeTablesAsync(
            primaryTableId, 
            It.Is<IEnumerable<Guid>>(ids => ids.Contains(secondaryTableId1) && ids.Contains(secondaryTableId2)), 
            reason, 
            staffId), Times.Once);
    }

    [Fact]
    public async Task MergeHandler_ShouldThrowArgumentException_WhenPrimaryTableIdIsEmpty()
    {
        // Arrange
        var command = new MergeTablesCommand(Guid.Empty, new[] { Guid.NewGuid() }, "reason", Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _mergeHandler.HandleAsync(command));
    }

    [Fact]
    public async Task MergeHandler_ShouldThrowArgumentException_WhenSecondaryTableIdsIsEmpty()
    {
        // Arrange
        var command = new MergeTablesCommand(Guid.NewGuid(), Array.Empty<Guid>(), "reason", Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _mergeHandler.HandleAsync(command));
    }

    [Fact]
    public async Task MergeHandler_ShouldThrowArgumentException_WhenReasonIsEmpty()
    {
        // Arrange
        var command = new MergeTablesCommand(Guid.NewGuid(), new[] { Guid.NewGuid() }, "", Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _mergeHandler.HandleAsync(command));
    }

    [Fact]
    public async Task SplitHandler_ShouldSplitTables_WhenValidCommand()
    {
        // Arrange
        var mergedSessionId = Guid.NewGuid();
        var tableId1 = Guid.NewGuid();
        var tableId2 = Guid.NewGuid();
        var staffId = Guid.NewGuid();
        var reason = "Group wants separate bills";
        
        var splitAllocations = new List<TableSplitAllocationInfo>
        {
            new(tableId1, 0.6m, 3), // 60/100 = 0.6
            new(tableId2, 0.4m, 2)  // 40/100 = 0.4
        };
        
        var command = new SplitTablesCommand(mergedSessionId, splitAllocations, reason, staffId);
        
        var resultingSessionIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        
        var operationData = new TableOperationData(
            OperationId: Guid.NewGuid(),
            OperationType: TableOperationType.Split,
            TableIds: new[] { tableId1, tableId2 },
            ResultingSessionId: null,
            ResultingSessionIds: resultingSessionIds,
            TotalChargesBefore: new Money(100m),
            TotalChargesAfter: new Money(100m),
            OperationTimestamp: DateTime.UtcNow,
            StaffId: staffId,
            Reason: reason
        );
        
        var operationResult = TableOperationResult.Success(operationData);
        
        _tableOperationsServiceMock
            .Setup(s => s.SplitTablesAsync(mergedSessionId, It.IsAny<TableSplitAllocation>(), reason, staffId))
            .ReturnsAsync(operationResult);

        // Act
        var result = await _splitHandler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.OriginalMergedSessionId.Should().Be(mergedSessionId);
        result.SplitSessions.Should().HaveCount(2);
        result.TotalChargesAllocated.Should().Be(100m);
        result.StaffId.Should().Be(staffId);
        
        _tableOperationsServiceMock.Verify(s => s.SplitTablesAsync(
            mergedSessionId, 
            It.IsAny<TableSplitAllocation>(), 
            reason, 
            staffId), Times.Once);
    }

    [Fact]
    public async Task SplitHandler_ShouldThrowArgumentException_WhenChargeAllocationDoesNotSumTo100()
    {
        // Arrange
        var splitAllocations = new List<TableSplitAllocationInfo>
        {
            new(Guid.NewGuid(), 0.6m, 3),
            new(Guid.NewGuid(), 0.3m, 2)
        };
        
        var command = new SplitTablesCommand(Guid.NewGuid(), splitAllocations, "reason", Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _splitHandler.HandleAsync(command));
    }

    [Fact]
    public async Task SplitHandler_ShouldThrowArgumentException_WhenMergedSessionIdIsEmpty()
    {
        // Arrange
        var splitAllocations = new List<TableSplitAllocationInfo>
        {
            new(Guid.NewGuid(), 1.0m, 2)
        };
        var command = new SplitTablesCommand(Guid.Empty, splitAllocations, "reason", Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _splitHandler.HandleAsync(command));
    }

    [Fact]
    public async Task SplitHandler_ShouldThrowArgumentException_WhenChargeAllocationIsEmpty()
    {
        // Arrange
        var splitAllocations = new List<TableSplitAllocationInfo>();
        var command = new SplitTablesCommand(Guid.NewGuid(), splitAllocations, "reason", Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _splitHandler.HandleAsync(command));
    }

    [Fact]
    public async Task SplitHandler_ShouldThrowInvalidOperationException_WhenOperationFails()
    {
        // Arrange
        var splitAllocations = new List<TableSplitAllocationInfo>
        {
            new(Guid.NewGuid(), 1.0m, 2)
        };
        var command = new SplitTablesCommand(Guid.NewGuid(), splitAllocations, "reason", Guid.NewGuid());
        
        var failureResult = TableOperationResult.InvalidOperation("Cannot split session that is not merged");
        
        _tableOperationsServiceMock
            .Setup(s => s.SplitTablesAsync(It.IsAny<Guid>(), It.IsAny<TableSplitAllocation>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(failureResult);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _splitHandler.HandleAsync(command));
    }
}
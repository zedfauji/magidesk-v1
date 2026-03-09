using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.Commands.Inventory;
using Magidesk.Domain.Entities;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Commands.Inventory.Handlers;

/// <summary>
/// Stock adjustment tests for UpdateInventoryItemCommandHandler.
/// </summary>
public partial class UpdateInventoryItemCommandHandlerTests
{
    [Fact]
    public async Task Handle_StockQuantityChanged_CreatesAdjustmentRecord()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "TEST-001",
            null);

        var command = new UpdateInventoryItemCommand(
            Id: itemId,
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 150m,
            ReorderPoint: 20m,
            SkuCode: "TEST-001",
            CategoryId: null,
            IsActive: true);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _mockUserContextService
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        InventoryAdjustment? capturedAdjustment = null;
        _mockAdjustmentRepository
            .Setup(x => x.AddAsync(It.IsAny<InventoryAdjustment>(), It.IsAny<CancellationToken>()))
            .Callback<InventoryAdjustment, CancellationToken>((adj, _) => capturedAdjustment = adj)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingItem.StockQuantity.Should().Be(150m);
        
        capturedAdjustment.Should().NotBeNull();
        capturedAdjustment!.InventoryItemId.Should().Be(existingItem.Id);
        capturedAdjustment.QuantityDelta.Should().Be(50m);
        capturedAdjustment.Reason.Should().Be("Stock adjustment via update");
        capturedAdjustment.UserId.Should().Be(userId);

        _mockAdjustmentRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryAdjustment>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_StockQuantityUnchanged_DoesNotCreateAdjustmentRecord()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "TEST-001",
            null);

        var command = new UpdateInventoryItemCommand(
            Id: itemId,
            Name: "Updated Name",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 25m,
            SkuCode: "TEST-001",
            CategoryId: null,
            IsActive: true);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingItem.Name.Should().Be("Updated Name");
        existingItem.ReorderPoint.Should().Be(25m);
        
        _mockAdjustmentRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryAdjustment>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Adjustment record should not be created when stock quantity is unchanged");
    }

    [Fact]
    public async Task Handle_StockQuantityDecreased_CreatesNegativeAdjustmentRecord()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "TEST-001",
            null);

        var command = new UpdateInventoryItemCommand(
            Id: itemId,
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 75m,
            ReorderPoint: 20m,
            SkuCode: "TEST-001",
            CategoryId: null,
            IsActive: true);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _mockUserContextService
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        InventoryAdjustment? capturedAdjustment = null;
        _mockAdjustmentRepository
            .Setup(x => x.AddAsync(It.IsAny<InventoryAdjustment>(), It.IsAny<CancellationToken>()))
            .Callback<InventoryAdjustment, CancellationToken>((adj, _) => capturedAdjustment = adj)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingItem.StockQuantity.Should().Be(75m);
        
        capturedAdjustment.Should().NotBeNull();
        capturedAdjustment!.QuantityDelta.Should().Be(-25m);
        capturedAdjustment.Reason.Should().Be("Stock adjustment via update");

        _mockAdjustmentRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryAdjustment>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyUserContext_CreatesAdjustmentWithNullUserId()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "TEST-001",
            null);

        var command = new UpdateInventoryItemCommand(
            Id: itemId,
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 150m,
            ReorderPoint: 20m,
            SkuCode: "TEST-001",
            CategoryId: null,
            IsActive: true);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _mockUserContextService
            .Setup(x => x.GetCurrentUserId())
            .Returns(Guid.Empty);

        InventoryAdjustment? capturedAdjustment = null;
        _mockAdjustmentRepository
            .Setup(x => x.AddAsync(It.IsAny<InventoryAdjustment>(), It.IsAny<CancellationToken>()))
            .Callback<InventoryAdjustment, CancellationToken>((adj, _) => capturedAdjustment = adj)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedAdjustment.Should().NotBeNull();
        capturedAdjustment!.UserId.Should().BeNull("UserId should be null when user context returns Guid.Empty");
    }
}

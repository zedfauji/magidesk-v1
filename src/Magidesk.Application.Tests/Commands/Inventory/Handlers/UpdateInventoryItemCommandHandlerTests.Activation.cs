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
/// Activation and deactivation tests for UpdateInventoryItemCommandHandler.
/// </summary>
public partial class UpdateInventoryItemCommandHandlerTests
{
    [Fact]
    public async Task Handle_ActivationChange_UpdatesIsActiveStatus()
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
        existingItem.Activate();

        var command = new UpdateInventoryItemCommand(
            Id: itemId,
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "TEST-001",
            CategoryId: null,
            IsActive: false);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingItem.IsActive.Should().BeFalse();

        _mockItemRepository.Verify(
            x => x.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_Deactivation_SetsIsActiveToFalse()
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
        existingItem.Activate();

        var command = new UpdateInventoryItemCommand(
            Id: itemId,
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "TEST-001",
            CategoryId: null,
            IsActive: false);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingItem.IsActive.Should().BeFalse();

        _mockItemRepository.Verify(
            x => x.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_Activation_SetsIsActiveToTrue()
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
        existingItem.Deactivate();

        var command = new UpdateInventoryItemCommand(
            Id: itemId,
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "TEST-001",
            CategoryId: null,
            IsActive: true);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingItem.IsActive.Should().BeTrue();

        _mockItemRepository.Verify(
            x => x.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}

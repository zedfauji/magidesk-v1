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
/// SKU validation tests for UpdateInventoryItemCommandHandler.
/// </summary>
public partial class UpdateInventoryItemCommandHandlerTests
{
    [Fact]
    public async Task Handle_DuplicateSkuExcludingSelf_ThrowsInvalidOperationException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var otherItemId = Guid.NewGuid();
        
        var existingItem = InventoryItem.Create(
            "Original Item",
            "kg",
            100m,
            20m,
            "ORIG-001",
            null);

        var otherItem = InventoryItem.Create(
            "Other Item",
            "kg",
            50m,
            10m,
            "DUPLICATE-SKU",
            null);

        var command = new UpdateInventoryItemCommand(
            Id: itemId,
            Name: "Updated Item",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "DUPLICATE-SKU",
            CategoryId: null,
            IsActive: true);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _mockItemRepository
            .Setup(x => x.GetBySkuCodeAsync("DUPLICATE-SKU", It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherItem);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("SKU code already exists");

        _mockItemRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Item should not be updated when SKU is duplicate");
    }

    [Fact]
    public async Task Handle_SkuUnchanged_DoesNotCheckForDuplicates()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "UNCHANGED-SKU",
            null);

        var command = new UpdateInventoryItemCommand(
            Id: itemId,
            Name: "Updated Name",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "UNCHANGED-SKU",
            CategoryId: null,
            IsActive: true);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingItem.Name.Should().Be("Updated Name");
        
        _mockItemRepository.Verify(
            x => x.GetBySkuCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "SKU uniqueness check should not be performed when SKU is unchanged");
    }

    [Fact]
    public async Task Handle_NullSkuCode_DoesNotCheckForDuplicates()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "ORIG-SKU",
            null);

        var command = new UpdateInventoryItemCommand(
            Id: itemId,
            Name: "Updated Name",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: null,
            CategoryId: null,
            IsActive: true);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockItemRepository.Verify(
            x => x.GetBySkuCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "SKU uniqueness check should not be performed when SKU is null");
    }

    [Fact]
    public async Task Handle_WhitespaceSkuCode_DoesNotCheckForDuplicates()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "ORIG-SKU",
            null);

        var command = new UpdateInventoryItemCommand(
            Id: itemId,
            Name: "Updated Name",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "   ",
            CategoryId: null,
            IsActive: true);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockItemRepository.Verify(
            x => x.GetBySkuCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "SKU uniqueness check should not be performed when SKU is whitespace");
    }
}

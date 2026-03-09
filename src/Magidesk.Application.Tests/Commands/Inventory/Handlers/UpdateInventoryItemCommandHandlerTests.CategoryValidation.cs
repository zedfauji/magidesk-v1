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
/// Category validation and change tests for UpdateInventoryItemCommandHandler.
/// </summary>
public partial class UpdateInventoryItemCommandHandlerTests
{
    [Fact]
    public async Task Handle_CategoryChanged_UpdatesCategorySuccessfully()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var newCategoryId = Guid.NewGuid();
        
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "TEST-001",
            null);

        var newCategory = InventoryCategory.Create("New Category", 1, null);

        var command = new UpdateInventoryItemCommand(
            Id: itemId,
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "TEST-001",
            CategoryId: newCategoryId,
            IsActive: true);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(newCategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newCategory);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingItem.CategoryId.Should().Be(newCategoryId);

        _mockCategoryRepository.Verify(
            x => x.GetByIdAsync(newCategoryId, It.IsAny<CancellationToken>()), 
            Times.Once,
            "Category should be validated when changed");

        _mockItemRepository.Verify(
            x => x.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var nonExistentCategoryId = Guid.NewGuid();
        
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
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "TEST-001",
            CategoryId: nonExistentCategoryId,
            IsActive: true);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(nonExistentCategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryCategory?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Category not found or inactive");

        _mockItemRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Item should not be updated when category does not exist");
    }

    [Fact]
    public async Task Handle_InactiveCategory_ThrowsInvalidOperationException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var inactiveCategoryId = Guid.NewGuid();
        
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "TEST-001",
            null);

        var inactiveCategory = InventoryCategory.Create("Inactive Category", 1, null);
        inactiveCategory.Deactivate();

        var command = new UpdateInventoryItemCommand(
            Id: itemId,
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "TEST-001",
            CategoryId: inactiveCategoryId,
            IsActive: true);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(inactiveCategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inactiveCategory);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Category not found or inactive");

        _mockItemRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Item should not be updated when category is inactive");
    }

    [Fact]
    public async Task Handle_CategoryUnchanged_DoesNotValidateCategory()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "TEST-001",
            categoryId);

        var command = new UpdateInventoryItemCommand(
            Id: itemId,
            Name: "Updated Name",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "TEST-001",
            CategoryId: categoryId,
            IsActive: true);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingItem.Name.Should().Be("Updated Name");
        
        _mockCategoryRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Category should not be validated when unchanged");
    }

    [Fact]
    public async Task Handle_CategoryClearedToNull_RemovesCategoryAssignment()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var originalCategoryId = Guid.NewGuid();
        
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "TEST-001",
            originalCategoryId);

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
        existingItem.CategoryId.Should().BeNull();

        _mockItemRepository.Verify(
            x => x.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}

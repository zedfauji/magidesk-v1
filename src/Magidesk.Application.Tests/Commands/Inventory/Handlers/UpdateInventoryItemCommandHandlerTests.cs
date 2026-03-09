using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.Commands.Inventory;
using Magidesk.Application.Commands.Inventory.Handlers;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Commands.Inventory.Handlers;

/// <summary>
/// Unit tests for UpdateInventoryItemCommandHandler.
/// Tests item updates, validation, stock adjustments, and category changes.
/// Split into partial classes to maintain file size limits.
/// </summary>
public partial class UpdateInventoryItemCommandHandlerTests
{
    private readonly Mock<IInventoryItemRepository> _mockItemRepository;
    private readonly Mock<IInventoryCategoryRepository> _mockCategoryRepository;
    private readonly Mock<IRepository<InventoryAdjustment>> _mockAdjustmentRepository;
    private readonly Mock<IUserContextService> _mockUserContextService;
    private readonly UpdateInventoryItemCommandHandler _handler;

    public UpdateInventoryItemCommandHandlerTests()
    {
        _mockItemRepository = new Mock<IInventoryItemRepository>();
        _mockCategoryRepository = new Mock<IInventoryCategoryRepository>();
        _mockAdjustmentRepository = new Mock<IRepository<InventoryAdjustment>>();
        _mockUserContextService = new Mock<IUserContextService>();
        
        _handler = new UpdateInventoryItemCommandHandler(
            _mockItemRepository.Object,
            _mockCategoryRepository.Object,
            _mockAdjustmentRepository.Object,
            _mockUserContextService.Object);
    }

    [Fact]
    public async Task Handle_ValidItemUpdate_UpdatesItemSuccessfully()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = InventoryItem.Create(
            "Original Item",
            "kg",
            100m,
            20m,
            "ORIG-001",
            null);

        var command = new UpdateInventoryItemCommand(
            Id: itemId,
            Name: "Updated Item",
            Unit: "lbs",
            StockQuantity: 100m,
            ReorderPoint: 25m,
            SkuCode: "UPD-001",
            CategoryId: null,
            IsActive: true);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _mockItemRepository
            .Setup(x => x.GetBySkuCodeAsync(command.SkuCode!, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingItem.Name.Should().Be("Updated Item");
        existingItem.Unit.Should().Be("lbs");
        existingItem.ReorderPoint.Should().Be(25m);
        existingItem.SkuCode.Should().Be("UPD-001");
        existingItem.IsActive.Should().BeTrue();

        _mockItemRepository.Verify(
            x => x.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_ItemNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonExistentItemId = Guid.NewGuid();
        var command = new UpdateInventoryItemCommand(
            Id: nonExistentItemId,
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "TEST-001",
            CategoryId: null,
            IsActive: true);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(nonExistentItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Item not found");

        _mockItemRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Item should not be updated when it does not exist");
    }

    [Fact]
    public async Task Handle_MultiplePropertiesChanged_UpdatesAllProperties()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var newCategoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var existingItem = InventoryItem.Create(
            "Original Item",
            "kg",
            100m,
            20m,
            "ORIG-001",
            null);

        var newCategory = InventoryCategory.Create("New Category", 1, null);

        var command = new UpdateInventoryItemCommand(
            Id: itemId,
            Name: "Completely Updated Item",
            Unit: "lbs",
            StockQuantity: 200m,
            ReorderPoint: 50m,
            SkuCode: "NEW-001",
            CategoryId: newCategoryId,
            IsActive: true);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(newCategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newCategory);

        _mockItemRepository
            .Setup(x => x.GetBySkuCodeAsync(command.SkuCode!, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        _mockUserContextService
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingItem.Name.Should().Be("Completely Updated Item");
        existingItem.Unit.Should().Be("lbs");
        existingItem.StockQuantity.Should().Be(200m);
        existingItem.ReorderPoint.Should().Be(50m);
        existingItem.SkuCode.Should().Be("NEW-001");
        existingItem.CategoryId.Should().Be(newCategoryId);
        existingItem.IsActive.Should().BeTrue();

        _mockItemRepository.Verify(
            x => x.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), 
            Times.Once);

        _mockAdjustmentRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryAdjustment>(), It.IsAny<CancellationToken>()), 
            Times.Once,
            "Adjustment record should be created when stock quantity changes");
    }
}

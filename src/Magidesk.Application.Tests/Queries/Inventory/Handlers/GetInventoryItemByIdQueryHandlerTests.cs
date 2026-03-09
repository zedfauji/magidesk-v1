using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Queries.Inventory.Handlers;

/// <summary>
/// Unit tests for GetInventoryItemByIdQueryHandler.
/// Tests item retrieval, null handling, and DTO mapping correctness.
/// </summary>
public class GetInventoryItemByIdQueryHandlerTests
{
    private readonly Mock<IInventoryItemRepository> _mockItemRepository;
    private readonly Mock<IInventoryCategoryRepository> _mockCategoryRepository;
    private readonly GetInventoryItemByIdQueryHandler _handler;

    public GetInventoryItemByIdQueryHandlerTests()
    {
        _mockItemRepository = new Mock<IInventoryItemRepository>();
        _mockCategoryRepository = new Mock<IInventoryCategoryRepository>();
        
        _handler = new GetInventoryItemByIdQueryHandler(
            _mockItemRepository.Object,
            _mockCategoryRepository.Object);
    }

    [Fact]
    public async Task Handle_ItemFound_ReturnsDtoWithAllProperties()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "TEST-001",
            null);

        var query = new GetInventoryItemByIdQuery(itemId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(item.Id);
        result.Name.Should().Be("Test Item");
        result.Unit.Should().Be("kg");
        result.SkuCode.Should().Be("TEST-001");
        result.StockQuantity.Should().Be(100m);
        result.ReorderPoint.Should().Be(20m);
        result.CategoryId.Should().BeNull();
        result.CategoryName.Should().BeNull();
        result.IsActive.Should().BeTrue();
        result.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));

        _mockItemRepository.Verify(
            x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_ItemNotFound_ReturnsNull()
    {
        // Arrange
        var nonExistentItemId = Guid.NewGuid();
        var query = new GetInventoryItemByIdQuery(nonExistentItemId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(nonExistentItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull("Item does not exist in the repository");

        _mockItemRepository.Verify(
            x => x.GetByIdAsync(nonExistentItemId, It.IsAny<CancellationToken>()), 
            Times.Once);
        
        _mockCategoryRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Category should not be queried when item is not found");
    }

    [Fact]
    public async Task Handle_ItemWithCategory_ReturnsDtoWithCategoryName()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = InventoryCategory.Create("Beverages", 1, null);
        
        var item = InventoryItem.Create(
            "Coffee Beans",
            "kg",
            50m,
            10m,
            "COFFEE-001",
            categoryId);

        var query = new GetInventoryItemByIdQuery(itemId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.CategoryId.Should().Be(categoryId);
        result.CategoryName.Should().Be("Beverages");

        _mockCategoryRepository.Verify(
            x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), 
            Times.Once,
            "Category should be loaded when item has a category assigned");
    }

    [Fact]
    public async Task Handle_ItemWithNonExistentCategory_ReturnsDtoWithNullCategoryName()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        
        var item = InventoryItem.Create(
            "Orphaned Item",
            "kg",
            30m,
            5m,
            "ORPHAN-001",
            categoryId);

        var query = new GetInventoryItemByIdQuery(itemId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryCategory?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.CategoryId.Should().Be(categoryId);
        result.CategoryName.Should().BeNull("Category does not exist in repository");

        _mockCategoryRepository.Verify(
            x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_ItemWithoutCategory_DoesNotQueryCategoryRepository()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = InventoryItem.Create(
            "Uncategorized Item",
            "unit",
            75m,
            15m,
            "UNCAT-001",
            null);

        var query = new GetInventoryItemByIdQuery(itemId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.CategoryId.Should().BeNull();
        result.CategoryName.Should().BeNull();

        _mockCategoryRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Category repository should not be queried when item has no category");
    }

    [Fact]
    public async Task Handle_ItemWithNullSkuCode_ReturnsDtoWithNullSku()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = InventoryItem.Create(
            "No SKU Item",
            "liter",
            200m,
            40m,
            null,
            null);

        var query = new GetInventoryItemByIdQuery(itemId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.SkuCode.Should().BeNull();
    }

    [Fact]
    public async Task Handle_InactiveItem_ReturnsDtoWithIsActiveFalse()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = InventoryItem.Create(
            "Inactive Item",
            "kg",
            0m,
            0m,
            "INACTIVE-001",
            null);
        
        item.Deactivate();

        var query = new GetInventoryItemByIdQuery(itemId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_DtoMapping_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = InventoryCategory.Create("Test Category", 5, null);
        
        var item = InventoryItem.Create(
            "Complete Item",
            "piece",
            250m,
            50m,
            "COMPLETE-001",
            categoryId);

        var query = new GetInventoryItemByIdQuery(itemId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert - Verify complete DTO mapping
        result.Should().NotBeNull();
        result.Should().BeOfType<InventoryItemDto>();
        
        // Verify all properties are mapped correctly
        result!.Id.Should().Be(item.Id);
        result.Name.Should().Be(item.Name);
        result.Unit.Should().Be(item.Unit);
        result.SkuCode.Should().Be(item.SkuCode);
        result.StockQuantity.Should().Be(item.StockQuantity);
        result.ReorderPoint.Should().Be(item.ReorderPoint);
        result.CategoryId.Should().Be(item.CategoryId);
        result.CategoryName.Should().Be(category.Name);
        result.CreatedAt.Should().Be(item.CreatedAt);
        result.IsActive.Should().Be(item.IsActive);
    }

    [Fact]
    public async Task Handle_ZeroStockQuantity_ReturnsDtoWithZeroStock()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = InventoryItem.Create(
            "Zero Stock Item",
            "kg",
            0m,
            10m,
            "ZERO-001",
            null);

        var query = new GetInventoryItemByIdQuery(itemId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.StockQuantity.Should().Be(0m);
        result.ReorderPoint.Should().Be(10m);
    }
}

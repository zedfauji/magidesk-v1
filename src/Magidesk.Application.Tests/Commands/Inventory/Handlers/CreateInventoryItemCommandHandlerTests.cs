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
/// Unit tests for CreateInventoryItemCommandHandler.
/// Tests item creation, validation, and adjustment record creation.
/// </summary>
public class CreateInventoryItemCommandHandlerTests
{
    private readonly Mock<IInventoryItemRepository> _mockItemRepository;
    private readonly Mock<IInventoryCategoryRepository> _mockCategoryRepository;
    private readonly Mock<IRepository<InventoryAdjustment>> _mockAdjustmentRepository;
    private readonly Mock<IUserContextService> _mockUserContextService;
    private readonly CreateInventoryItemCommandHandler _handler;

    public CreateInventoryItemCommandHandlerTests()
    {
        _mockItemRepository = new Mock<IInventoryItemRepository>();
        _mockCategoryRepository = new Mock<IInventoryCategoryRepository>();
        _mockAdjustmentRepository = new Mock<IRepository<InventoryAdjustment>>();
        _mockUserContextService = new Mock<IUserContextService>();
        
        _handler = new CreateInventoryItemCommandHandler(
            _mockItemRepository.Object,
            _mockCategoryRepository.Object,
            _mockAdjustmentRepository.Object,
            _mockUserContextService.Object);
    }

    [Fact]
    public async Task Handle_ValidItemCreation_CreatesItemAndReturnsId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateInventoryItemCommand(
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "TEST-001",
            CategoryId: null);

        _mockUserContextService
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        InventoryItem? capturedItem = null;
        _mockItemRepository
            .Setup(x => x.AddAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()))
            .Callback<InventoryItem, CancellationToken>((item, _) => capturedItem = item)
            .Returns(Task.CompletedTask);

        _mockItemRepository
            .Setup(x => x.GetBySkuCodeAsync(command.SkuCode!, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        
        capturedItem.Should().NotBeNull();
        capturedItem!.Id.Should().Be(result);
        capturedItem.Name.Should().Be("Test Item");
        capturedItem.Unit.Should().Be("kg");
        capturedItem.StockQuantity.Should().Be(100m);
        capturedItem.ReorderPoint.Should().Be(20m);
        capturedItem.SkuCode.Should().Be("TEST-001");
        capturedItem.CategoryId.Should().BeNull();
        capturedItem.IsActive.Should().BeTrue();
        capturedItem.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));

        _mockItemRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), 
            Times.Once);
        
        _mockAdjustmentRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryAdjustment>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateSku_ThrowsInvalidOperationException()
    {
        // Arrange
        var existingItem = InventoryItem.Create(
            "Existing Item",
            "kg",
            50m,
            10m,
            "DUPLICATE-SKU",
            null);

        var command = new CreateInventoryItemCommand(
            Name: "New Item",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "DUPLICATE-SKU",
            CategoryId: null);

        _mockItemRepository
            .Setup(x => x.GetBySkuCodeAsync(command.SkuCode!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("SKU code already exists");

        _mockItemRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), 
            Times.Never, 
            "Item should not be added when SKU is duplicate");
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonExistentCategoryId = Guid.NewGuid();
        var command = new CreateInventoryItemCommand(
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "TEST-001",
            CategoryId: nonExistentCategoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(nonExistentCategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryCategory?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Category not found or inactive");

        _mockItemRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), 
            Times.Never, 
            "Item should not be added when category does not exist");
    }

    [Fact]
    public async Task Handle_InactiveCategory_ThrowsInvalidOperationException()
    {
        // Arrange
        var inactiveCategoryId = Guid.NewGuid();
        var inactiveCategory = InventoryCategory.Create("Inactive Category", 1, null);
        inactiveCategory.Deactivate();

        var command = new CreateInventoryItemCommand(
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "TEST-001",
            CategoryId: inactiveCategoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(inactiveCategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inactiveCategory);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Category not found or inactive");

        _mockItemRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), 
            Times.Never, 
            "Item should not be added when category is inactive");
    }

    [Fact]
    public async Task Handle_ZeroStockQuantity_DoesNotCreateAdjustmentRecord()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateInventoryItemCommand(
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 0m,
            ReorderPoint: 20m,
            SkuCode: "TEST-002",
            CategoryId: null);

        _mockUserContextService
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        _mockItemRepository
            .Setup(x => x.GetBySkuCodeAsync(command.SkuCode!, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        
        _mockItemRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), 
            Times.Once);
        
        _mockAdjustmentRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryAdjustment>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Adjustment record should not be created when stock quantity is zero");
    }

    [Fact]
    public async Task Handle_PositiveStockQuantity_CreatesAdjustmentRecordWithCorrectDetails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateInventoryItemCommand(
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 150m,
            ReorderPoint: 30m,
            SkuCode: "TEST-003",
            CategoryId: null);

        _mockUserContextService
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        _mockItemRepository
            .Setup(x => x.GetBySkuCodeAsync(command.SkuCode!, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        InventoryAdjustment? capturedAdjustment = null;
        _mockAdjustmentRepository
            .Setup(x => x.AddAsync(It.IsAny<InventoryAdjustment>(), It.IsAny<CancellationToken>()))
            .Callback<InventoryAdjustment, CancellationToken>((adj, _) => capturedAdjustment = adj)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedAdjustment.Should().NotBeNull();
        capturedAdjustment!.InventoryItemId.Should().Be(result);
        capturedAdjustment.QuantityDelta.Should().Be(150m);
        capturedAdjustment.Reason.Should().Be("Initial stock");
        capturedAdjustment.UserId.Should().Be(userId);
        capturedAdjustment.AdjustedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _mockAdjustmentRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryAdjustment>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyUserContext_CreatesAdjustmentWithNullUserId()
    {
        // Arrange
        var command = new CreateInventoryItemCommand(
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 50m,
            ReorderPoint: 10m,
            SkuCode: "TEST-004",
            CategoryId: null);

        _mockUserContextService
            .Setup(x => x.GetCurrentUserId())
            .Returns(Guid.Empty);

        _mockItemRepository
            .Setup(x => x.GetBySkuCodeAsync(command.SkuCode!, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        InventoryAdjustment? capturedAdjustment = null;
        _mockAdjustmentRepository
            .Setup(x => x.AddAsync(It.IsAny<InventoryAdjustment>(), It.IsAny<CancellationToken>()))
            .Callback<InventoryAdjustment, CancellationToken>((adj, _) => capturedAdjustment = adj)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        capturedAdjustment.Should().NotBeNull();
        capturedAdjustment!.UserId.Should().BeNull("UserId should be null when user context returns Guid.Empty");
    }

    [Fact]
    public async Task Handle_ValidCategoryId_CreatesItemWithCategory()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var activeCategory = InventoryCategory.Create("Active Category", 1, null);

        var command = new CreateInventoryItemCommand(
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "TEST-005",
            CategoryId: categoryId);

        _mockUserContextService
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeCategory);

        _mockItemRepository
            .Setup(x => x.GetBySkuCodeAsync(command.SkuCode!, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        InventoryItem? capturedItem = null;
        _mockItemRepository
            .Setup(x => x.AddAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()))
            .Callback<InventoryItem, CancellationToken>((item, _) => capturedItem = item)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        capturedItem.Should().NotBeNull();
        capturedItem!.CategoryId.Should().Be(categoryId);

        _mockCategoryRepository.Verify(
            x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), 
            Times.Once,
            "Category should be validated before item creation");
    }

    [Fact]
    public async Task Handle_NullSkuCode_DoesNotCheckForDuplicates()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateInventoryItemCommand(
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: null,
            CategoryId: null);

        _mockUserContextService
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        
        _mockItemRepository.Verify(
            x => x.GetBySkuCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "SKU uniqueness check should not be performed when SKU is null");
    }

    [Fact]
    public async Task Handle_WhitespaceSkuCode_DoesNotCheckForDuplicates()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateInventoryItemCommand(
            Name: "Test Item",
            Unit: "kg",
            StockQuantity: 100m,
            ReorderPoint: 20m,
            SkuCode: "   ",
            CategoryId: null);

        _mockUserContextService
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        
        _mockItemRepository.Verify(
            x => x.GetBySkuCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "SKU uniqueness check should not be performed when SKU is whitespace");
    }
}

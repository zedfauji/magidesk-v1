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
/// Unit tests for DeleteCategoryCommandHandler.
/// Tests category soft deletion, validation of assigned items, and child category constraints.
/// </summary>
public class DeleteCategoryCommandHandlerTests
{
    private readonly Mock<IInventoryCategoryRepository> _mockCategoryRepository;
    private readonly Mock<IInventoryItemRepository> _mockItemRepository;
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _mockCategoryRepository = new Mock<IInventoryCategoryRepository>();
        _mockItemRepository = new Mock<IInventoryItemRepository>();
        _handler = new DeleteCategoryCommandHandler(
            _mockCategoryRepository.Object,
            _mockItemRepository.Object);
    }

    #region Valid Category Deletion Tests (5.2.3.1)

    [Fact]
    public async Task Handle_ValidCategoryDeletion_DeactivatesCategorySuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Category to Delete", 1, null);

        var command = new DeleteCategoryCommand(categoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockItemRepository
            .Setup(x => x.CountActiveItemsByCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _mockCategoryRepository
            .Setup(x => x.CountActiveChildCategoriesAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingCategory.IsActive.Should().BeFalse("category should be soft deleted");

        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()),
            Times.Once,
            "category should be updated with IsActive = false");
    }

    [Fact]
    public async Task Handle_ValidDeletion_PerformsAllValidationChecks()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Valid Category", 1, null);

        var command = new DeleteCategoryCommand(categoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockItemRepository
            .Setup(x => x.CountActiveItemsByCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _mockCategoryRepository
            .Setup(x => x.CountActiveChildCategoriesAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockCategoryRepository.Verify(
            x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Once,
            "category existence should be validated");

        _mockItemRepository.Verify(
            x => x.CountActiveItemsByCategoryAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Once,
            "assigned items should be checked");

        _mockCategoryRepository.Verify(
            x => x.CountActiveChildCategoriesAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Once,
            "child categories should be checked");

        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()),
            Times.Once,
            "category should be updated");
    }

    [Fact]
    public async Task Handle_CategoryWithNoConstraints_DeletesSuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Empty Category", 5, null);

        var command = new DeleteCategoryCommand(categoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockItemRepository
            .Setup(x => x.CountActiveItemsByCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _mockCategoryRepository
            .Setup(x => x.CountActiveChildCategoriesAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingCategory.IsActive.Should().BeFalse();
        
        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(It.Is<InventoryCategory>(c => !c.IsActive), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region Category Not Found Tests (5.2.3.2)

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(nonExistentId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryCategory?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Category not found");

        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "update should not be called when category does not exist");

        _mockItemRepository.Verify(
            x => x.CountActiveItemsByCategoryAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "item count check should not be performed when category does not exist");
    }

    [Fact]
    public async Task Handle_NullCategoryReturned_ThrowsInvalidOperationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryCategory?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Category not found");
    }

    #endregion

    #region Rejection When Category Has Items Tests (5.2.3.3)

    [Fact]
    public async Task Handle_CategoryHasOneActiveItem_ThrowsInvalidOperationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Category with Items", 1, null);

        var command = new DeleteCategoryCommand(categoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockItemRepository
            .Setup(x => x.CountActiveItemsByCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot delete category with assigned items");

        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "category should not be deleted when it has assigned items");

        _mockCategoryRepository.Verify(
            x => x.CountActiveChildCategoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "child category check should not be performed when item check fails");
    }

    [Fact]
    public async Task Handle_CategoryHasMultipleActiveItems_ThrowsInvalidOperationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Popular Category", 1, null);

        var command = new DeleteCategoryCommand(categoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockItemRepository
            .Setup(x => x.CountActiveItemsByCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(25);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot delete category with assigned items");

        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_CategoryHasNoActiveItems_ProceedsToChildCheck()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Empty Category", 1, null);

        var command = new DeleteCategoryCommand(categoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockItemRepository
            .Setup(x => x.CountActiveItemsByCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _mockCategoryRepository
            .Setup(x => x.CountActiveChildCategoriesAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockCategoryRepository.Verify(
            x => x.CountActiveChildCategoriesAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Once,
            "child category check should be performed when no items are assigned");
    }

    #endregion

    #region Rejection When Category Has Children Tests (5.2.3.4)

    [Fact]
    public async Task Handle_CategoryHasOneChildCategory_ThrowsInvalidOperationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Parent Category", 1, null);

        var command = new DeleteCategoryCommand(categoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockItemRepository
            .Setup(x => x.CountActiveItemsByCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _mockCategoryRepository
            .Setup(x => x.CountActiveChildCategoriesAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot delete category with child categories");

        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "category should not be deleted when it has child categories");
    }

    [Fact]
    public async Task Handle_CategoryHasMultipleChildCategories_ThrowsInvalidOperationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Root Category", 1, null);

        var command = new DeleteCategoryCommand(categoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockItemRepository
            .Setup(x => x.CountActiveItemsByCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _mockCategoryRepository
            .Setup(x => x.CountActiveChildCategoriesAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot delete category with child categories");

        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_CategoryHasNoChildren_DeletesSuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Leaf Category", 1, null);

        var command = new DeleteCategoryCommand(categoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockItemRepository
            .Setup(x => x.CountActiveItemsByCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _mockCategoryRepository
            .Setup(x => x.CountActiveChildCategoriesAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingCategory.IsActive.Should().BeFalse();
        
        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_CategoryHasItemsAndChildren_ThrowsForItemsFirst()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Busy Category", 1, null);

        var command = new DeleteCategoryCommand(categoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockItemRepository
            .Setup(x => x.CountActiveItemsByCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        _mockCategoryRepository
            .Setup(x => x.CountActiveChildCategoriesAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot delete category with assigned items");

        _mockCategoryRepository.Verify(
            x => x.CountActiveChildCategoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "child check should not be performed when item check fails");
    }

    #endregion

    #region IsActive Set to False Tests (5.2.3.5)

    [Fact]
    public async Task Handle_SuccessfulDeletion_SetsIsActiveToFalse()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Category", 1, null);

        var command = new DeleteCategoryCommand(categoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockItemRepository
            .Setup(x => x.CountActiveItemsByCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _mockCategoryRepository
            .Setup(x => x.CountActiveChildCategoriesAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Verify category starts as active
        existingCategory.IsActive.Should().BeTrue("category should be active before deletion");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingCategory.IsActive.Should().BeFalse("category should be deactivated after deletion");
    }

    [Fact]
    public async Task Handle_SoftDelete_DoesNotPhysicallyDeleteCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Category", 1, null);

        var command = new DeleteCategoryCommand(categoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockItemRepository
            .Setup(x => x.CountActiveItemsByCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _mockCategoryRepository
            .Setup(x => x.CountActiveChildCategoriesAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()),
            Times.Once,
            "category should be updated, not physically deleted");
    }

    [Fact]
    public async Task Handle_DeactivatedCategory_PreservesOtherProperties()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Category Name", 10, parentId);

        var command = new DeleteCategoryCommand(categoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockItemRepository
            .Setup(x => x.CountActiveItemsByCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _mockCategoryRepository
            .Setup(x => x.CountActiveChildCategoriesAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingCategory.IsActive.Should().BeFalse();
        existingCategory.Name.Should().Be("Category Name", "name should be preserved");
        existingCategory.SortOrder.Should().Be(10, "sort order should be preserved");
        existingCategory.ParentCategoryId.Should().Be(parentId, "parent ID should be preserved");
    }

    [Fact]
    public async Task Handle_ValidDeletion_CallsDeactivateMethod()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Category", 1, null);

        var command = new DeleteCategoryCommand(categoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockItemRepository
            .Setup(x => x.CountActiveItemsByCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _mockCategoryRepository
            .Setup(x => x.CountActiveChildCategoriesAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var wasActive = existingCategory.IsActive;

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        wasActive.Should().BeTrue("category should start as active");
        existingCategory.IsActive.Should().BeFalse("Deactivate() method should set IsActive to false");
    }

    #endregion
}

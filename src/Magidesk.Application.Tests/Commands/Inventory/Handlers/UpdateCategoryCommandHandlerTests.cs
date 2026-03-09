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
/// Unit tests for UpdateCategoryCommandHandler.
/// Tests category updates, name uniqueness validation, parent changes, and circular reference detection.
/// </summary>
public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<IInventoryCategoryRepository> _mockCategoryRepository;
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        _mockCategoryRepository = new Mock<IInventoryCategoryRepository>();
        _handler = new UpdateCategoryCommandHandler(_mockCategoryRepository.Object);
    }

    #region Valid Category Update Tests (5.2.2.1)

    [Fact]
    public async Task Handle_ValidCategoryUpdate_UpdatesCategorySuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Old Name", 1, null);

        var command = new UpdateCategoryCommand(
            Id: categoryId,
            Name: "New Name",
            SortOrder: 5,
            ParentCategoryId: null);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockCategoryRepository
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryCategory?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingCategory.Name.Should().Be("New Name");
        existingCategory.SortOrder.Should().Be(5);
        existingCategory.ParentCategoryId.Should().BeNull();

        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateNameOnly_UpdatesNameWithoutChangingOtherProperties()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Old Name", 10, null);

        var command = new UpdateCategoryCommand(
            Id: categoryId,
            Name: "Updated Name",
            SortOrder: 10,
            ParentCategoryId: null);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockCategoryRepository
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryCategory?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingCategory.Name.Should().Be("Updated Name");
        existingCategory.SortOrder.Should().Be(10);
        
        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateSortOrderOnly_UpdatesSortOrderSuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Category Name", 1, null);

        var command = new UpdateCategoryCommand(
            Id: categoryId,
            Name: "Category Name",
            SortOrder: 99,
            ParentCategoryId: null);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingCategory.SortOrder.Should().Be(99);
        existingCategory.Name.Should().Be("Category Name");
        
        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region Category Not Found Tests (5.2.2.2)

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateCategoryCommand(
            Id: nonExistentId,
            Name: "New Name",
            SortOrder: 1,
            ParentCategoryId: null);

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
            "Update should not be called when category does not exist");
    }

    #endregion

    #region Duplicate Name Rejection Tests (5.2.2.3)

    [Fact]
    public async Task Handle_DuplicateNameExcludingSelf_ThrowsInvalidOperationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var otherCategoryId = Guid.NewGuid();
        
        var existingCategory = InventoryCategory.Create("Old Name", 1, null);
        var otherCategory = InventoryCategory.Create("Duplicate Name", 2, null);

        var command = new UpdateCategoryCommand(
            Id: categoryId,
            Name: "Duplicate Name",
            SortOrder: 1,
            ParentCategoryId: null);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockCategoryRepository
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherCategory);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Category name already exists");

        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "Update should not be called when name is duplicate");
    }

    [Fact]
    public async Task Handle_SameNameAsSelf_UpdatesSuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Same Name", 1, null);

        var command = new UpdateCategoryCommand(
            Id: categoryId,
            Name: "Same Name",
            SortOrder: 5,
            ParentCategoryId: null);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingCategory.SortOrder.Should().Be(5);
        
        _mockCategoryRepository.Verify(
            x => x.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "Name uniqueness check should be skipped when name is unchanged");
        
        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateInactiveCategoryName_UpdatesSuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Old Name", 1, null);
        
        var inactiveCategory = InventoryCategory.Create("Inactive Name", 2, null);
        inactiveCategory.Deactivate();

        var command = new UpdateCategoryCommand(
            Id: categoryId,
            Name: "Inactive Name",
            SortOrder: 1,
            ParentCategoryId: null);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockCategoryRepository
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inactiveCategory);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingCategory.Name.Should().Be("Inactive Name");
        
        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()),
            Times.Once,
            "Update should succeed when duplicate name belongs to inactive category");
    }

    #endregion

    #region Circular Reference Detection Tests (5.2.2.4)

    [Fact]
    public async Task Handle_SelfAsParent_ThrowsInvalidOperationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = InventoryCategory.Create("Category", 1, null);

        var command = new UpdateCategoryCommand(
            Id: categoryId,
            Name: "Category",
            SortOrder: 1,
            ParentCategoryId: categoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Category cannot be its own parent");

        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_DirectCircularReference_ThrowsInvalidOperationException()
    {
        // Arrange
        // Category A wants to set Category B as parent, but B already has A as parent
        var categoryAId = Guid.NewGuid();
        var categoryBId = Guid.NewGuid();
        
        var categoryA = InventoryCategory.Create("Category A", 1, null);
        var categoryB = InventoryCategory.Create("Category B", 2, categoryAId);

        var command = new UpdateCategoryCommand(
            Id: categoryAId,
            Name: "Category A",
            SortOrder: 1,
            ParentCategoryId: categoryBId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryAId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryA);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryBId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryB);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Circular category reference detected");

        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_IndirectCircularReference_ThrowsInvalidOperationException()
    {
        // Arrange
        // Category A -> Category B -> Category C
        // Now trying to set C as parent of A (would create A -> C -> B -> A)
        var categoryAId = Guid.NewGuid();
        var categoryBId = Guid.NewGuid();
        var categoryCId = Guid.NewGuid();
        
        var categoryA = InventoryCategory.Create("Category A", 1, null);
        var categoryB = InventoryCategory.Create("Category B", 2, categoryAId);
        var categoryC = InventoryCategory.Create("Category C", 3, categoryBId);

        var command = new UpdateCategoryCommand(
            Id: categoryAId,
            Name: "Category A",
            SortOrder: 1,
            ParentCategoryId: categoryCId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryAId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryA);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryCId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryC);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryBId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryB);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Circular category reference detected");

        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_DeepCircularReference_ThrowsInvalidOperationException()
    {
        // Arrange
        // Category A -> B -> C -> D -> E
        // Now trying to set E as parent of A (would create cycle)
        var categoryAId = Guid.NewGuid();
        var categoryBId = Guid.NewGuid();
        var categoryCId = Guid.NewGuid();
        var categoryDId = Guid.NewGuid();
        var categoryEId = Guid.NewGuid();
        
        var categoryA = InventoryCategory.Create("Category A", 1, null);
        var categoryB = InventoryCategory.Create("Category B", 2, categoryAId);
        var categoryC = InventoryCategory.Create("Category C", 3, categoryBId);
        var categoryD = InventoryCategory.Create("Category D", 4, categoryCId);
        var categoryE = InventoryCategory.Create("Category E", 5, categoryDId);

        var command = new UpdateCategoryCommand(
            Id: categoryAId,
            Name: "Category A",
            SortOrder: 1,
            ParentCategoryId: categoryEId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryAId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryA);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryEId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryE);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryDId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryD);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryCId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryC);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryBId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryB);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Circular category reference detected");

        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_NoCircularReference_UpdatesSuccessfully()
    {
        // Arrange
        // Category A (no parent), Category B (no parent)
        // Setting B as parent of A is valid
        var categoryAId = Guid.NewGuid();
        var categoryBId = Guid.NewGuid();
        
        var categoryA = InventoryCategory.Create("Category A", 1, null);
        var categoryB = InventoryCategory.Create("Category B", 2, null);

        var command = new UpdateCategoryCommand(
            Id: categoryAId,
            Name: "Category A",
            SortOrder: 1,
            ParentCategoryId: categoryBId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryAId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryA);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryBId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryB);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        categoryA.ParentCategoryId.Should().Be(categoryBId);
        
        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(categoryA, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region Parent Change Tests (5.2.2.5)

    [Fact]
    public async Task Handle_AddParentToRootCategory_UpdatesSuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        
        var existingCategory = InventoryCategory.Create("Child Category", 1, null);
        var parentCategory = InventoryCategory.Create("Parent Category", 2, null);

        var command = new UpdateCategoryCommand(
            Id: categoryId,
            Name: "Child Category",
            SortOrder: 1,
            ParentCategoryId: parentId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(parentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parentCategory);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingCategory.ParentCategoryId.Should().Be(parentId);
        
        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_RemoveParentFromCategory_UpdatesSuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var oldParentId = Guid.NewGuid();
        
        var existingCategory = InventoryCategory.Create("Category", 1, oldParentId);

        var command = new UpdateCategoryCommand(
            Id: categoryId,
            Name: "Category",
            SortOrder: 1,
            ParentCategoryId: null);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingCategory.ParentCategoryId.Should().BeNull();
        
        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ChangeParentToAnotherCategory_UpdatesSuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var oldParentId = Guid.NewGuid();
        var newParentId = Guid.NewGuid();
        
        var existingCategory = InventoryCategory.Create("Category", 1, oldParentId);
        var newParentCategory = InventoryCategory.Create("New Parent", 2, null);

        var command = new UpdateCategoryCommand(
            Id: categoryId,
            Name: "Category",
            SortOrder: 1,
            ParentCategoryId: newParentId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(newParentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newParentCategory);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingCategory.ParentCategoryId.Should().Be(newParentId);
        
        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(existingCategory, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ParentNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var nonExistentParentId = Guid.NewGuid();
        
        var existingCategory = InventoryCategory.Create("Category", 1, null);

        var command = new UpdateCategoryCommand(
            Id: categoryId,
            Name: "Category",
            SortOrder: 1,
            ParentCategoryId: nonExistentParentId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(nonExistentParentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryCategory?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Parent category not found or inactive");

        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_InactiveParent_ThrowsInvalidOperationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var inactiveParentId = Guid.NewGuid();
        
        var existingCategory = InventoryCategory.Create("Category", 1, null);
        var inactiveParent = InventoryCategory.Create("Inactive Parent", 2, null);
        inactiveParent.Deactivate();

        var command = new UpdateCategoryCommand(
            Id: categoryId,
            Name: "Category",
            SortOrder: 1,
            ParentCategoryId: inactiveParentId);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(inactiveParentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inactiveParent);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Parent category not found or inactive");

        _mockCategoryRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    #endregion
}

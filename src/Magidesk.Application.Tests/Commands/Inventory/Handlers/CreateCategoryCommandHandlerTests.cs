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
/// Unit tests for CreateCategoryCommandHandler.
/// Tests category creation, name uniqueness validation, and parent category validation.
/// </summary>
public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<IInventoryCategoryRepository> _mockCategoryRepository;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _mockCategoryRepository = new Mock<IInventoryCategoryRepository>();
        _handler = new CreateCategoryCommandHandler(_mockCategoryRepository.Object);
    }

    #region Valid Category Creation Tests (5.2.1.1)

    [Fact]
    public async Task Handle_ValidCategoryCreation_CreatesCategoryAndReturnsId()
    {
        // Arrange
        var command = new CreateCategoryCommand(
            Name: "Beverages",
            SortOrder: 1,
            ParentCategoryId: null);

        _mockCategoryRepository
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryCategory?)null);

        InventoryCategory? capturedCategory = null;
        _mockCategoryRepository
            .Setup(x => x.AddAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()))
            .Callback<InventoryCategory, CancellationToken>((cat, _) => capturedCategory = cat)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        
        capturedCategory.Should().NotBeNull();
        capturedCategory!.Id.Should().Be(result);
        capturedCategory.Name.Should().Be("Beverages");
        capturedCategory.SortOrder.Should().Be(1);
        capturedCategory.ParentCategoryId.Should().BeNull();
        capturedCategory.IsActive.Should().BeTrue();

        _mockCategoryRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCategoryWithParent_CreatesCategoryWithParentLink()
    {
        // Arrange
        var parentCategoryId = Guid.NewGuid();
        var parentCategory = InventoryCategory.Create("Food", 1, null);

        var command = new CreateCategoryCommand(
            Name: "Frozen Food",
            SortOrder: 2,
            ParentCategoryId: parentCategoryId);

        _mockCategoryRepository
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryCategory?)null);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(parentCategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parentCategory);

        InventoryCategory? capturedCategory = null;
        _mockCategoryRepository
            .Setup(x => x.AddAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()))
            .Callback<InventoryCategory, CancellationToken>((cat, _) => capturedCategory = cat)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        
        capturedCategory.Should().NotBeNull();
        capturedCategory!.ParentCategoryId.Should().Be(parentCategoryId);
        capturedCategory.Name.Should().Be("Frozen Food");
        capturedCategory.SortOrder.Should().Be(2);

        _mockCategoryRepository.Verify(
            x => x.GetByIdAsync(parentCategoryId, It.IsAny<CancellationToken>()), 
            Times.Once,
            "Parent category should be validated before creation");
    }

    [Fact]
    public async Task Handle_ValidCategoryWithZeroSortOrder_CreatesSuccessfully()
    {
        // Arrange
        var command = new CreateCategoryCommand(
            Name: "Uncategorized",
            SortOrder: 0,
            ParentCategoryId: null);

        _mockCategoryRepository
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryCategory?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        
        _mockCategoryRepository.Verify(
            x => x.AddAsync(It.Is<InventoryCategory>(c => c.SortOrder == 0), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    #endregion

    #region Duplicate Name Rejection Tests (5.2.1.2)

    [Fact]
    public async Task Handle_DuplicateActiveCategoryName_ThrowsInvalidOperationException()
    {
        // Arrange
        var existingCategory = InventoryCategory.Create("Beverages", 1, null);

        var command = new CreateCategoryCommand(
            Name: "Beverages",
            SortOrder: 2,
            ParentCategoryId: null);

        _mockCategoryRepository
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Category name already exists");

        _mockCategoryRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Category should not be added when name is duplicate");
    }

    [Fact]
    public async Task Handle_DuplicateInactiveCategoryName_CreatesSuccessfully()
    {
        // Arrange
        var inactiveCategory = InventoryCategory.Create("Old Beverages", 1, null);
        inactiveCategory.Deactivate();

        var command = new CreateCategoryCommand(
            Name: "Old Beverages",
            SortOrder: 2,
            ParentCategoryId: null);

        _mockCategoryRepository
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inactiveCategory);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        
        _mockCategoryRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()), 
            Times.Once,
            "Category should be created when existing category with same name is inactive");
    }

    [Fact]
    public async Task Handle_CaseVariationOfExistingName_ThrowsIfRepositoryReturnsMatch()
    {
        // Arrange
        var existingCategory = InventoryCategory.Create("Beverages", 1, null);

        var command = new CreateCategoryCommand(
            Name: "BEVERAGES",
            SortOrder: 2,
            ParentCategoryId: null);

        // Assuming repository performs case-insensitive search
        _mockCategoryRepository
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Category name already exists");
    }

    #endregion

    #region Invalid Parent Rejection Tests (5.2.1.3)

    [Fact]
    public async Task Handle_ParentCategoryNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonExistentParentId = Guid.NewGuid();
        var command = new CreateCategoryCommand(
            Name: "Subcategory",
            SortOrder: 1,
            ParentCategoryId: nonExistentParentId);

        _mockCategoryRepository
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryCategory?)null);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(nonExistentParentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryCategory?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Parent category not found or inactive");

        _mockCategoryRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Category should not be added when parent does not exist");
    }

    [Fact]
    public async Task Handle_InactiveParentCategory_ThrowsInvalidOperationException()
    {
        // Arrange
        var inactiveParentId = Guid.NewGuid();
        var inactiveParent = InventoryCategory.Create("Inactive Parent", 1, null);
        inactiveParent.Deactivate();

        var command = new CreateCategoryCommand(
            Name: "Subcategory",
            SortOrder: 1,
            ParentCategoryId: inactiveParentId);

        _mockCategoryRepository
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryCategory?)null);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(inactiveParentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inactiveParent);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Parent category not found or inactive");

        _mockCategoryRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Category should not be added when parent is inactive");
    }

    #endregion

    #region Validation Failures Tests (5.2.1.4)

    [Fact]
    public async Task Handle_NullParentCategoryId_DoesNotValidateParent()
    {
        // Arrange
        var command = new CreateCategoryCommand(
            Name: "Root Category",
            SortOrder: 1,
            ParentCategoryId: null);

        _mockCategoryRepository
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryCategory?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        
        _mockCategoryRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Parent validation should not be performed when ParentCategoryId is null");
    }

    [Fact]
    public async Task Handle_NameUniquenessCheckPerformed_BeforeParentValidation()
    {
        // Arrange
        var existingCategory = InventoryCategory.Create("Duplicate", 1, null);
        var parentId = Guid.NewGuid();

        var command = new CreateCategoryCommand(
            Name: "Duplicate",
            SortOrder: 2,
            ParentCategoryId: parentId);

        _mockCategoryRepository
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Category name already exists");

        _mockCategoryRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Parent validation should not be performed when name is duplicate");
    }

    [Fact]
    public async Task Handle_AllValidationsPass_CreatesCategory()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var parentCategory = InventoryCategory.Create("Parent", 1, null);

        var command = new CreateCategoryCommand(
            Name: "Valid Subcategory",
            SortOrder: 5,
            ParentCategoryId: parentId);

        _mockCategoryRepository
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryCategory?)null);

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(parentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parentCategory);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        
        _mockCategoryRepository.Verify(
            x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()), 
            Times.Once,
            "Name uniqueness should be validated");
        
        _mockCategoryRepository.Verify(
            x => x.GetByIdAsync(parentId, It.IsAny<CancellationToken>()), 
            Times.Once,
            "Parent category should be validated");
        
        _mockCategoryRepository.Verify(
            x => x.AddAsync(It.IsAny<InventoryCategory>(), It.IsAny<CancellationToken>()), 
            Times.Once,
            "Category should be created when all validations pass");
    }

    #endregion
}

using FluentAssertions;
using Magidesk.Domain.Entities;
using Xunit;

namespace Magidesk.Domain.Tests.Entities;

public class InventoryCategoryTests
{
    [Fact]
    public void Create_ValidName_ReturnsActiveCategory()
    {
        // Arrange
        var name = "Beverages";
        var sortOrder = 1;

        // Act
        var category = InventoryCategory.Create(name, sortOrder);

        // Assert
        category.Should().NotBeNull();
        category.Id.Should().NotBeEmpty();
        category.Name.Should().Be(name);
        category.SortOrder.Should().Be(sortOrder);
        category.IsActive.Should().BeTrue();
        category.ParentCategoryId.Should().BeNull();
    }

    [Fact]
    public void Create_NullName_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => InventoryCategory.Create(null!, 1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Name cannot be null or whitespace*");
    }

    [Fact]
    public void Create_EmptyName_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => InventoryCategory.Create("", 1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Name cannot be null or whitespace*");
    }

    [Fact]
    public void Create_WhitespaceName_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => InventoryCategory.Create("   ", 1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Name cannot be null or whitespace*");
    }

    [Fact]
    public void Create_WithParentCategoryId_SetsParentCategoryId()
    {
        // Arrange
        var name = "Cold Beverages";
        var sortOrder = 2;
        var parentId = Guid.NewGuid();

        // Act
        var category = InventoryCategory.Create(name, sortOrder, parentId);

        // Assert
        category.ParentCategoryId.Should().Be(parentId);
    }

    [Fact]
    public void UpdateName_ValidName_UpdatesName()
    {
        // Arrange
        var category = InventoryCategory.Create("Old Name", 1);
        var newName = "New Name";

        // Act
        category.UpdateName(newName);

        // Assert
        category.Name.Should().Be(newName);
    }

    [Fact]
    public void UpdateName_WhitespaceName_ThrowsArgumentException()
    {
        // Arrange
        var category = InventoryCategory.Create("Test Category", 1);

        // Act
        var act = () => category.UpdateName("   ");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Name cannot be null or whitespace*");
    }

    [Fact]
    public void Deactivate_ActiveCategory_SetsIsActiveFalse()
    {
        // Arrange
        var category = InventoryCategory.Create("Test Category", 1);
        category.IsActive.Should().BeTrue();

        // Act
        category.Deactivate();

        // Assert
        category.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_InactiveCategory_SetsIsActiveTrue()
    {
        // Arrange
        var category = InventoryCategory.Create("Test Category", 1);
        category.Deactivate();
        category.IsActive.Should().BeFalse();

        // Act
        category.Activate();

        // Assert
        category.IsActive.Should().BeTrue();
    }
}

using FluentAssertions;
using Magidesk.Domain.Entities;
using Xunit;

namespace Magidesk.Domain.Tests.Entities;

public class InventoryItemExtendedTests
{
    [Fact]
    public void Create_WithSkuCode_SetsSkuCode()
    {
        // Arrange
        var name = "Coffee Beans";
        var unit = "kg";
        var stockQuantity = 10m;
        var reorderPoint = 5m;
        var skuCode = "SKU-COFFEE-001";

        // Act
        var item = InventoryItem.Create(name, unit, stockQuantity, reorderPoint, skuCode);

        // Assert
        item.SkuCode.Should().Be(skuCode);
    }

    [Fact]
    public void Create_WithCategoryId_SetsCategoryId()
    {
        // Arrange
        var name = "Tea Leaves";
        var unit = "kg";
        var stockQuantity = 8m;
        var reorderPoint = 3m;
        var categoryId = Guid.NewGuid();

        // Act
        var item = InventoryItem.Create(name, unit, stockQuantity, reorderPoint, null, categoryId);

        // Assert
        item.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void Create_NoSkuOrCategory_HasNullSkuAndNullCategoryId()
    {
        // Arrange
        var name = "Sugar";
        var unit = "kg";
        var stockQuantity = 20m;
        var reorderPoint = 10m;

        // Act
        var item = InventoryItem.Create(name, unit, stockQuantity, reorderPoint);

        // Assert
        item.SkuCode.Should().BeNull();
        item.CategoryId.Should().BeNull();
    }

    [Fact]
    public void Create_SetsCreatedAt_ToUtcNow()
    {
        // Arrange
        var name = "Milk";
        var unit = "liters";
        var stockQuantity = 15m;
        var reorderPoint = 5m;
        var beforeCreation = DateTimeOffset.UtcNow;

        // Act
        var item = InventoryItem.Create(name, unit, stockQuantity, reorderPoint);
        var afterCreation = DateTimeOffset.UtcNow.AddSeconds(1);

        // Assert
        item.CreatedAt.Should().BeOnOrAfter(beforeCreation);
        item.CreatedAt.Should().BeBefore(afterCreation);
    }

    [Fact]
    public void AssignCategory_SetsCategoryId()
    {
        // Arrange
        var item = InventoryItem.Create("Flour", "kg", 50m, 20m);
        var categoryId = Guid.NewGuid();
        item.CategoryId.Should().BeNull();

        // Act
        item.AssignCategory(categoryId);

        // Assert
        item.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void ClearCategory_SetsCategoryIdToNull()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var item = InventoryItem.Create("Rice", "kg", 100m, 30m, null, categoryId);
        item.CategoryId.Should().Be(categoryId);

        // Act
        item.ClearCategory();

        // Assert
        item.CategoryId.Should().BeNull();
    }

    [Fact]
    public void UpdateSkuCode_SetsNewCode()
    {
        // Arrange
        var item = InventoryItem.Create("Salt", "kg", 25m, 10m, "SKU-SALT-OLD");
        var newSkuCode = "SKU-SALT-NEW";

        // Act
        item.UpdateSkuCode(newSkuCode);

        // Assert
        item.SkuCode.Should().Be(newSkuCode);
    }

    [Fact]
    public void UpdateSkuCode_Null_ClearsSkuCode()
    {
        // Arrange
        var item = InventoryItem.Create("Pepper", "kg", 5m, 2m, "SKU-PEPPER-001");
        item.SkuCode.Should().NotBeNull();

        // Act
        item.UpdateSkuCode(null);

        // Assert
        item.SkuCode.Should().BeNull();
    }
}

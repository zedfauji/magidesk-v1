using Xunit;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Domain.Tests.Entities;

/// <summary>
/// Unit tests for MenuCategory hierarchy functionality (Feature G.4)
/// Tests domain invariants for parent-child relationships
/// </summary>
public class MenuCategoryTests
{
    [Fact]
    public void Create_ValidName_ReturnsCategory()
    {
        // Arrange & Act
        var category = MenuCategory.Create("Beverages");
        
        // Assert
        Assert.NotNull(category);
        Assert.Equal("Beverages", category.Name);
        Assert.Null(category.ParentCategoryId);
        Assert.True(category.IsRoot);
    }
    
    [Fact]
    public void SetParent_SelfReference_ThrowsBusinessRuleViolationException()
    {
        // Arrange
        var category = MenuCategory.Create("Test Category");
        
        // Act & Assert
        var exception = Assert.Throws<BusinessRuleViolationException>(() => 
            category.SetParent(category.Id));
        
        Assert.Contains("cannot set category as its own parent", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
    
    [Fact]
    public void SetParent_ValidParentId_SetsParentId()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var child = MenuCategory.Create("Child Category");
        
        // Act
        child.SetParent(parentId);
        
        // Assert
        Assert.Equal(parentId, child.ParentCategoryId);
        Assert.False(child.IsRoot);
    }
    
    [Fact]
    public void SetParent_NullParentId_ClearsParent()
    {
        // Arrange
        var child = MenuCategory.Create("Child");
        child.SetParent(Guid.NewGuid());
        
        // Act
        child.SetParent(null);
        
        // Assert
        Assert.Null(child.ParentCategoryId);
        Assert.True(child.IsRoot);
    }
    
    [Fact]
    public void ClearParent_WithParent_RemovesParent()
    {
        // Arrange
        var child = MenuCategory.Create("Child");
        child.SetParent(Guid.NewGuid());
        
        // Act
        child.ClearParent();
        
        // Assert
        Assert.Null(child.ParentCategoryId);
        Assert.True(child.IsRoot);
    }
    
    [Fact]
    public void IsRoot_WithNoParent_ReturnsTrue()
    {
        // Arrange & Act
        var category = MenuCategory.Create("Root");
        
        // Assert
        Assert.True(category.IsRoot);
    }
    
    [Fact]
    public void IsRoot_WithParent_ReturnsFalse()
    {
        // Arrange
        var category = MenuCategory.Create("Child");
        category.SetParent(Guid.NewGuid());
        
        // Act & Assert
        Assert.False(category.IsRoot);
    }
    
    [Fact]
    public void HasSubcategories_NewCategory_ReturnsFalse()
    {
        // Arrange & Act
        var category = MenuCategory.Create("Empty");
        
        // Assert
        Assert.False(category.HasSubcategories);
    }
    
    [Fact]
    public void GetDepth_RootCategory_ReturnsZero()
    {
        // Arrange
        var root = MenuCategory.Create("Root");
        
        // Act
        var depth = root.GetDepth();
        
        // Assert
        Assert.Equal(0, depth);
    }
}

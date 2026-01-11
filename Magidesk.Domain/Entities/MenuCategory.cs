using System;
using System.Collections.Generic;
using System.Linq;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a menu category with hierarchical support.
/// Categories organize menu items into logical groups for POS navigation.
/// Supports multi-level hierarchy (e.g., Beverages > Hot Drinks > Coffee).
/// </summary>
public class MenuCategory
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public int SortOrder { get; private set; }
    public bool IsVisible { get; private set; }
    public bool IsBeverage { get; private set; }
    public string? ButtonColor { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? PrinterGroupId { get; private set; }
    
    // Hierarchy Support (G.4)
    public Guid? ParentCategoryId { get; private set; }
    public virtual MenuCategory? Parent { get; private set; }
    public virtual ICollection<MenuCategory> Subcategories { get; private set; } = new List<MenuCategory>();

    // Private constructor for EF Core
    private MenuCategory()
    {
        Name = string.Empty;
    }

    public static MenuCategory Create(
        string name,
        int sortOrder = 0,
        bool isBeverage = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exceptions.BusinessRuleViolationException("Category name cannot be empty.");

        return new MenuCategory
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            SortOrder = sortOrder,
            IsVisible = true,
            IsBeverage = isBeverage,
            IsActive = true
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exceptions.BusinessRuleViolationException("Category name cannot be empty.");
        
        Name = name.Trim();
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }

    public void SetVisibility(bool isVisible)
    {
        IsVisible = isVisible;
    }

    public void SetBeverageFlag(bool isBeverage)
    {
        IsBeverage = isBeverage;
    }

    public void SetButtonColor(string? color)
    {
        ButtonColor = color;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void SetPrinterGroup(Guid? printerGroupId)
    {
        PrinterGroupId = printerGroupId;
    }
    
    /// <summary>
    /// Sets the parent category for hierarchical organization.
    /// Validates against self-reference. Circular reference validation
    /// must be performed by the application layer with repository access.
    /// </summary>
    /// <param name="parentCategoryId">Parent category ID or null for root level</param>
    /// <exception cref="Exceptions.BusinessRuleViolationException">
    /// Thrown if attempting to set category as its own parent
    /// </exception>
    public void SetParent(Guid? parentCategoryId)
    {
        // Invariant: Cannot set category as its own parent
        if (parentCategoryId.HasValue && parentCategoryId.Value == Id)
            throw new Exceptions.BusinessRuleViolationException(
                "Cannot set category as its own parent.");
        
        ParentCategoryId = parentCategoryId;
    }
    
    /// <summary>
    /// Clears the parent, making this a root-level category.
    /// </summary>
    public void ClearParent()
    {
        ParentCategoryId = null;
    }
    
    /// <summary>
    /// Gets the depth of this category in the hierarchy.
    /// Root categories have depth 0, their children have depth 1, etc.
    /// </summary>
    /// <returns>Hierarchy depth (0 for root)</returns>
    public int GetDepth()
    {
        int depth = 0;
        var current = Parent;
        
        while (current != null)
        {
            depth++;
            current = current.Parent;
            
            // Safety: Prevent infinite loop if data is corrupted
            if (depth > 10) break;
        }
        
        return depth;
    }
    
    /// <summary>
    /// Checks if this category is a root category (has no parent).
    /// </summary>
    public bool IsRoot => !ParentCategoryId.HasValue;
    
    /// <summary>
    /// Checks if this category has any subcategories.
    /// </summary>
    public bool HasSubcategories => Subcategories.Count > 0;
}

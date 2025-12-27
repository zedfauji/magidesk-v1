using System;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a sub-category within a MenuCategory (e.g., Hot Appetizers, Cold Appetizers).
/// Groups provide an additional level of organization within categories.
/// </summary>
public class MenuGroup
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public Guid CategoryId { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsVisible { get; private set; }
    public string? ButtonColor { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation property
    public MenuCategory? Category { get; private set; }

    // Private constructor for EF Core
    private MenuGroup()
    {
        Name = string.Empty;
    }

    public static MenuGroup Create(
        string name,
        Guid categoryId,
        int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exceptions.BusinessRuleViolationException("Group name cannot be empty.");

        if (categoryId == Guid.Empty)
            throw new Exceptions.BusinessRuleViolationException("Group must belong to a category.");

        return new MenuGroup
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            CategoryId = categoryId,
            SortOrder = sortOrder,
            IsVisible = true,
            IsActive = true
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exceptions.BusinessRuleViolationException("Group name cannot be empty.");
        
        Name = name.Trim();
    }

    public void UpdateCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new Exceptions.BusinessRuleViolationException("Group must belong to a category.");
        
        CategoryId = categoryId;
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }

    public void SetVisibility(bool isVisible)
    {
        IsVisible = isVisible;
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
}

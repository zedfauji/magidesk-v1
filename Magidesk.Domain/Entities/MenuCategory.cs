using System;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a top-level menu category (e.g., Appetizers, Entrees, Beverages).
/// Categories organize menu items into logical groups for POS navigation.
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

    // Private constructor for EF Core
    private MenuCategory()
    {
        Name = string.Empty;
    }
    
    public Guid? PrinterGroupId { get; private set; }

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
}

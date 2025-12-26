using System;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a group of modifiers (e.g., "Toppings", "Size", "Crust", "Sauce").
/// Used to organize modifiers and enforce selection rules.
/// </summary>
public class ModifierGroup
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsRequired { get; private set; }
    public int MinSelections { get; private set; }
    public int MaxSelections { get; private set; }
    public bool AllowMultipleSelections { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }
    public int Version { get; private set; }

    // Private constructor for EF Core
    private ModifierGroup()
    {
        Name = string.Empty;
    }

    /// <summary>
    /// Creates a new modifier group.
    /// </summary>
    public static ModifierGroup Create(
        string name,
        bool isRequired = false,
        int minSelections = 0,
        int maxSelections = 1,
        bool allowMultipleSelections = false,
        string? description = null,
        int displayOrder = 0,
        bool isActive = true)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exceptions.BusinessRuleViolationException("Modifier group name cannot be empty.");
        }

        if (minSelections < 0)
        {
            throw new Exceptions.BusinessRuleViolationException("Minimum selections cannot be negative.");
        }

        if (maxSelections < minSelections)
        {
            throw new Exceptions.BusinessRuleViolationException("Maximum selections cannot be less than minimum selections.");
        }

        if (maxSelections > 1 && !allowMultipleSelections)
        {
            throw new Exceptions.BusinessRuleViolationException("Multiple selections must be allowed when max selections is greater than 1.");
        }

        return new ModifierGroup
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            IsRequired = isRequired,
            MinSelections = minSelections,
            MaxSelections = maxSelections,
            AllowMultipleSelections = allowMultipleSelections,
            DisplayOrder = displayOrder,
            IsActive = isActive,
            Version = 1
        };
    }

    /// <summary>
    /// Updates the group name.
    /// </summary>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exceptions.BusinessRuleViolationException("Modifier group name cannot be empty.");
        }

        Name = name;
    }

    /// <summary>
    /// Updates the group description.
    /// </summary>
    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    /// <summary>
    /// Sets whether the group is required.
    /// </summary>
    public void SetIsRequired(bool isRequired)
    {
        IsRequired = isRequired;
    }

    /// <summary>
    /// Updates the selection constraints.
    /// </summary>
    public void UpdateSelectionConstraints(int minSelections, int maxSelections, bool allowMultipleSelections)
    {
        if (minSelections < 0)
        {
            throw new Exceptions.BusinessRuleViolationException("Minimum selections cannot be negative.");
        }

        if (maxSelections < minSelections)
        {
            throw new Exceptions.BusinessRuleViolationException("Maximum selections cannot be less than minimum selections.");
        }

        if (maxSelections > 1 && !allowMultipleSelections)
        {
            throw new Exceptions.BusinessRuleViolationException("Multiple selections must be allowed when max selections is greater than 1.");
        }

        MinSelections = minSelections;
        MaxSelections = maxSelections;
        AllowMultipleSelections = allowMultipleSelections;
    }

    /// <summary>
    /// Updates the display order.
    /// </summary>
    public void UpdateDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }

    /// <summary>
    /// Activates the modifier group.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the modifier group.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Validates if a selection count meets the group's constraints.
    /// </summary>
    public bool IsValidSelectionCount(int selectionCount)
    {
        if (IsRequired && selectionCount < MinSelections)
        {
            return false;
        }

        if (selectionCount < MinSelections || selectionCount > MaxSelections)
        {
            return false;
        }

        return true;
    }
}


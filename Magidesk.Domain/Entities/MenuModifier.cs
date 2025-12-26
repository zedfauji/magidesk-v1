using System;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a modifier definition (reference data).
/// Examples: "Extra Cheese", "Large Size", "No Onions", etc.
/// </summary>
public class MenuModifier
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid? ModifierGroupId { get; private set; }
    public ModifierType ModifierType { get; private set; }
    public Money BasePrice { get; private set; }
    public decimal TaxRate { get; private set; }
    public bool ShouldPrintToKitchen { get; private set; }
    public bool IsSectionWisePrice { get; private set; }
    public string? SectionName { get; private set; }
    public string? MultiplierName { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }
    public int Version { get; private set; }

    // Private constructor for EF Core
    private MenuModifier()
    {
        Name = string.Empty;
        BasePrice = Money.Zero();
    }

    /// <summary>
    /// Creates a new menu modifier.
    /// </summary>
    public static MenuModifier Create(
        string name,
        ModifierType modifierType,
        Money basePrice,
        Guid? modifierGroupId = null,
        string? description = null,
        decimal taxRate = 0m,
        bool shouldPrintToKitchen = true,
        bool isSectionWisePrice = false,
        string? sectionName = null,
        string? multiplierName = null,
        int displayOrder = 0,
        bool isActive = true)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exceptions.BusinessRuleViolationException("Modifier name cannot be empty.");
        }

        if (basePrice < Money.Zero())
        {
            throw new Exceptions.BusinessRuleViolationException("Base price cannot be negative.");
        }

        if (taxRate < 0 || taxRate > 1)
        {
            throw new Exceptions.BusinessRuleViolationException("Tax rate must be between 0 and 1.");
        }

        return new MenuModifier
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            ModifierGroupId = modifierGroupId,
            ModifierType = modifierType,
            BasePrice = basePrice,
            TaxRate = taxRate,
            ShouldPrintToKitchen = shouldPrintToKitchen,
            IsSectionWisePrice = isSectionWisePrice,
            SectionName = sectionName,
            MultiplierName = multiplierName,
            DisplayOrder = displayOrder,
            IsActive = isActive,
            Version = 1
        };
    }

    /// <summary>
    /// Updates the modifier name.
    /// </summary>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exceptions.BusinessRuleViolationException("Modifier name cannot be empty.");
        }

        Name = name;
    }

    /// <summary>
    /// Updates the modifier description.
    /// </summary>
    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    /// <summary>
    /// Updates the base price.
    /// </summary>
    public void UpdateBasePrice(Money basePrice)
    {
        if (basePrice < Money.Zero())
        {
            throw new Exceptions.BusinessRuleViolationException("Base price cannot be negative.");
        }

        BasePrice = basePrice;
    }

    /// <summary>
    /// Updates the tax rate.
    /// </summary>
    public void UpdateTaxRate(decimal taxRate)
    {
        if (taxRate < 0 || taxRate > 1)
        {
            throw new Exceptions.BusinessRuleViolationException("Tax rate must be between 0 and 1.");
        }

        TaxRate = taxRate;
    }

    /// <summary>
    /// Updates the modifier group assignment.
    /// </summary>
    public void UpdateModifierGroup(Guid? modifierGroupId)
    {
        ModifierGroupId = modifierGroupId;
    }

    /// <summary>
    /// Sets whether the modifier should print to kitchen.
    /// </summary>
    public void SetShouldPrintToKitchen(bool shouldPrint)
    {
        ShouldPrintToKitchen = shouldPrint;
    }

    /// <summary>
    /// Sets section-wise pricing configuration.
    /// </summary>
    public void SetSectionWisePrice(bool isSectionWisePrice, string? sectionName = null)
    {
        IsSectionWisePrice = isSectionWisePrice;
        SectionName = sectionName;
    }

    /// <summary>
    /// Sets the multiplier name (for pizza-style modifiers).
    /// </summary>
    public void SetMultiplierName(string? multiplierName)
    {
        MultiplierName = multiplierName;
    }

    /// <summary>
    /// Updates the display order.
    /// </summary>
    public void UpdateDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }

    /// <summary>
    /// Activates the modifier.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the modifier.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }
}


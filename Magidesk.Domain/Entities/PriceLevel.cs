using System;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a price tier/level (e.g., "Happy Hour", "Delivery").
/// </summary>
public class PriceLevel
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDefault { get; private set; }
    public int DisplayOrder { get; private set; }

    // Private constructor for EF Core
    private PriceLevel() { }

    public static PriceLevel Create(string name, string? description = null, bool isDefault = false, int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        return new PriceLevel
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            IsActive = true,
            IsDefault = isDefault,
            DisplayOrder = displayOrder
        };
    }

    public void Update(string name, string? description, bool isActive, bool isDefault, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name;
        Description = description;
        IsActive = isActive;
        IsDefault = isDefault;
        DisplayOrder = displayOrder;
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

using System;
using System.Collections.Generic;
using System.Linq;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a restaurant floor or dining area.
/// Contains layout information and associated tables.
/// </summary>
public class Floor
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int Width { get; private set; } = 2000;
    public int Height { get; private set; } = 2000;
    public string BackgroundColor { get; private set; } = "#f8f8f8";
    public List<TableLayout> TableLayouts { get; private set; } = new();
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public int Version { get; private set; }

    // Private constructor for EF Core
    private Floor()
    {
    }

    /// <summary>
    /// Creates a new floor.
    /// </summary>
    public static Floor Create(string name, string description = "", int width = 2000, int height = 2000)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exceptions.BusinessRuleViolationException("Floor name cannot be empty.");
        }

        if (width <= 0 || height <= 0)
        {
            throw new Exceptions.BusinessRuleViolationException("Floor dimensions must be greater than zero.");
        }

        return new Floor
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Width = width,
            Height = height,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 1
        };
    }

    /// <summary>
    /// Updates the floor name.
    /// </summary>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exceptions.BusinessRuleViolationException("Floor name cannot be empty.");
        }

        Name = name;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Updates the floor description.
    /// </summary>
    public void UpdateDescription(string description)
    {
        Description = description ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Updates the floor dimensions.
    /// </summary>
    public void UpdateDimensions(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            throw new Exceptions.BusinessRuleViolationException("Floor dimensions must be greater than zero.");
        }

        Width = width;
        Height = height;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Updates the background color.
    /// </summary>
    public void UpdateBackgroundColor(string backgroundColor)
    {
        BackgroundColor = backgroundColor ?? "#f8f8f8";
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Adds a layout to the floor.
    /// </summary>
    public void AddLayout(TableLayout layout)
    {
        if (layout == null)
        {
            throw new ArgumentNullException(nameof(layout));
        }

        // Check for duplicate layout names
        if (TableLayouts.Any(l => l.Name.Equals(layout.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new Exceptions.BusinessRuleViolationException($"Layout name '{layout.Name}' already exists on this floor.");
        }

        TableLayouts.Add(layout);
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Removes a layout from the floor.
    /// </summary>
    public void RemoveLayout(Guid layoutId)
    {
        var layout = TableLayouts.FirstOrDefault(l => l.Id == layoutId);
        if (layout != null)
        {
            TableLayouts.Remove(layout);
            UpdatedAt = DateTime.UtcNow;
            Version++;
        }
    }

    /// <summary>
    /// Gets the active layout for the floor.
    /// </summary>
    public TableLayout? GetActiveLayout()
    {
        return TableLayouts.FirstOrDefault(l => l.IsActive && !l.IsDraft);
    }

    /// <summary>
    /// Gets all tables across all layouts on this floor.
    /// </summary>
    public IReadOnlyList<Table> GetAllTables()
    {
        return TableLayouts.SelectMany(l => l.Tables).ToList().AsReadOnly();
    }

    /// <summary>
    /// Activates the floor.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Deactivates the floor.
    /// </summary>
    public void Deactivate()
    {
        // Check if any layouts have active tables
        if (TableLayouts.Any(l => l.Tables.Any(t => t.Status == TableStatus.Seat)))
        {
            throw new Exceptions.InvalidOperationException("Cannot deactivate floor with active tables.");
        }

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Checks if a point is within the floor bounds.
    /// </summary>
    public bool ContainsPoint(int x, int y)
    {
        return x >= 0 && y >= 0 && x < Width && y < Height;
    }
}

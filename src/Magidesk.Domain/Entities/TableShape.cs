using System;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a table shape configuration.
/// Defines the visual appearance and properties of table shapes.
/// </summary>
public class TableShape
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public TableShapeType ShapeType { get; private set; }
    public int Width { get; private set; } = 100;
    public int Height { get; private set; } = 100;
    public string BackgroundColor { get; private set; } = "#ffffff";
    public string BorderColor { get; private set; } = "#cccccc";
    public int BorderThickness { get; private set; } = 2;
    public int CornerRadius { get; private set; } = 8;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Private constructor for EF Core
    private TableShape()
    {
    }

    /// <summary>
    /// Creates a new table shape.
    /// </summary>
    public static TableShape Create(string name, TableShapeType shapeType, int width = 100, int height = 100)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exceptions.BusinessRuleViolationException("Shape name cannot be empty.");
        }

        if (width <= 0 || height <= 0)
        {
            throw new Exceptions.BusinessRuleViolationException("Shape dimensions must be greater than zero.");
        }

        var cornerRadius = shapeType switch
        {
            TableShapeType.Rectangle => 8,
            TableShapeType.Square => 8,
            TableShapeType.Round => width / 2, // Make it circular
            TableShapeType.Oval => height / 2, // Make it elliptical
            _ => 8
        };

        return new TableShape
        {
            Id = Guid.NewGuid(),
            Name = name,
            ShapeType = shapeType,
            Width = width,
            Height = height,
            CornerRadius = cornerRadius,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Updates the shape name.
    /// </summary>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exceptions.BusinessRuleViolationException("Shape name cannot be empty.");
        }

        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the shape dimensions.
    /// </summary>
    public void UpdateDimensions(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            throw new Exceptions.BusinessRuleViolationException("Shape dimensions must be greater than zero.");
        }

        Width = width;
        Height = height;

        // Update corner radius based on shape type
        CornerRadius = ShapeType switch
        {
            TableShapeType.Rectangle => 8,
            TableShapeType.Square => 8,
            TableShapeType.Round => width / 2,
            TableShapeType.Oval => height / 2,
            _ => 8
        };

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the shape colors.
    /// </summary>
    public void UpdateColors(string backgroundColor, string borderColor)
    {
        BackgroundColor = backgroundColor ?? "#ffffff";
        BorderColor = borderColor ?? "#cccccc";
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the border thickness.
    /// </summary>
    public void UpdateBorderThickness(int thickness)
    {
        if (thickness < 0)
        {
            throw new Exceptions.BusinessRuleViolationException("Border thickness cannot be negative.");
        }

        BorderThickness = thickness;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the shape.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the shape.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the shape as a DTO.
    /// </summary>
    public TableShapeDto ToDto()
    {
        return new TableShapeDto
        {
            Id = Id,
            Name = Name,
            ShapeType = ShapeType,
            Width = Width,
            Height = Height,
            BackgroundColor = BackgroundColor,
            BorderColor = BorderColor,
            BorderThickness = BorderThickness,
            CornerRadius = CornerRadius,
            IsActive = IsActive
        };
    }
}

/// <summary>
/// DTO for table shape (defined here to avoid circular references)
/// </summary>
public class TableShapeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TableShapeType ShapeType { get; set; }
    public int Width { get; set; } = 100;
    public int Height { get; set; } = 100;
    public string BackgroundColor { get; set; } = "#ffffff";
    public string BorderColor { get; set; } = "#cccccc";
    public int BorderThickness { get; set; } = 2;
    public int CornerRadius { get; set; } = 8;
    public bool IsActive { get; set; } = true;
}

using System;
using System.Collections.Generic;
using System.Linq;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a table layout configuration for a restaurant floor.
/// Contains the arrangement and properties of tables in a specific layout.
/// </summary>
public class TableLayout
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Guid? FloorId { get; private set; }
    public Floor? Floor { get; private set; }
    public List<Table> Tables { get; private set; } = new();
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsDraft { get; private set; } = false;
    public int Version { get; private set; }

    // Private constructor for EF Core
    private TableLayout()
    {
    }

    /// <summary>
    /// Creates a new table layout.
    /// </summary>
    public static TableLayout Create(string name, Guid? floorId = null, bool isDraft = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exceptions.BusinessRuleViolationException("Layout name cannot be empty.");
        }

        return new TableLayout
        {
            Id = Guid.NewGuid(),
            Name = name,
            FloorId = floorId,
            IsDraft = isDraft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 1
        };
    }

    /// <summary>
    /// Updates the layout name.
    /// </summary>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exceptions.BusinessRuleViolationException("Layout name cannot be empty.");
        }

        Name = name;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Updates the floor assignment.
    /// </summary>
    public void UpdateFloor(Guid? floorId)
    {
        FloorId = floorId;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Adds a table to the layout.
    /// </summary>
    public void AddTable(Table table)
    {
        if (table == null)
        {
            throw new ArgumentNullException(nameof(table));
        }

        // Check for duplicate table numbers
        if (Tables.Any(t => t.TableNumber == table.TableNumber))
        {
            throw new Exceptions.BusinessRuleViolationException($"Table number {table.TableNumber} already exists in this layout.");
        }

        Tables.Add(table);
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Removes a table from the layout.
    /// </summary>
    public void RemoveTable(Guid tableId)
    {
        var table = Tables.FirstOrDefault(t => t.Id == tableId);
        if (table != null)
        {
            // Check if table has active ticket
            if (table.Status != TableStatus.Available)
            {
                throw new Exceptions.InvalidOperationException($"Cannot remove table {table.TableNumber} with status {table.Status}. Table must be available.");
            }

            Tables.Remove(table);
            UpdatedAt = DateTime.UtcNow;
            Version++;
        }
    }

    /// <summary>
    /// Updates a table's position in the layout.
    /// </summary>
    public void UpdateTablePosition(Guid tableId, double x, double y)
    {
        var table = Tables.FirstOrDefault(t => t.Id == tableId);
        if (table != null)
        {
            table.UpdatePosition(x, y);
            UpdatedAt = DateTime.UtcNow;
            Version++;
        }
    }

    /// <summary>
    /// Gets tables by their status.
    /// </summary>
    public IReadOnlyList<Table> GetTablesByStatus(TableStatus status)
    {
        return Tables.Where(t => t.Status == status).ToList().AsReadOnly();
    }

    /// <summary>
    /// Gets the next available table number.
    /// </summary>
    public int GetNextTableNumber()
    {
        if (!Tables.Any())
        {
            return 1;
        }

        return Tables.Max(t => t.TableNumber) + 1;
    }

    /// <summary>
    /// Activates the layout.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Deactivates the layout.
    /// </summary>
    public void Deactivate()
    {
        // Check if any tables have active tickets
        if (Tables.Any(t => t.Status == TableStatus.Seat))
        {
            throw new Exceptions.InvalidOperationException("Cannot deactivate layout with active tables.");
        }

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Marks the layout as a draft or published.
    /// </summary>
    public void SetDraftStatus(bool isDraft)
    {
        IsDraft = isDraft;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Checks if the layout is valid for use.
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name) && Tables.Any();
    }
}

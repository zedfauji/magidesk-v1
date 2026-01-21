using System;
using System.Collections.Generic;
using System.Linq;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a server section assignment for managing table assignments to servers.
/// Used to organize tables by server responsibility areas.
/// </summary>
public class ServerSection
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid ServerId { get; private set; }
    public List<Guid> TableIds { get; private set; } = new();
    public string Color { get; private set; } = "#3498db"; // Default blue
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public int Version { get; private set; }
    
    // Navigation properties
    public virtual User? Server { get; private set; }
    public virtual ICollection<Table> Tables { get; private set; } = new List<Table>();

    // Private constructor for EF Core
    private ServerSection()
    {
    }

    /// <summary>
    /// Creates a new server section.
    /// </summary>
    public static ServerSection Create(string name, Guid serverId, string description = "", string color = "#3498db")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exceptions.BusinessRuleViolationException("Section name cannot be empty.");
        }

        if (serverId == Guid.Empty)
        {
            throw new Exceptions.BusinessRuleViolationException("Server ID is required.");
        }

        return new ServerSection
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description ?? string.Empty,
            ServerId = serverId,
            Color = color ?? "#3498db",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 1
        };
    }

    /// <summary>
    /// Updates the section name.
    /// </summary>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exceptions.BusinessRuleViolationException("Section name cannot be empty.");
        }

        Name = name;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Updates the section description.
    /// </summary>
    public void UpdateDescription(string description)
    {
        Description = description ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Updates the section color.
    /// </summary>
    public void UpdateColor(string color)
    {
        Color = color ?? "#3498db";
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Updates the server assignment.
    /// </summary>
    public void UpdateServer(Guid serverId)
    {
        if (serverId == Guid.Empty)
        {
            throw new Exceptions.BusinessRuleViolationException("Server ID is required.");
        }

        ServerId = serverId;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Adds tables to the section.
    /// </summary>
    public void AddTables(IEnumerable<Guid> tableIds)
    {
        if (tableIds == null)
        {
            throw new ArgumentNullException(nameof(tableIds));
        }

        foreach (var tableId in tableIds)
        {
            if (!TableIds.Contains(tableId))
            {
                TableIds.Add(tableId);
            }
        }

        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Adds a single table to the section.
    /// </summary>
    public void AddTable(Guid tableId)
    {
        if (tableId == Guid.Empty)
        {
            throw new ArgumentException("Table ID cannot be empty.", nameof(tableId));
        }

        if (!TableIds.Contains(tableId))
        {
            TableIds.Add(tableId);
            UpdatedAt = DateTime.UtcNow;
            Version++;
        }
    }

    /// <summary>
    /// Removes tables from the section.
    /// </summary>
    public void RemoveTables(IEnumerable<Guid> tableIds)
    {
        if (tableIds == null)
        {
            throw new ArgumentNullException(nameof(tableIds));
        }

        foreach (var tableId in tableIds)
        {
            TableIds.Remove(tableId);
        }

        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Removes a single table from the section.
    /// </summary>
    public void RemoveTable(Guid tableId)
    {
        if (TableIds.Remove(tableId))
        {
            UpdatedAt = DateTime.UtcNow;
            Version++;
        }
    }

    /// <summary>
    /// Clears all tables from the section.
    /// </summary>
    public void ClearTables()
    {
        if (TableIds.Any())
        {
            TableIds.Clear();
            UpdatedAt = DateTime.UtcNow;
            Version++;
        }
    }

    /// <summary>
    /// Checks if a table is in this section.
    /// </summary>
    public bool ContainsTable(Guid tableId)
    {
        return TableIds.Contains(tableId);
    }

    /// <summary>
    /// Gets the count of tables in this section.
    /// </summary>
    public int TableCount => TableIds.Count;

    /// <summary>
    /// Activates the section.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Deactivates the section.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Checks if the section is valid for use.
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name) && ServerId != Guid.Empty;
    }
}

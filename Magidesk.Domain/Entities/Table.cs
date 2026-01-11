using System;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a restaurant table.
/// Used for table management and assignment to tickets.
/// </summary>
public class Table
{
    public Guid Id { get; private set; }
    public int TableNumber { get; private set; }
    public int Capacity { get; private set; }
    public double X { get; private set; } = 0;
    public double Y { get; private set; } = 0;
    public double Width { get; private set; } = 100; // Default width
    public double Height { get; private set; } = 100; // Default height
    public TableShapeType Shape { get; private set; } = TableShapeType.Rectangle;
    public Guid? FloorId { get; private set; }
    public Guid? LayoutId { get; private set; }
    public TableLayout? Layout { get; private set; }
    public TableStatus Status { get; private set; } = TableStatus.Available;
    public Guid? CurrentTicketId { get; private set; }
    public Guid? TableTypeId { get; private set; }
    public TableType? TableType { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public int Version { get; private set; }

    // Private constructor for EF Core
    private Table()
    {
    }

    /// <summary>
    /// Creates a new table.
    /// </summary>
    public static Table Create(
        int tableNumber,
        int capacity,
        double x = 0,
        double y = 0,
        Guid? floorId = null,
        Guid? layoutId = null,
        Guid? tableTypeId = null,
        bool isActive = true,
        TableShapeType shape = TableShapeType.Rectangle,
        double width = 100,
        double height = 100)
    {
        if (tableNumber <= 0)
        {
            throw new Exceptions.BusinessRuleViolationException("Table number must be greater than zero.");
        }

        if (capacity <= 0)
        {
            throw new Exceptions.BusinessRuleViolationException("Table capacity must be greater than zero.");
        }

        return new Table
        {
            Id = Guid.NewGuid(),
            TableNumber = tableNumber,
            FloorId = floorId,
            Capacity = capacity,
            X = x,
            Y = y,
            Width = width,
            Height = height,
            Shape = shape,
            Status = TableStatus.Available,
            CurrentTicketId = null,
            TableTypeId = tableTypeId,
            LayoutId = layoutId,
            IsActive = isActive,
            Version = 1
        };
    }

    /// <summary>
    /// Updates the table number.
    /// </summary>
    public void UpdateTableNumber(int tableNumber)
    {
        if (tableNumber <= 0)
        {
            throw new Exceptions.BusinessRuleViolationException("Table number must be greater than zero.");
        }

        TableNumber = tableNumber;
    }

    /// <summary>
    /// Updates the table capacity.
    /// </summary>
    public void UpdateCapacity(int capacity)
    {
        if (capacity <= 0)
        {
            throw new Exceptions.BusinessRuleViolationException("Table capacity must be greater than zero.");
        }

        Capacity = capacity;
    }

    /// <summary>
    /// Updates the table position.
    /// </summary>
    public void UpdatePosition(double x, double y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Updates all geometry properties.
    /// </summary>
    public void UpdateGeometry(double x, double y, TableShapeType shape, double width, double height)
    {
        X = x;
        Y = y;
        Shape = shape;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Updates the floor assignment.
    /// </summary>
    public void UpdateFloor(Guid? floorId)
    {
        FloorId = floorId;
    }

    /// <summary>
    /// Assigns a table type to this table for pricing purposes.
    /// </summary>
    /// <param name="tableTypeId">The ID of the table type to assign.</param>
    /// <exception cref="ArgumentException">Thrown when tableTypeId is empty.</exception>
    public void SetTableType(Guid tableTypeId)
    {
        if (tableTypeId == Guid.Empty)
        {
            throw new ArgumentException("Table type ID cannot be empty.", nameof(tableTypeId));
        }

        TableTypeId = tableTypeId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes the table type assignment from this table.
    /// </summary>
    public void ClearTableType()
    {
        TableTypeId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Assigns a ticket to this table.
    /// </summary>
    public void AssignTicket(Guid ticketId)
    {
        // If already assigned to the same ticket, no-op
        if (CurrentTicketId.HasValue && CurrentTicketId.Value == ticketId)
        {
            return;
        }

        if (Status != TableStatus.Available && Status != TableStatus.Booked)
        {
            throw new Exceptions.InvalidOperationException($"Cannot assign ticket to table with status {Status}.");
        }

        if (CurrentTicketId.HasValue && CurrentTicketId.Value != ticketId)
        {
            throw new Exceptions.InvalidOperationException("Table already has an assigned ticket.");
        }

        CurrentTicketId = ticketId;
        Status = TableStatus.Seat;
    }

    /// <summary>
    /// Releases the ticket from this table.
    /// </summary>
    public void ReleaseTicket()
    {
        if (!CurrentTicketId.HasValue)
        {
            throw new Exceptions.InvalidOperationException("Table does not have an assigned ticket.");
        }

        CurrentTicketId = null;
        Status = TableStatus.Available;
    }

    /// <summary>
    /// Books the table (reserves it).
    /// </summary>
    public void Book()
    {
        if (Status != TableStatus.Available)
        {
            throw new Exceptions.InvalidOperationException($"Cannot book table with status {Status}.");
        }

        Status = TableStatus.Booked;
    }

    /// <summary>
    /// Marks table as in-use (for sessions without tickets).
    /// </summary>
    public void MarkInUse()
    {
        if (Status == TableStatus.Seat)
        {
            // Already in use, no-op
            return;
        }

        if (Status != TableStatus.Available && Status != TableStatus.Booked)
        {
            throw new Exceptions.InvalidOperationException($"Cannot mark table in-use from status {Status}.");
        }

        Status = TableStatus.Seat;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks table as available (releases from session).
    /// </summary>
    public void MarkAvailable()
    {
        if (CurrentTicketId.HasValue)
        {
            throw new Exceptions.InvalidOperationException("Cannot mark table available while ticket is assigned. Release ticket first.");
        }

        Status = TableStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the table as dirty (needs cleaning).
    /// </summary>
    public void MarkDirty()
    {
        if (Status != TableStatus.Available)
        {
            throw new Exceptions.InvalidOperationException($"Cannot mark table as dirty with status {Status}.");
        }

        Status = TableStatus.Dirty;
    }

    /// <summary>
    /// Marks the table as clean (available).
    /// </summary>
    public void MarkClean()
    {
        if (Status != TableStatus.Dirty)
        {
            throw new Exceptions.InvalidOperationException($"Cannot mark table as clean with status {Status}.");
        }

        Status = TableStatus.Available;
    }

    /// <summary>
    /// Disables the table.
    /// </summary>
    public void Disable()
    {
        if (Status == TableStatus.Seat)
        {
            throw new Exceptions.InvalidOperationException("Cannot disable table with active ticket.");
        }

        Status = TableStatus.Disable;
    }

    /// <summary>
    /// Enables the table (makes it available).
    /// </summary>
    public void Enable()
    {
        if (Status != TableStatus.Disable)
        {
            throw new Exceptions.InvalidOperationException($"Cannot enable table with status {Status}.");
        }

        Status = TableStatus.Available;
    }

    /// <summary>
    /// Activates the table.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the table.
    /// </summary>
    public void Deactivate()
    {
        if (Status == TableStatus.Seat)
        {
            throw new Exceptions.InvalidOperationException("Cannot deactivate table with active ticket.");
        }

        IsActive = false;
    }

    /// <summary>
    /// Checks if the table is available for assignment.
    /// </summary>
    public bool IsAvailable()
    {
        return IsActive && Status == TableStatus.Available;
    }
}


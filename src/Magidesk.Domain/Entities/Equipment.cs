using System;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents equipment and accessories that can be assigned to tables.
/// Tracks equipment status, availability, and maintenance requirements.
/// </summary>
public class Equipment
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public EquipmentType Type { get; private set; }
    public EquipmentStatus Status { get; private set; }
    public Guid? AssignedTableId { get; private set; }
    public DateTime? LastMaintenanceDate { get; private set; }
    public DateTime? NextMaintenanceDate { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Private constructor for EF Core
    private Equipment()
    {
    }

    /// <summary>
    /// Creates a new equipment item.
    /// </summary>
    /// <param name="name">Name of the equipment</param>
    /// <param name="type">Type of equipment</param>
    /// <param name="description">Optional description</param>
    /// <returns>New Equipment instance</returns>
    /// <exception cref="ArgumentException">Thrown when name is empty</exception>
    public static Equipment Create(string name, EquipmentType type, string description = "")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Equipment name cannot be empty.", nameof(name));
        }

        var now = DateTime.UtcNow;

        return new Equipment
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Type = type,
            Description = description?.Trim() ?? string.Empty,
            Status = EquipmentStatus.Available,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    /// <summary>
    /// Assigns this equipment to a table.
    /// </summary>
    /// <param name="tableId">ID of the table to assign to</param>
    /// <exception cref="BusinessRuleViolationException">Thrown when equipment cannot be assigned</exception>
    public void AssignToTable(Guid tableId)
    {
        if (tableId == Guid.Empty)
        {
            throw new ArgumentException("Table ID cannot be empty.", nameof(tableId));
        }

        if (Status != EquipmentStatus.Available)
        {
            throw new BusinessRuleViolationException("Equipment must be available to assign to table");
        }

        if (!IsActive)
        {
            throw new BusinessRuleViolationException("Cannot assign inactive equipment to table");
        }

        AssignedTableId = tableId;
        Status = EquipmentStatus.InUse;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Unassigns this equipment from its current table.
    /// </summary>
    /// <exception cref="BusinessRuleViolationException">Thrown when equipment is not assigned</exception>
    public void UnassignFromTable()
    {
        if (!AssignedTableId.HasValue)
        {
            throw new BusinessRuleViolationException("Equipment is not currently assigned to a table");
        }

        AssignedTableId = null;
        Status = EquipmentStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Schedules maintenance for this equipment.
    /// </summary>
    /// <param name="maintenanceDate">Date when maintenance is scheduled</param>
    /// <exception cref="ArgumentException">Thrown when maintenance date is in the past</exception>
    public void ScheduleMaintenance(DateTime maintenanceDate)
    {
        if (maintenanceDate <= DateTime.UtcNow)
        {
            throw new ArgumentException("Maintenance date must be in the future.", nameof(maintenanceDate));
        }

        NextMaintenanceDate = maintenanceDate;
        
        // If maintenance is due within 7 days, mark as requiring maintenance
        if (maintenanceDate <= DateTime.UtcNow.AddDays(7))
        {
            Status = EquipmentStatus.MaintenanceRequired;
            
            // If assigned to a table, unassign it
            if (AssignedTableId.HasValue)
            {
                AssignedTableId = null;
            }
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Records that maintenance has been completed.
    /// </summary>
    public void CompleteMaintenance()
    {
        LastMaintenanceDate = DateTime.UtcNow;
        NextMaintenanceDate = null;
        
        // Return to available status if not out of service
        if (Status == EquipmentStatus.MaintenanceRequired)
        {
            Status = EquipmentStatus.Available;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks equipment as out of service.
    /// </summary>
    /// <param name="reason">Reason for taking out of service</param>
    public void TakeOutOfService(string reason = "")
    {
        Status = EquipmentStatus.OutOfService;
        
        // If assigned to a table, unassign it
        if (AssignedTableId.HasValue)
        {
            AssignedTableId = null;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Returns equipment to service.
    /// </summary>
    public void ReturnToService()
    {
        if (Status == EquipmentStatus.OutOfService)
        {
            Status = EquipmentStatus.Available;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Marks equipment as missing.
    /// </summary>
    public void MarkAsMissing()
    {
        Status = EquipmentStatus.Missing;
        
        // If assigned to a table, unassign it
        if (AssignedTableId.HasValue)
        {
            AssignedTableId = null;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks equipment as found (returns to available status).
    /// </summary>
    public void MarkAsFound()
    {
        if (Status == EquipmentStatus.Missing)
        {
            Status = EquipmentStatus.Available;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Deactivates this equipment item.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        
        // If assigned to a table, unassign it
        if (AssignedTableId.HasValue)
        {
            AssignedTableId = null;
            Status = EquipmentStatus.Available;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates this equipment item.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the equipment details.
    /// </summary>
    /// <param name="name">New name</param>
    /// <param name="description">New description</param>
    /// <exception cref="ArgumentException">Thrown when name is empty</exception>
    public void UpdateDetails(string name, string description = "")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Equipment name cannot be empty.", nameof(name));
        }

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }
}
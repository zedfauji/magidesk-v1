using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for equipment data access.
/// </summary>
public interface IEquipmentRepository : IRepository<Equipment>
{
    /// <summary>
    /// Gets all equipment assigned to a specific table.
    /// </summary>
    /// <param name="tableId">ID of the table</param>
    /// <returns>List of equipment assigned to the table</returns>
    Task<IEnumerable<Equipment>> GetEquipmentByTableIdAsync(Guid tableId);

    /// <summary>
    /// Gets all equipment of a specific type.
    /// </summary>
    /// <param name="equipmentType">Type of equipment</param>
    /// <returns>List of equipment of the specified type</returns>
    Task<IEnumerable<Equipment>> GetEquipmentByTypeAsync(EquipmentType equipmentType);

    /// <summary>
    /// Gets all equipment with a specific status.
    /// </summary>
    /// <param name="status">Equipment status</param>
    /// <returns>List of equipment with the specified status</returns>
    Task<IEnumerable<Equipment>> GetEquipmentByStatusAsync(EquipmentStatus status);

    /// <summary>
    /// Gets all available equipment of a specific type.
    /// </summary>
    /// <param name="equipmentType">Type of equipment</param>
    /// <returns>List of available equipment of the specified type</returns>
    Task<IEnumerable<Equipment>> GetAvailableEquipmentByTypeAsync(EquipmentType equipmentType);

    /// <summary>
    /// Gets equipment that requires maintenance within the specified number of days.
    /// </summary>
    /// <param name="daysAhead">Number of days to look ahead</param>
    /// <returns>List of equipment requiring maintenance</returns>
    Task<IEnumerable<Equipment>> GetEquipmentRequiringMaintenanceAsync(int daysAhead = 7);

    /// <summary>
    /// Gets all active equipment.
    /// </summary>
    /// <returns>List of active equipment</returns>
    Task<IEnumerable<Equipment>> GetActiveEquipmentAsync();

    /// <summary>
    /// Checks if equipment is available for assignment.
    /// </summary>
    /// <param name="equipmentId">ID of the equipment</param>
    /// <returns>True if available, false otherwise</returns>
    Task<bool> IsEquipmentAvailableAsync(Guid equipmentId);

    /// <summary>
    /// Gets equipment utilization data for analytics.
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Equipment utilization data</returns>
    Task<IEnumerable<EquipmentUtilizationData>> GetEquipmentUtilizationAsync(DateTime fromDate, DateTime toDate);
}

/// <summary>
/// Equipment utilization data for analytics.
/// </summary>
public record EquipmentUtilizationData(
    Guid EquipmentId,
    string Name,
    EquipmentType Type,
    TimeSpan TotalUsageTime,
    int AssignmentCount,
    decimal UtilizationPercentage,
    DateTime LastUsed
);
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Represents how charges should be allocated when splitting a merged table.
/// </summary>
public record TableSplitAllocation(
    IReadOnlyDictionary<Guid, SplitTableAllocation> TableAllocations
)
{
    /// <summary>
    /// Creates a new table split allocation.
    /// </summary>
    /// <param name="allocations">Dictionary mapping table IDs to their allocation details</param>
    /// <returns>New TableSplitAllocation instance</returns>
    /// <exception cref="ArgumentException">Thrown when allocations are invalid</exception>
    public static TableSplitAllocation Create(IDictionary<Guid, SplitTableAllocation> allocations)
    {
        if (allocations == null || !allocations.Any())
        {
            throw new ArgumentException("At least one table allocation is required.", nameof(allocations));
        }

        // Validate that percentages sum to 100%
        var totalPercentage = allocations.Values.Sum(a => a.ChargePercentage);
        if (Math.Abs(totalPercentage - 100m) > 0.01m)
        {
            throw new ArgumentException($"Charge percentages must sum to 100%. Current total: {totalPercentage}%");
        }

        // Validate that all percentages are positive
        if (allocations.Values.Any(a => a.ChargePercentage <= 0))
        {
            throw new ArgumentException("All charge percentages must be greater than zero.");
        }

        // Validate that all table IDs are unique and not empty
        if (allocations.Keys.Any(id => id == Guid.Empty))
        {
            throw new ArgumentException("All table IDs must be valid (non-empty GUIDs).");
        }

        return new TableSplitAllocation(allocations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
    }

    /// <summary>
    /// Gets the total number of tables in this split allocation.
    /// </summary>
    public int TableCount => TableAllocations.Count;

    /// <summary>
    /// Gets the total charge percentage (should always be 100%).
    /// </summary>
    public decimal TotalChargePercentage => TableAllocations.Values.Sum(a => a.ChargePercentage);

    /// <summary>
    /// Validates that the allocation is mathematically consistent.
    /// </summary>
    /// <returns>True if valid, false otherwise</returns>
    public bool IsValid()
    {
        return TableAllocations.Any() && 
               Math.Abs(TotalChargePercentage - 100m) <= 0.01m &&
               TableAllocations.Values.All(a => a.ChargePercentage > 0) &&
               TableAllocations.Keys.All(id => id != Guid.Empty);
    }
}

/// <summary>
/// Allocation details for a single table in a split operation.
/// </summary>
public record SplitTableAllocation(
    Guid TableId,
    decimal ChargePercentage,
    int GuestCount,
    IReadOnlyList<Guid>? EquipmentIds = null,
    IReadOnlyList<Guid>? ServerIds = null
)
{
    /// <summary>
    /// Creates a new split table allocation.
    /// </summary>
    /// <param name="tableId">ID of the table</param>
    /// <param name="chargePercentage">Percentage of total charges (0-100)</param>
    /// <param name="guestCount">Number of guests for this table</param>
    /// <param name="equipmentIds">Optional equipment to assign to this table</param>
    /// <param name="serverIds">Optional servers to assign to this table</param>
    /// <returns>New SplitTableAllocation instance</returns>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
    public static SplitTableAllocation Create(
        Guid tableId, 
        decimal chargePercentage, 
        int guestCount,
        IEnumerable<Guid>? equipmentIds = null,
        IEnumerable<Guid>? serverIds = null)
    {
        if (tableId == Guid.Empty)
        {
            throw new ArgumentException("Table ID cannot be empty.", nameof(tableId));
        }

        if (chargePercentage <= 0 || chargePercentage > 100)
        {
            throw new ArgumentException("Charge percentage must be between 0 and 100.", nameof(chargePercentage));
        }

        if (guestCount <= 0)
        {
            throw new ArgumentException("Guest count must be greater than zero.", nameof(guestCount));
        }

        return new SplitTableAllocation(
            tableId,
            chargePercentage,
            guestCount,
            equipmentIds?.ToList(),
            serverIds?.ToList()
        );
    }
}
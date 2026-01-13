using System;
using System.Collections.Generic;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Status information for a table's merge configuration.
/// </summary>
public record TableMergeStatus(
    Guid TableId,
    bool IsMerged,
    Guid? MergedSessionId = null,
    Guid? PrimaryTableId = null,
    IReadOnlyList<Guid>? MergedTableIds = null,
    DateTime? MergeTimestamp = null,
    string? MergeReason = null,
    Guid? MergedByStaffId = null
)
{
    /// <summary>
    /// Creates a status for a non-merged table.
    /// </summary>
    /// <param name="tableId">ID of the table</param>
    /// <returns>TableMergeStatus for a standalone table</returns>
    public static TableMergeStatus NotMerged(Guid tableId)
    {
        if (tableId == Guid.Empty)
        {
            throw new ArgumentException("Table ID cannot be empty.", nameof(tableId));
        }

        return new TableMergeStatus(tableId, false);
    }

    /// <summary>
    /// Creates a status for a merged table.
    /// </summary>
    /// <param name="tableId">ID of the table</param>
    /// <param name="mergedSessionId">ID of the merged session</param>
    /// <param name="primaryTableId">ID of the primary table in the merge</param>
    /// <param name="mergedTableIds">IDs of all tables in the merge</param>
    /// <param name="mergeTimestamp">When the merge occurred</param>
    /// <param name="mergeReason">Reason for the merge</param>
    /// <param name="mergedByStaffId">ID of staff who performed the merge</param>
    /// <returns>TableMergeStatus for a merged table</returns>
    public static TableMergeStatus Merged(
        Guid tableId,
        Guid mergedSessionId,
        Guid primaryTableId,
        IEnumerable<Guid> mergedTableIds,
        DateTime mergeTimestamp,
        string mergeReason,
        Guid mergedByStaffId)
    {
        if (tableId == Guid.Empty)
        {
            throw new ArgumentException("Table ID cannot be empty.", nameof(tableId));
        }

        if (mergedSessionId == Guid.Empty)
        {
            throw new ArgumentException("Merged session ID cannot be empty.", nameof(mergedSessionId));
        }

        if (primaryTableId == Guid.Empty)
        {
            throw new ArgumentException("Primary table ID cannot be empty.", nameof(primaryTableId));
        }

        if (mergedByStaffId == Guid.Empty)
        {
            throw new ArgumentException("Staff ID cannot be empty.", nameof(mergedByStaffId));
        }

        if (string.IsNullOrWhiteSpace(mergeReason))
        {
            throw new ArgumentException("Merge reason cannot be empty.", nameof(mergeReason));
        }

        return new TableMergeStatus(
            tableId,
            true,
            mergedSessionId,
            primaryTableId,
            mergedTableIds?.ToList(),
            mergeTimestamp,
            mergeReason,
            mergedByStaffId
        );
    }

    /// <summary>
    /// Checks if this table is the primary table in a merge.
    /// </summary>
    public bool IsPrimaryTable => IsMerged && PrimaryTableId == TableId;

    /// <summary>
    /// Checks if this table is a secondary table in a merge.
    /// </summary>
    public bool IsSecondaryTable => IsMerged && PrimaryTableId != TableId;

    /// <summary>
    /// Gets the number of tables in the merge (including this one).
    /// </summary>
    public int MergedTableCount => MergedTableIds?.Count ?? (IsMerged ? 1 : 0);
}
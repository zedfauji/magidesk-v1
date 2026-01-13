using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Services;

/// <summary>
/// Service interface for table operations including merge/split operations.
/// Handles table merging for large groups, table splitting with proper charge allocation,
/// and equipment/server assignment management during table operations.
/// </summary>
public interface ITableOperationsService
{
    /// <summary>
    /// Merges multiple tables into a single session for large groups.
    /// </summary>
    /// <param name="primaryTableId">ID of the primary table that will host the merged session</param>
    /// <param name="secondaryTableIds">IDs of tables to merge with the primary table</param>
    /// <param name="reason">Reason for merging tables</param>
    /// <param name="staffId">ID of the staff member performing the merge</param>
    /// <returns>Result of the table merge operation</returns>
    Task<TableOperationResult> MergeTablesAsync(
        Guid primaryTableId, 
        IEnumerable<Guid> secondaryTableIds, 
        string reason, 
        Guid staffId);

    /// <summary>
    /// Splits a merged table back into individual tables.
    /// </summary>
    /// <param name="mergedSessionId">ID of the merged session to split</param>
    /// <param name="splitAllocation">Allocation of charges to each resulting table</param>
    /// <param name="reason">Reason for splitting tables</param>
    /// <param name="staffId">ID of the staff member performing the split</param>
    /// <returns>Result of the table split operation</returns>
    Task<TableOperationResult> SplitTablesAsync(
        Guid mergedSessionId, 
        TableSplitAllocation splitAllocation, 
        string reason, 
        Guid staffId);

    /// <summary>
    /// Gets the current merge status and configuration for a table.
    /// </summary>
    /// <param name="tableId">ID of the table to check</param>
    /// <returns>Merge status information</returns>
    Task<TableMergeStatus> GetTableMergeStatusAsync(Guid tableId);

    /// <summary>
    /// Validates that tables can be merged (adjacent, available, compatible).
    /// </summary>
    /// <param name="primaryTableId">ID of the primary table</param>
    /// <param name="secondaryTableIds">IDs of tables to merge</param>
    /// <returns>Validation result with any issues identified</returns>
    Task<TableMergeValidationResult> ValidateTableMergeAsync(
        Guid primaryTableId, 
        IEnumerable<Guid> secondaryTableIds);

    /// <summary>
    /// Validates that a merged session can be split with the proposed allocation.
    /// </summary>
    /// <param name="mergedSessionId">ID of the merged session</param>
    /// <param name="splitAllocation">Proposed split allocation</param>
    /// <returns>Validation result with any issues identified</returns>
    Task<TableSplitValidationResult> ValidateTableSplitAsync(
        Guid mergedSessionId, 
        TableSplitAllocation splitAllocation);

    /// <summary>
    /// Gets audit trail for all table operations performed on a table.
    /// </summary>
    /// <param name="tableId">ID of the table</param>
    /// <param name="fromDate">Start date for audit trail</param>
    /// <param name="toDate">End date for audit trail</param>
    /// <returns>Collection of table operation audit entries</returns>
    Task<IEnumerable<TableOperationAuditEntry>> GetTableOperationAuditTrailAsync(
        Guid tableId, 
        DateTime fromDate, 
        DateTime toDate);

    /// <summary>
    /// Transfers equipment assignments during table operations.
    /// </summary>
    /// <param name="fromTableId">Source table ID</param>
    /// <param name="toTableId">Destination table ID</param>
    /// <param name="equipmentIds">IDs of equipment to transfer</param>
    /// <returns>Result of the equipment transfer</returns>
    Task<EquipmentTransferResult> TransferEquipmentAsync(
        Guid fromTableId, 
        Guid toTableId, 
        IEnumerable<Guid> equipmentIds);

    /// <summary>
    /// Manages server assignments during table merge/split operations.
    /// </summary>
    /// <param name="operationType">Type of operation (merge or split)</param>
    /// <param name="tableIds">IDs of tables involved in the operation</param>
    /// <param name="serverAssignmentStrategy">Strategy for handling server assignments</param>
    /// <returns>Result of the server assignment management</returns>
    Task<ServerAssignmentManagementResult> ManageServerAssignmentsDuringOperationAsync(
        TableOperationType operationType, 
        IEnumerable<Guid> tableIds, 
        ServerAssignmentStrategy serverAssignmentStrategy);
}
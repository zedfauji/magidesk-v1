using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Services;

/// <summary>
/// Service for table operations including merge/split operations.
/// Handles table merging for large groups, table splitting with proper charge allocation,
/// and equipment/server assignment management during table operations.
/// </summary>
public class TableOperationsService : ITableOperationsService
{
    // In-memory storage for testing purposes
    private readonly Dictionary<Guid, TableSession> _sessions = new();
    private readonly Dictionary<Guid, Table> _tables = new();
    private readonly Dictionary<Guid, TableMergeStatus> _mergeStatuses = new();
    private readonly List<TableOperationAuditEntry> _auditEntries = new();

    /// <summary>
    /// Merges multiple tables into a single session for large groups.
    /// </summary>
    public async Task<TableOperationResult> MergeTablesAsync(
        Guid primaryTableId, 
        IEnumerable<Guid> secondaryTableIds, 
        string reason, 
        Guid staffId)
    {
        try
        {
            // Validate inputs
            if (primaryTableId == Guid.Empty)
            {
                return TableOperationResult.ValidationError("Primary table ID cannot be empty");
            }

            if (staffId == Guid.Empty)
            {
                return TableOperationResult.ValidationError("Staff ID cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return TableOperationResult.ValidationError("Reason for merge cannot be empty");
            }

            var secondaryIds = secondaryTableIds?.ToList() ?? new List<Guid>();
            if (!secondaryIds.Any())
            {
                return TableOperationResult.ValidationError("At least one secondary table is required for merge");
            }

            if (secondaryIds.Contains(primaryTableId))
            {
                return TableOperationResult.ValidationError("Primary table cannot be in secondary tables list");
            }

            if (secondaryIds.Any(id => id == Guid.Empty))
            {
                return TableOperationResult.ValidationError("All secondary table IDs must be valid");
            }

            // Validate merge operation
            var validationResult = await ValidateTableMergeAsync(primaryTableId, secondaryIds);
            if (!validationResult.IsValid)
            {
                return TableOperationResult.ValidationError(string.Join("; ", validationResult.ValidationErrors));
            }

            // Calculate combined charges
            var allTableIds = new[] { primaryTableId }.Concat(secondaryIds).ToList();
            var totalChargesBefore = Money.Zero();
            var totalChargesAfter = Money.Zero();

            // For testing, simulate charge calculation
            foreach (var tableId in allTableIds)
            {
                if (_sessions.ContainsKey(tableId))
                {
                    totalChargesBefore += _sessions[tableId].TotalCharge;
                }
            }

            // Create merged session
            var mergedSessionId = Guid.NewGuid();
            totalChargesAfter = totalChargesBefore; // In merge, total charges remain the same

            // Update merge statuses
            foreach (var tableId in allTableIds)
            {
                _mergeStatuses[tableId] = TableMergeStatus.Merged(
                    tableId,
                    mergedSessionId,
                    primaryTableId,
                    allTableIds,
                    DateTime.UtcNow,
                    reason,
                    staffId
                );
            }

            // Create audit entry
            var auditEntry = TableOperationAuditEntry.Create(
                primaryTableId,
                TableOperationType.Merge,
                staffId,
                $"Staff-{staffId}",
                reason,
                TableOperationAuditData.MultipleTables(allTableIds, new List<Guid>(), totalChargesBefore, 0),
                TableOperationAuditData.SingleTable(primaryTableId, mergedSessionId, totalChargesAfter, 0)
            );
            _auditEntries.Add(auditEntry);

            var operationData = new TableOperationData(
                auditEntry.Id,
                TableOperationType.Merge,
                allTableIds,
                mergedSessionId,
                null,
                totalChargesBefore,
                totalChargesAfter,
                DateTime.UtcNow,
                staffId,
                reason
            );

            return TableOperationResult.Success(operationData);
        }
        catch (Exception ex)
        {
            return TableOperationResult.InvalidOperation($"Merge operation failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Splits a merged table back into individual tables.
    /// </summary>
    public async Task<TableOperationResult> SplitTablesAsync(
        Guid mergedSessionId, 
        TableSplitAllocation splitAllocation, 
        string reason, 
        Guid staffId)
    {
        try
        {
            // Validate inputs
            if (mergedSessionId == Guid.Empty)
            {
                return TableOperationResult.ValidationError("Merged session ID cannot be empty");
            }

            if (staffId == Guid.Empty)
            {
                return TableOperationResult.ValidationError("Staff ID cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return TableOperationResult.ValidationError("Reason for split cannot be empty");
            }

            if (splitAllocation == null)
            {
                return TableOperationResult.ValidationError("Split allocation cannot be null");
            }

            if (!splitAllocation.IsValid())
            {
                return TableOperationResult.ValidationError("Split allocation is invalid");
            }

            // Validate split operation
            var validationResult = await ValidateTableSplitAsync(mergedSessionId, splitAllocation);
            if (!validationResult.IsValid)
            {
                return TableOperationResult.ValidationError(string.Join("; ", validationResult.ValidationErrors));
            }

            // Calculate charge allocation
            var totalChargesBefore = new Money(1000m); // Simulate existing merged charge
            var totalChargesAfter = Money.Zero();

            var resultingSessionIds = new List<Guid>();
            foreach (var allocation in splitAllocation.TableAllocations.Values)
            {
                var sessionId = Guid.NewGuid();
                resultingSessionIds.Add(sessionId);
                
                var allocatedCharge = new Money(totalChargesBefore.Amount * (allocation.ChargePercentage / 100m));
                totalChargesAfter += allocatedCharge;
            }

            // Update merge statuses (clear merged status)
            var tableIds = splitAllocation.TableAllocations.Keys.ToList();
            foreach (var tableId in tableIds)
            {
                _mergeStatuses[tableId] = TableMergeStatus.NotMerged(tableId);
            }

            // Create audit entry
            var auditEntry = TableOperationAuditEntry.Create(
                tableIds.First(),
                TableOperationType.Split,
                staffId,
                $"Staff-{staffId}",
                reason,
                TableOperationAuditData.SingleTable(tableIds.First(), mergedSessionId, totalChargesBefore, 0),
                TableOperationAuditData.MultipleTables(tableIds, resultingSessionIds, totalChargesAfter, 0)
            );
            _auditEntries.Add(auditEntry);

            var operationData = new TableOperationData(
                auditEntry.Id,
                TableOperationType.Split,
                tableIds,
                null,
                resultingSessionIds,
                totalChargesBefore,
                totalChargesAfter,
                DateTime.UtcNow,
                staffId,
                reason
            );

            return TableOperationResult.Success(operationData);
        }
        catch (Exception ex)
        {
            return TableOperationResult.InvalidOperation($"Split operation failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the current merge status and configuration for a table.
    /// </summary>
    public async Task<TableMergeStatus> GetTableMergeStatusAsync(Guid tableId)
    {
        await Task.CompletedTask; // Simulate async operation
        
        if (tableId == Guid.Empty)
        {
            throw new ArgumentException("Table ID cannot be empty.", nameof(tableId));
        }

        return _mergeStatuses.TryGetValue(tableId, out var status) 
            ? status 
            : TableMergeStatus.NotMerged(tableId);
    }

    /// <summary>
    /// Validates that tables can be merged (adjacent, available, compatible).
    /// </summary>
    public async Task<TableMergeValidationResult> ValidateTableMergeAsync(
        Guid primaryTableId, 
        IEnumerable<Guid> secondaryTableIds)
    {
        await Task.CompletedTask; // Simulate async operation
        
        var errors = new List<string>();
        var warnings = new List<string>();

        // Basic validation
        if (primaryTableId == Guid.Empty)
        {
            errors.Add("Primary table ID cannot be empty");
        }

        var secondaryIds = secondaryTableIds?.ToList() ?? new List<Guid>();
        if (!secondaryIds.Any())
        {
            errors.Add("At least one secondary table is required");
        }

        if (secondaryIds.Contains(primaryTableId))
        {
            errors.Add("Primary table cannot be in secondary tables list");
        }

        if (secondaryIds.Any(id => id == Guid.Empty))
        {
            errors.Add("All secondary table IDs must be valid");
        }

        // Check for duplicate secondary tables
        if (secondaryIds.Count != secondaryIds.Distinct().Count())
        {
            errors.Add("Duplicate secondary table IDs are not allowed");
        }

        // Check if tables are already merged
        var allTableIds = new[] { primaryTableId }.Concat(secondaryIds);
        foreach (var tableId in allTableIds)
        {
            if (_mergeStatuses.TryGetValue(tableId, out var status) && status.IsMerged)
            {
                errors.Add($"Table {tableId} is already part of a merge");
            }
        }

        // Simulate additional business rule validations
        if (secondaryIds.Count > 5)
        {
            warnings.Add("Merging more than 5 tables may impact service quality");
        }

        return errors.Any() 
            ? TableMergeValidationResult.Invalid(errors, warnings)
            : TableMergeValidationResult.Valid(warnings);
    }

    /// <summary>
    /// Validates that a merged session can be split with the proposed allocation.
    /// </summary>
    public async Task<TableSplitValidationResult> ValidateTableSplitAsync(
        Guid mergedSessionId, 
        TableSplitAllocation splitAllocation)
    {
        await Task.CompletedTask; // Simulate async operation
        
        var errors = new List<string>();
        var warnings = new List<string>();

        // Basic validation
        if (mergedSessionId == Guid.Empty)
        {
            errors.Add("Merged session ID cannot be empty");
        }

        if (splitAllocation == null)
        {
            errors.Add("Split allocation cannot be null");
            return TableSplitValidationResult.Invalid(errors);
        }

        if (!splitAllocation.IsValid())
        {
            errors.Add("Split allocation percentages must sum to 100%");
        }

        // Validate individual allocations
        foreach (var allocation in splitAllocation.TableAllocations.Values)
        {
            if (allocation.GuestCount <= 0)
            {
                errors.Add($"Guest count for table {allocation.TableId} must be greater than zero");
            }

            if (allocation.ChargePercentage <= 0 || allocation.ChargePercentage > 100)
            {
                errors.Add($"Charge percentage for table {allocation.TableId} must be between 0 and 100");
            }
        }

        // Check for minimum split requirements
        if (splitAllocation.TableCount < 2)
        {
            errors.Add("Split operation requires at least 2 tables");
        }

        // Simulate business rule validations
        if (splitAllocation.TableCount > 4)
        {
            warnings.Add("Splitting into more than 4 tables may complicate billing");
        }

        return errors.Any() 
            ? TableSplitValidationResult.Invalid(errors, warnings)
            : TableSplitValidationResult.Valid(warnings);
    }

    /// <summary>
    /// Gets audit trail for all table operations performed on a table.
    /// </summary>
    public async Task<IEnumerable<TableOperationAuditEntry>> GetTableOperationAuditTrailAsync(
        Guid tableId, 
        DateTime fromDate, 
        DateTime toDate)
    {
        await Task.CompletedTask; // Simulate async operation
        
        if (tableId == Guid.Empty)
        {
            throw new ArgumentException("Table ID cannot be empty.", nameof(tableId));
        }

        if (fromDate > toDate)
        {
            throw new ArgumentException("From date cannot be after to date.", nameof(fromDate));
        }

        return _auditEntries
            .Where(entry => entry.TableId == tableId || 
                           entry.BeforeState.TableIds.Contains(tableId) || 
                           entry.AfterState.TableIds.Contains(tableId))
            .Where(entry => entry.Timestamp >= fromDate && entry.Timestamp <= toDate)
            .OrderByDescending(entry => entry.Timestamp)
            .ToList();
    }

    /// <summary>
    /// Transfers equipment assignments during table operations.
    /// </summary>
    public async Task<EquipmentTransferResult> TransferEquipmentAsync(
        Guid fromTableId, 
        Guid toTableId, 
        IEnumerable<Guid> equipmentIds)
    {
        await Task.CompletedTask; // Simulate async operation
        
        try
        {
            if (fromTableId == Guid.Empty)
            {
                return EquipmentTransferResult.ValidationError("From table ID cannot be empty");
            }

            if (toTableId == Guid.Empty)
            {
                return EquipmentTransferResult.ValidationError("To table ID cannot be empty");
            }

            if (fromTableId == toTableId)
            {
                return EquipmentTransferResult.ValidationError("Cannot transfer equipment to the same table");
            }

            var equipmentList = equipmentIds?.ToList() ?? new List<Guid>();
            if (!equipmentList.Any())
            {
                return EquipmentTransferResult.ValidationError("At least one equipment ID is required");
            }

            if (equipmentList.Any(id => id == Guid.Empty))
            {
                return EquipmentTransferResult.ValidationError("All equipment IDs must be valid");
            }

            // Simulate successful transfer
            var transferData = new EquipmentTransferData(
                fromTableId,
                toTableId,
                equipmentList,
                new List<Guid>(), // No failed transfers in this simulation
                DateTime.UtcNow
            );

            return EquipmentTransferResult.Success(transferData);
        }
        catch (Exception ex)
        {
            return EquipmentTransferResult.InvalidOperation($"Equipment transfer failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Manages server assignments during table merge/split operations.
    /// </summary>
    public async Task<ServerAssignmentManagementResult> ManageServerAssignmentsDuringOperationAsync(
        TableOperationType operationType, 
        IEnumerable<Guid> tableIds, 
        ServerAssignmentStrategy serverAssignmentStrategy)
    {
        await Task.CompletedTask; // Simulate async operation
        
        try
        {
            var tableIdList = tableIds?.ToList() ?? new List<Guid>();
            
            if (!tableIdList.Any())
            {
                return ServerAssignmentManagementResult.ValidationError("At least one table ID is required");
            }

            if (tableIdList.Any(id => id == Guid.Empty))
            {
                return ServerAssignmentManagementResult.ValidationError("All table IDs must be valid");
            }

            // Simulate server assignment management based on strategy
            var serverAssignments = new Dictionary<Guid, IReadOnlyList<Guid>>();
            
            foreach (var tableId in tableIdList)
            {
                var serverIds = serverAssignmentStrategy switch
                {
                    ServerAssignmentStrategy.KeepExisting => new List<Guid> { Guid.NewGuid() },
                    ServerAssignmentStrategy.MergeEqual => new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                    ServerAssignmentStrategy.UsePrimary => new List<Guid> { tableIdList.First() }, // Use first table ID as server ID for simulation
                    ServerAssignmentStrategy.ClearAll => new List<Guid>(),
                    _ => new List<Guid> { Guid.NewGuid() }
                };
                
                serverAssignments[tableId] = serverIds;
            }

            var managementData = new ServerAssignmentManagementData(
                operationType,
                tableIdList,
                serverAssignments,
                DateTime.UtcNow
            );

            return ServerAssignmentManagementResult.Success(managementData);
        }
        catch (Exception ex)
        {
            return ServerAssignmentManagementResult.InvalidOperation($"Server assignment management failed: {ex.Message}");
        }
    }
}
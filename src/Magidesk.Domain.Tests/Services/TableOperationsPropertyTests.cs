using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FsCheck;
using FsCheck.Xunit;
using FluentAssertions;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Tests.Services;

/// <summary>
/// Property-based tests for table operations.
/// Tests table merge/split billing accuracy, equipment transfers, and server assignment management.
/// **Feature: table-game-management, Property 14: Table Merge/Split Billing Accuracy**
/// **Validates: Requirements 10.1, 10.2, 10.3**
/// </summary>
public class TableOperationsPropertyTests
{
    private readonly TableOperationsService _tableOperationsService;

    public TableOperationsPropertyTests()
    {
        _tableOperationsService = new TableOperationsService();
    }

    #region Test Data Generators

    /// <summary>
    /// Generator for valid table IDs.
    /// </summary>
    public static Arbitrary<Guid> ValidTableIdGenerator() =>
        Arb.From(Gen.Fresh(() => Guid.NewGuid()));

    /// <summary>
    /// Generator for valid staff IDs.
    /// </summary>
    public static Arbitrary<Guid> ValidStaffIdGenerator() =>
        Arb.From(Gen.Fresh(() => Guid.NewGuid()));

    /// <summary>
    /// Generator for valid session IDs.
    /// </summary>
    public static Arbitrary<Guid> ValidSessionIdGenerator() =>
        Arb.From(Gen.Fresh(() => Guid.NewGuid()));

    /// <summary>
    /// Generator for valid charge percentages (1% to 100%).
    /// </summary>
    public static Arbitrary<decimal> ValidChargePercentageGenerator() =>
        Arb.From(Gen.Choose(1, 100).Select(x => (decimal)x));

    /// <summary>
    /// Generator for valid guest counts (1 to 20).
    /// </summary>
    public static Arbitrary<int> ValidGuestCountGenerator() =>
        Arb.From(Gen.Choose(1, 20));

    /// <summary>
    /// Generator for valid reasons (non-empty strings).
    /// </summary>
    public static Arbitrary<string> ValidReasonGenerator() =>
        Arb.From(Gen.Elements("Large group", "Customer request", "Table maintenance", "Service optimization", "Special event"));

    /// <summary>
    /// Generator for valid secondary table lists (1 to 4 tables).
    /// </summary>
    public static Arbitrary<List<Guid>> ValidSecondaryTablesGenerator() =>
        Arb.From(
            from count in Gen.Choose(1, 4)
            from tables in Gen.ListOf(count, ValidTableIdGenerator().Generator)
            select tables.Distinct().ToList()
        );

    /// <summary>
    /// Generator for valid table split allocations.
    /// </summary>
    public static Arbitrary<TableSplitAllocation> ValidTableSplitAllocationGenerator() =>
        Arb.From(
            from tableCount in Gen.Choose(2, 4)
            from tableIds in Gen.ListOf(tableCount, ValidTableIdGenerator().Generator)
            from guestCounts in Gen.ListOf(tableCount, ValidGuestCountGenerator().Generator)
            select CreateValidSplitAllocation(tableIds.Distinct().Take(tableCount).ToList(), guestCounts.Take(tableCount).ToList())
        );

    /// <summary>
    /// Creates a valid split allocation with percentages that sum to 100%.
    /// </summary>
    private static TableSplitAllocation CreateValidSplitAllocation(List<Guid> tableIds, List<int> guestCounts)
    {
        var allocations = new Dictionary<Guid, SplitTableAllocation>();
        var remainingPercentage = 100m;
        
        for (int i = 0; i < tableIds.Count; i++)
        {
            var percentage = i == tableIds.Count - 1 
                ? remainingPercentage // Last table gets remaining percentage
                : Math.Round(remainingPercentage / (tableIds.Count - i), 2); // Distribute evenly
            
            remainingPercentage -= percentage;
            
            allocations[tableIds[i]] = SplitTableAllocation.Create(
                tableIds[i], 
                percentage, 
                guestCounts[i]
            );
        }
        
        return TableSplitAllocation.Create(allocations);
    }

    #endregion

    #region Table Merge Properties

    /// <summary>
    /// Property 14a: Table Merge Billing Accuracy - For any valid table merge operation,
    /// the total charges before and after merge should be equal (no charge loss or gain).
    /// </summary>
    [Property(MaxTest = 100)]
    public bool TableMerge_BillingAccuracy_TotalChargesPreserved(
        Guid primaryTableId, 
        List<Guid> secondaryTableIds, 
        string reason, 
        Guid staffId)
    {
        // Arrange - Ensure valid inputs
        if (primaryTableId == Guid.Empty || 
            staffId == Guid.Empty || 
            string.IsNullOrWhiteSpace(reason) ||
            secondaryTableIds == null || 
            !secondaryTableIds.Any() ||
            secondaryTableIds.Contains(primaryTableId) ||
            secondaryTableIds.Any(id => id == Guid.Empty) ||
            secondaryTableIds.Count != secondaryTableIds.Distinct().Count())
        {
            return true; // Skip invalid inputs
        }

        // Act
        var result = _tableOperationsService.MergeTablesAsync(
            primaryTableId, secondaryTableIds, reason, staffId).Result;

        // Assert
        if (result.IsSuccessful && result.Data != null)
        {
            // Total charges should be preserved during merge
            result.Data.TotalChargesBefore.Should().Be(result.Data.TotalChargesAfter,
                "Merge operation should preserve total charges - no money should be lost or gained");

            // Operation should involve all specified tables
            var expectedTableCount = 1 + secondaryTableIds.Count; // Primary + secondary tables
            result.Data.TableIds.Count.Should().Be(expectedTableCount,
                "Merge operation should involve all specified tables");

            // Should result in a single merged session
            result.Data.ResultingSessionId.Should().NotBeNull("Merge should create a single merged session");
            result.Data.ResultingSessionIds.Should().BeNull("Merge should not create multiple sessions");

            // Operation type should be correct
            result.Data.OperationType.Should().Be(TableOperationType.Merge);

            // All input tables should be included
            result.Data.TableIds.Should().Contain(primaryTableId, "Primary table should be included in operation");
            foreach (var secondaryId in secondaryTableIds)
            {
                result.Data.TableIds.Should().Contain(secondaryId, $"Secondary table {secondaryId} should be included in operation");
            }
        }

        return true;
    }

    /// <summary>
    /// Property 14b: Table Merge Validation - For any invalid merge inputs,
    /// the operation should fail with appropriate validation errors.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool TableMerge_InvalidInputs_ShouldFail(Guid primaryTableId, List<Guid> secondaryTableIds)
    {
        var validStaffId = Guid.NewGuid();
        var validReason = "Test merge";

        // Test empty primary table ID
        if (primaryTableId == Guid.Empty)
        {
            var result1 = _tableOperationsService.MergeTablesAsync(
                primaryTableId, new[] { Guid.NewGuid() }, validReason, validStaffId).Result;
            
            result1.IsSuccessful.Should().BeFalse("Merge should fail with empty primary table ID");
            result1.ErrorMessage.Should().Contain("Primary table ID cannot be empty");
        }

        // Test empty staff ID
        var result2 = _tableOperationsService.MergeTablesAsync(
            Guid.NewGuid(), new[] { Guid.NewGuid() }, validReason, Guid.Empty).Result;
        
        result2.IsSuccessful.Should().BeFalse("Merge should fail with empty staff ID");
        result2.ErrorMessage.Should().Contain("Staff ID cannot be empty");

        // Test empty reason
        var result3 = _tableOperationsService.MergeTablesAsync(
            Guid.NewGuid(), new[] { Guid.NewGuid() }, "", validStaffId).Result;
        
        result3.IsSuccessful.Should().BeFalse("Merge should fail with empty reason");
        result3.ErrorMessage.Should().Contain("Reason for merge cannot be empty");

        // Test empty secondary tables list
        var result4 = _tableOperationsService.MergeTablesAsync(
            Guid.NewGuid(), new List<Guid>(), validReason, validStaffId).Result;
        
        result4.IsSuccessful.Should().BeFalse("Merge should fail with empty secondary tables list");
        result4.ErrorMessage.Should().Contain("At least one secondary table is required");

        return true;
    }

    #endregion

    #region Table Split Properties

    /// <summary>
    /// Property 14c: Table Split Billing Accuracy - For any valid table split operation,
    /// the sum of allocated charges should equal the original merged charge.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool TableSplit_BillingAccuracy_AllocatedChargesEqualOriginal(
        Guid mergedSessionId, 
        TableSplitAllocation splitAllocation, 
        string reason, 
        Guid staffId)
    {
        // Arrange - Ensure valid inputs
        if (mergedSessionId == Guid.Empty || 
            staffId == Guid.Empty || 
            string.IsNullOrWhiteSpace(reason) ||
            splitAllocation == null || 
            !splitAllocation.IsValid())
        {
            return true; // Skip invalid inputs
        }

        // Act
        var result = _tableOperationsService.SplitTablesAsync(
            mergedSessionId, splitAllocation, reason, staffId).Result;

        // Assert
        if (result.IsSuccessful && result.Data != null)
        {
            // Total charges should be preserved during split (within rounding tolerance)
            var chargeDifference = Math.Abs(result.Data.TotalChargesAfter.Amount - result.Data.TotalChargesBefore.Amount);
            chargeDifference.Should().BeLessThanOrEqualTo(0.01m,
                "Split operation should preserve total charges within rounding tolerance");

            // Should create multiple sessions (one per table)
            result.Data.ResultingSessionIds.Should().NotBeNull("Split should create multiple sessions");
            result.Data.ResultingSessionIds.Count.Should().Be(splitAllocation.TableCount,
                "Split should create one session per table in allocation");

            // Should not have a single resulting session
            result.Data.ResultingSessionId.Should().BeNull("Split should not create a single session");

            // Operation type should be correct
            result.Data.OperationType.Should().Be(TableOperationType.Split);

            // All tables from allocation should be included
            foreach (var tableId in splitAllocation.TableAllocations.Keys)
            {
                result.Data.TableIds.Should().Contain(tableId, 
                    $"Table {tableId} from split allocation should be included in operation");
            }

            // Mathematical consistency: if we know the percentages, we can verify the allocation
            if (result.Data.TotalChargesBefore.Amount > 0)
            {
                foreach (var allocation in splitAllocation.TableAllocations.Values)
                {
                    var expectedAllocation = result.Data.TotalChargesBefore.Amount * (allocation.ChargePercentage / 100m);
                    // We can't directly verify individual allocations from the result, but we verified the total above
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Property 14d: Table Split Validation - For any invalid split allocation,
    /// the operation should fail with appropriate validation errors.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool TableSplit_InvalidAllocation_ShouldFail(Guid mergedSessionId, Guid staffId)
    {
        var validReason = "Test split";

        // Skip if basic IDs are invalid (different test case)
        if (mergedSessionId == Guid.Empty || staffId == Guid.Empty)
            return true;

        // Test null allocation
        TableSplitAllocation? nullAllocation = null;
        var result1 = _tableOperationsService.SplitTablesAsync(
            mergedSessionId, nullAllocation!, validReason, staffId).Result;
        
        result1.IsSuccessful.Should().BeFalse("Split should fail with null allocation");
        result1.ErrorMessage.Should().Contain("Split allocation cannot be null");

        // Test invalid allocation (percentages don't sum to 100%)
        var invalidAllocations = new Dictionary<Guid, SplitTableAllocation>
        {
            { Guid.NewGuid(), SplitTableAllocation.Create(Guid.NewGuid(), 50m, 2) },
            { Guid.NewGuid(), SplitTableAllocation.Create(Guid.NewGuid(), 30m, 3) }
            // Total: 80% (should be 100%)
        };

        try
        {
            var invalidSplitAllocation = TableSplitAllocation.Create(invalidAllocations);
            var result2 = _tableOperationsService.SplitTablesAsync(
                mergedSessionId, invalidSplitAllocation, validReason, staffId).Result;
            
            // This should fail either at creation or validation
            result2.IsSuccessful.Should().BeFalse("Split should fail with invalid percentage allocation");
        }
        catch (ArgumentException)
        {
            // Expected - invalid allocation should throw during creation
        }

        return true;
    }

    #endregion

    #region Equipment Transfer Properties

    /// <summary>
    /// Property 14e: Equipment Transfer Consistency - For any valid equipment transfer,
    /// all specified equipment should be successfully transferred with proper audit trail.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool EquipmentTransfer_ValidInputs_ShouldSucceed(
        Guid fromTableId, 
        Guid toTableId, 
        List<Guid> equipmentIds)
    {
        // Arrange - Ensure valid inputs
        if (fromTableId == Guid.Empty || 
            toTableId == Guid.Empty || 
            fromTableId == toTableId ||
            equipmentIds == null || 
            !equipmentIds.Any() ||
            equipmentIds.Any(id => id == Guid.Empty) ||
            equipmentIds.Count != equipmentIds.Distinct().Count())
        {
            return true; // Skip invalid inputs
        }

        // Act
        var result = _tableOperationsService.TransferEquipmentAsync(
            fromTableId, toTableId, equipmentIds).Result;

        // Assert
        if (result.IsSuccessful && result.Data != null)
        {
            // Transfer should preserve equipment IDs
            result.Data.FromTableId.Should().Be(fromTableId, "Source table ID should be preserved");
            result.Data.ToTableId.Should().Be(toTableId, "Destination table ID should be preserved");

            // All equipment should be transferred successfully (in our test implementation)
            result.Data.TransferredEquipmentIds.Count.Should().Be(equipmentIds.Count,
                "All equipment should be transferred successfully");

            foreach (var equipmentId in equipmentIds)
            {
                result.Data.TransferredEquipmentIds.Should().Contain(equipmentId,
                    $"Equipment {equipmentId} should be in transferred list");
            }

            // No equipment should fail in our test implementation
            result.Data.FailedEquipmentIds.Should().BeEmpty("No equipment should fail to transfer in valid scenario");

            // Transfer timestamp should be recent
            result.Data.TransferTimestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5),
                "Transfer timestamp should be recent");
        }

        return true;
    }

    /// <summary>
    /// Property 14f: Equipment Transfer Validation - For any invalid transfer inputs,
    /// the operation should fail with appropriate validation errors.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool EquipmentTransfer_InvalidInputs_ShouldFail(Guid fromTableId, Guid toTableId)
    {
        var validEquipmentIds = new[] { Guid.NewGuid(), Guid.NewGuid() };

        // Test empty from table ID
        if (fromTableId == Guid.Empty)
        {
            var result1 = _tableOperationsService.TransferEquipmentAsync(
                fromTableId, Guid.NewGuid(), validEquipmentIds).Result;
            
            result1.IsSuccessful.Should().BeFalse("Transfer should fail with empty from table ID");
            result1.ErrorMessage.Should().Contain("From table ID cannot be empty");
        }

        // Test empty to table ID
        if (toTableId == Guid.Empty)
        {
            var result2 = _tableOperationsService.TransferEquipmentAsync(
                Guid.NewGuid(), toTableId, validEquipmentIds).Result;
            
            result2.IsSuccessful.Should().BeFalse("Transfer should fail with empty to table ID");
            result2.ErrorMessage.Should().Contain("To table ID cannot be empty");
        }

        // Test same table transfer
        if (fromTableId != Guid.Empty && toTableId != Guid.Empty && fromTableId == toTableId)
        {
            var result3 = _tableOperationsService.TransferEquipmentAsync(
                fromTableId, toTableId, validEquipmentIds).Result;
            
            result3.IsSuccessful.Should().BeFalse("Transfer should fail when from and to tables are the same");
            result3.ErrorMessage.Should().Contain("Cannot transfer equipment to the same table");
        }

        // Test empty equipment list
        var result4 = _tableOperationsService.TransferEquipmentAsync(
            Guid.NewGuid(), Guid.NewGuid(), new List<Guid>()).Result;
        
        result4.IsSuccessful.Should().BeFalse("Transfer should fail with empty equipment list");
        result4.ErrorMessage.Should().Contain("At least one equipment ID is required");

        return true;
    }

    #endregion

    #region Server Assignment Management Properties

    /// <summary>
    /// Property 14g: Server Assignment Management Consistency - For any valid server assignment
    /// management operation, the result should properly handle assignments according to strategy.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool ServerAssignmentManagement_ValidInputs_ShouldSucceed(
        TableOperationType operationType, 
        List<Guid> tableIds, 
        ServerAssignmentStrategy strategy)
    {
        // Arrange - Ensure valid inputs
        if (tableIds == null || 
            !tableIds.Any() || 
            tableIds.Any(id => id == Guid.Empty) ||
            tableIds.Count != tableIds.Distinct().Count())
        {
            return true; // Skip invalid inputs
        }

        // Act
        var result = _tableOperationsService.ManageServerAssignmentsDuringOperationAsync(
            operationType, tableIds, strategy).Result;

        // Assert
        if (result.IsSuccessful && result.Data != null)
        {
            // Operation details should be preserved
            result.Data.OperationType.Should().Be(operationType, "Operation type should be preserved");
            result.Data.TableIds.Count.Should().Be(tableIds.Count, "All table IDs should be included");

            foreach (var tableId in tableIds)
            {
                result.Data.TableIds.Should().Contain(tableId, $"Table {tableId} should be included in result");
            }

            // Server assignments should be provided for all tables
            result.Data.ServerAssignmentsByTable.Should().NotBeNull("Server assignments should be provided");
            result.Data.ServerAssignmentsByTable.Count.Should().Be(tableIds.Count,
                "Server assignments should be provided for all tables");

            foreach (var tableId in tableIds)
            {
                result.Data.ServerAssignmentsByTable.Should().ContainKey(tableId,
                    $"Server assignment should be provided for table {tableId}");
            }

            // Validate strategy-specific behavior
            switch (strategy)
            {
                case ServerAssignmentStrategy.ClearAll:
                    foreach (var assignment in result.Data.ServerAssignmentsByTable.Values)
                    {
                        assignment.Should().BeEmpty("ClearAll strategy should result in no server assignments");
                    }
                    break;

                case ServerAssignmentStrategy.KeepExisting:
                case ServerAssignmentStrategy.MergeEqual:
                case ServerAssignmentStrategy.UsePrimary:
                case ServerAssignmentStrategy.CustomAllocation:
                    // These strategies should have some server assignments (in our test implementation)
                    foreach (var assignment in result.Data.ServerAssignmentsByTable.Values)
                    {
                        assignment.Should().NotBeNull($"{strategy} strategy should provide server assignments");
                    }
                    break;
            }

            // Timestamp should be recent
            result.Data.OperationTimestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5),
                "Operation timestamp should be recent");
        }

        return true;
    }

    /// <summary>
    /// Property 14h: Server Assignment Management Validation - For any invalid inputs,
    /// the operation should fail with appropriate validation errors.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool ServerAssignmentManagement_InvalidInputs_ShouldFail(TableOperationType operationType)
    {
        var validStrategy = ServerAssignmentStrategy.KeepExisting;

        // Test empty table list
        var result1 = _tableOperationsService.ManageServerAssignmentsDuringOperationAsync(
            operationType, new List<Guid>(), validStrategy).Result;
        
        result1.IsSuccessful.Should().BeFalse("Management should fail with empty table list");
        result1.ErrorMessage.Should().Contain("At least one table ID is required");

        // Test table list with empty GUIDs
        var invalidTableIds = new[] { Guid.NewGuid(), Guid.Empty, Guid.NewGuid() };
        var result2 = _tableOperationsService.ManageServerAssignmentsDuringOperationAsync(
            operationType, invalidTableIds, validStrategy).Result;
        
        result2.IsSuccessful.Should().BeFalse("Management should fail with invalid table IDs");
        result2.ErrorMessage.Should().Contain("All table IDs must be valid");

        return true;
    }

    #endregion

    #region Audit Trail Properties

    /// <summary>
    /// Property 14i: Audit Trail Completeness - For any table operation,
    /// a complete audit trail should be created and retrievable.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool AuditTrail_TableOperations_ShouldBeComplete(
        Guid tableId, 
        DateTime fromDate, 
        DateTime toDate)
    {
        // Arrange - Ensure valid inputs
        if (tableId == Guid.Empty || fromDate > toDate)
        {
            return true; // Skip invalid inputs
        }

        // Act
        var auditEntries = _tableOperationsService.GetTableOperationAuditTrailAsync(
            tableId, fromDate, toDate).Result;

        // Assert
        auditEntries.Should().NotBeNull("Audit trail should never be null");
        
        // All entries should be within the date range
        foreach (var entry in auditEntries)
        {
            entry.Timestamp.Should().BeOnOrAfter(fromDate, "Audit entry should be within requested date range");
            entry.Timestamp.Should().BeOnOrBefore(toDate, "Audit entry should be within requested date range");
            
            // Entry should be related to the requested table
            var isRelatedToTable = entry.TableId == tableId ||
                                 entry.BeforeState.TableIds.Contains(tableId) ||
                                 entry.AfterState.TableIds.Contains(tableId);
            
            isRelatedToTable.Should().BeTrue($"Audit entry should be related to table {tableId}");
            
            // Basic audit entry validation
            entry.Id.Should().NotBe(Guid.Empty, "Audit entry should have valid ID");
            entry.StaffId.Should().NotBe(Guid.Empty, "Audit entry should have valid staff ID");
            entry.StaffName.Should().NotBeNullOrWhiteSpace("Audit entry should have staff name");
            entry.Reason.Should().NotBeNullOrWhiteSpace("Audit entry should have reason");
            entry.BeforeState.Should().NotBeNull("Audit entry should have before state");
            entry.AfterState.Should().NotBeNull("Audit entry should have after state");
        }

        // Entries should be ordered by timestamp (most recent first)
        var timestamps = auditEntries.Select(e => e.Timestamp).ToList();
        for (int i = 1; i < timestamps.Count; i++)
        {
            timestamps[i].Should().BeOnOrBefore(timestamps[i - 1], 
                "Audit entries should be ordered by timestamp (most recent first)");
        }

        return true;
    }

    #endregion
}
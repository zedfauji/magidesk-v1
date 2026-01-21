using System;
using System.Collections.Generic;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Audit entry for table operations (merge, split, transfer).
/// </summary>
public record TableOperationAuditEntry(
    Guid Id,
    Guid TableId,
    TableOperationType OperationType,
    DateTime Timestamp,
    Guid StaffId,
    string StaffName,
    string Reason,
    TableOperationAuditData BeforeState,
    TableOperationAuditData AfterState,
    IReadOnlyDictionary<string, object> AdditionalData
)
{
    /// <summary>
    /// Creates a new table operation audit entry.
    /// </summary>
    /// <param name="tableId">ID of the primary table involved</param>
    /// <param name="operationType">Type of operation performed</param>
    /// <param name="staffId">ID of staff who performed the operation</param>
    /// <param name="staffName">Name of staff who performed the operation</param>
    /// <param name="reason">Reason for the operation</param>
    /// <param name="beforeState">State before the operation</param>
    /// <param name="afterState">State after the operation</param>
    /// <param name="additionalData">Additional operation-specific data</param>
    /// <returns>New TableOperationAuditEntry</returns>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
    public static TableOperationAuditEntry Create(
        Guid tableId,
        TableOperationType operationType,
        Guid staffId,
        string staffName,
        string reason,
        TableOperationAuditData beforeState,
        TableOperationAuditData afterState,
        IDictionary<string, object>? additionalData = null)
    {
        if (tableId == Guid.Empty)
        {
            throw new ArgumentException("Table ID cannot be empty.", nameof(tableId));
        }

        if (staffId == Guid.Empty)
        {
            throw new ArgumentException("Staff ID cannot be empty.", nameof(staffId));
        }

        if (string.IsNullOrWhiteSpace(staffName))
        {
            throw new ArgumentException("Staff name cannot be empty.", nameof(staffName));
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason cannot be empty.", nameof(reason));
        }

        if (beforeState == null)
        {
            throw new ArgumentNullException(nameof(beforeState));
        }

        if (afterState == null)
        {
            throw new ArgumentNullException(nameof(afterState));
        }

        return new TableOperationAuditEntry(
            Guid.NewGuid(),
            tableId,
            operationType,
            DateTime.UtcNow,
            staffId,
            staffName,
            reason,
            beforeState,
            afterState,
            additionalData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, object>()
        );
    }

    /// <summary>
    /// Gets a summary description of the operation.
    /// </summary>
    public string OperationSummary => $"{OperationType} operation on table {TableId} by {StaffName} at {Timestamp:yyyy-MM-dd HH:mm:ss} UTC";

    /// <summary>
    /// Checks if the operation resulted in a charge change.
    /// </summary>
    public bool HasChargeChange => BeforeState.TotalCharge != AfterState.TotalCharge;

    /// <summary>
    /// Gets the charge difference (after - before).
    /// </summary>
    public Money ChargeDifference => AfterState.TotalCharge - BeforeState.TotalCharge;
}

/// <summary>
/// Audit data representing the state of tables before or after an operation.
/// </summary>
public record TableOperationAuditData(
    IReadOnlyList<Guid> TableIds,
    IReadOnlyList<Guid> SessionIds,
    Money TotalCharge,
    int TotalGuestCount,
    IReadOnlyList<Guid> EquipmentIds,
    IReadOnlyList<Guid> ServerIds,
    IReadOnlyDictionary<string, object> StateData
)
{
    /// <summary>
    /// Creates audit data for a single table state.
    /// </summary>
    /// <param name="tableId">ID of the table</param>
    /// <param name="sessionId">ID of the session (if any)</param>
    /// <param name="charge">Current charge</param>
    /// <param name="guestCount">Number of guests</param>
    /// <param name="equipmentIds">Equipment assigned to the table</param>
    /// <param name="serverIds">Servers assigned to the table</param>
    /// <param name="stateData">Additional state information</param>
    /// <returns>New TableOperationAuditData</returns>
    public static TableOperationAuditData SingleTable(
        Guid tableId,
        Guid? sessionId,
        Money charge,
        int guestCount,
        IEnumerable<Guid>? equipmentIds = null,
        IEnumerable<Guid>? serverIds = null,
        IDictionary<string, object>? stateData = null)
    {
        return new TableOperationAuditData(
            new[] { tableId },
            sessionId.HasValue ? new[] { sessionId.Value } : Array.Empty<Guid>(),
            charge,
            guestCount,
            equipmentIds?.ToList() ?? new List<Guid>(),
            serverIds?.ToList() ?? new List<Guid>(),
            stateData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, object>()
        );
    }

    /// <summary>
    /// Creates audit data for multiple tables state.
    /// </summary>
    /// <param name="tableIds">IDs of the tables</param>
    /// <param name="sessionIds">IDs of the sessions</param>
    /// <param name="totalCharge">Combined charge across all tables</param>
    /// <param name="totalGuestCount">Combined guest count</param>
    /// <param name="equipmentIds">All equipment assigned</param>
    /// <param name="serverIds">All servers assigned</param>
    /// <param name="stateData">Additional state information</param>
    /// <returns>New TableOperationAuditData</returns>
    public static TableOperationAuditData MultipleTables(
        IEnumerable<Guid> tableIds,
        IEnumerable<Guid> sessionIds,
        Money totalCharge,
        int totalGuestCount,
        IEnumerable<Guid>? equipmentIds = null,
        IEnumerable<Guid>? serverIds = null,
        IDictionary<string, object>? stateData = null)
    {
        return new TableOperationAuditData(
            tableIds?.ToList() ?? new List<Guid>(),
            sessionIds?.ToList() ?? new List<Guid>(),
            totalCharge,
            totalGuestCount,
            equipmentIds?.ToList() ?? new List<Guid>(),
            serverIds?.ToList() ?? new List<Guid>(),
            stateData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, object>()
        );
    }

    /// <summary>
    /// Gets the number of tables in this state.
    /// </summary>
    public int TableCount => TableIds.Count;

    /// <summary>
    /// Gets the number of active sessions in this state.
    /// </summary>
    public int SessionCount => SessionIds.Count;

    /// <summary>
    /// Checks if this represents a merged table state.
    /// </summary>
    public bool IsMergedState => TableIds.Count > 1 && SessionIds.Count == 1;

    /// <summary>
    /// Checks if this represents individual table states.
    /// </summary>
    public bool IsIndividualState => TableIds.Count == SessionIds.Count;
}
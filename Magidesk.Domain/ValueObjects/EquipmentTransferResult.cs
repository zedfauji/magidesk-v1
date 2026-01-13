using System;
using System.Collections.Generic;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Result of an equipment transfer operation between tables.
/// </summary>
public record EquipmentTransferResult(
    bool IsSuccessful,
    string? ErrorMessage = null,
    EquipmentTransferData? Data = null
)
{
    public static EquipmentTransferResult Success(EquipmentTransferData? data = null) => 
        new(true, null, data);
    
    public static EquipmentTransferResult NotFound(string entityType = "Equipment") => 
        new(false, $"{entityType} not found");
    
    public static EquipmentTransferResult InvalidOperation(string message) => 
        new(false, message);
    
    public static EquipmentTransferResult ValidationError(string message) => 
        new(false, message);
}

/// <summary>
/// Data associated with an equipment transfer result.
/// </summary>
public record EquipmentTransferData(
    Guid FromTableId,
    Guid ToTableId,
    IReadOnlyList<Guid> TransferredEquipmentIds,
    IReadOnlyList<Guid> FailedEquipmentIds,
    DateTime TransferTimestamp
);

/// <summary>
/// Result of managing server assignments during table operations.
/// </summary>
public record ServerAssignmentManagementResult(
    bool IsSuccessful,
    string? ErrorMessage = null,
    ServerAssignmentManagementData? Data = null
)
{
    public static ServerAssignmentManagementResult Success(ServerAssignmentManagementData? data = null) => 
        new(true, null, data);
    
    public static ServerAssignmentManagementResult InvalidOperation(string message) => 
        new(false, message);
    
    public static ServerAssignmentManagementResult ValidationError(string message) => 
        new(false, message);
}

/// <summary>
/// Data associated with server assignment management result.
/// </summary>
public record ServerAssignmentManagementData(
    TableOperationType OperationType,
    IReadOnlyList<Guid> TableIds,
    IReadOnlyDictionary<Guid, IReadOnlyList<Guid>> ServerAssignmentsByTable,
    DateTime OperationTimestamp
);

/// <summary>
/// Strategy for handling server assignments during table operations.
/// </summary>
public enum ServerAssignmentStrategy
{
    /// <summary>
    /// Keep existing server assignments unchanged.
    /// </summary>
    KeepExisting,
    
    /// <summary>
    /// Merge all servers from all tables with equal allocation.
    /// </summary>
    MergeEqual,
    
    /// <summary>
    /// Use primary table's servers for all tables.
    /// </summary>
    UsePrimary,
    
    /// <summary>
    /// Reassign servers based on custom allocation rules.
    /// </summary>
    CustomAllocation,
    
    /// <summary>
    /// Clear all server assignments and require manual reassignment.
    /// </summary>
    ClearAll
}
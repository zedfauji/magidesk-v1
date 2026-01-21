using System;
using System.Collections.Generic;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Result of a table operation (merge or split).
/// </summary>
public record TableOperationResult(
    bool IsSuccessful,
    string? ErrorMessage = null,
    TableOperationData? Data = null
)
{
    public static TableOperationResult Success(TableOperationData? data = null) => 
        new(true, null, data);
    
    public static TableOperationResult NotFound(string entityType = "Table") => 
        new(false, $"{entityType} not found");
    
    public static TableOperationResult InvalidOperation(string message) => 
        new(false, message);
    
    public static TableOperationResult ValidationError(string message) => 
        new(false, message);
    
    public static TableOperationResult Unauthorized(string message = "Unauthorized operation") => 
        new(false, message);
}

/// <summary>
/// Data associated with a table operation result.
/// </summary>
public record TableOperationData(
    Guid OperationId,
    TableOperationType OperationType,
    IReadOnlyList<Guid> TableIds,
    Guid? ResultingSessionId,
    IReadOnlyList<Guid>? ResultingSessionIds,
    Money TotalChargesBefore,
    Money TotalChargesAfter,
    DateTime OperationTimestamp,
    Guid StaffId,
    string Reason
);

/// <summary>
/// Type of table operation.
/// </summary>
public enum TableOperationType
{
    Merge,
    Split,
    Transfer
}
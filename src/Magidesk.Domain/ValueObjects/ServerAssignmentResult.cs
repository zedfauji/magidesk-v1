using System;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Result of a server assignment operation.
/// </summary>
public record ServerAssignmentResult(
    bool IsSuccessful,
    string? ErrorMessage = null,
    ServerAssignmentData? Data = null
)
{
    public static ServerAssignmentResult Success(ServerAssignmentData? data = null) => 
        new(true, null, data);
    
    public static ServerAssignmentResult NotFound(string entityType = "Session") => 
        new(false, $"{entityType} not found");
    
    public static ServerAssignmentResult InvalidOperation(string message) => 
        new(false, message);
    
    public static ServerAssignmentResult ValidationError(string message) => 
        new(false, message);
}

/// <summary>
/// Data associated with a server assignment result.
/// </summary>
public record ServerAssignmentData(
    Guid AssignmentId,
    Guid SessionId,
    Guid ServerId,
    bool IsPrimary,
    decimal AllocationPercentage,
    DateTime AssignedAt
);
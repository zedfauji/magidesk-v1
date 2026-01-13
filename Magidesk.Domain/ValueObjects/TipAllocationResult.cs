using System;
using System.Collections.Generic;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Result of tip allocation calculation for a session.
/// </summary>
public record TipAllocationResult(
    Guid SessionId,
    Money TotalTipAmount,
    IReadOnlyList<ServerTipAllocation> Allocations,
    bool IsValid = true,
    string? ValidationMessage = null
)
{
    public static TipAllocationResult Success(
        Guid sessionId, 
        Money totalTipAmount, 
        IReadOnlyList<ServerTipAllocation> allocations) =>
        new(sessionId, totalTipAmount, allocations);
    
    public static TipAllocationResult ValidationError(
        Guid sessionId, 
        Money totalTipAmount, 
        string validationMessage) =>
        new(sessionId, totalTipAmount, Array.Empty<ServerTipAllocation>(), false, validationMessage);
}

/// <summary>
/// Tip allocation for an individual server.
/// </summary>
public record ServerTipAllocation(
    Guid ServerId,
    string ServerName,
    decimal AllocationPercentage,
    Money AllocatedAmount,
    bool IsPrimary
);
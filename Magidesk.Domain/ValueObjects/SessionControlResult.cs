using System;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Result of a session control operation.
/// </summary>
public record SessionControlResult(
    bool IsSuccessful,
    string? ErrorMessage = null,
    SessionControlData? Data = null
)
{
    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static SessionControlResult Success(SessionControlData? data = null) => 
        new(true, null, data);
    
    /// <summary>
    /// Creates a result for session not found.
    /// </summary>
    public static SessionControlResult NotFound() => 
        new(false, "Session not found");
    
    /// <summary>
    /// Creates a result for invalid session state.
    /// </summary>
    public static SessionControlResult InvalidState(string message) => 
        new(false, message);
    
    /// <summary>
    /// Creates a result for authorization failure.
    /// </summary>
    public static SessionControlResult Unauthorized(string message = "Unauthorized operation") => 
        new(false, message);
    
    /// <summary>
    /// Creates a result for validation failure.
    /// </summary>
    public static SessionControlResult ValidationError(string message) => 
        new(false, message);
}

/// <summary>
/// Data associated with a session control operation result.
/// </summary>
public record SessionControlData(
    Guid SessionId,
    TableSessionStatus Status,
    DateTime? PausedAt,
    TimeSpan TotalPausedDuration,
    Money CurrentCharge
);
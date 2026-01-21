using System;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Represents the result of a manager override operation.
/// Immutable value object containing operation status and optional data.
/// </summary>
public sealed record OverrideResult(
    bool IsSuccessful,
    string? ErrorMessage = null,
    OverrideData? Data = null
)
{
    /// <summary>
    /// Creates a successful result with optional data.
    /// </summary>
    /// <param name="data">Optional override data</param>
    /// <returns>Successful OverrideResult</returns>
    public static OverrideResult Success(OverrideData? data = null) => 
        new(true, null, data);
    
    /// <summary>
    /// Creates a result indicating unauthorized access.
    /// </summary>
    /// <returns>Unauthorized OverrideResult</returns>
    public static OverrideResult Unauthorized() => 
        new(false, "Manager authorization required");
    
    /// <summary>
    /// Creates a result indicating the session was not found.
    /// </summary>
    /// <returns>Not found OverrideResult</returns>
    public static OverrideResult NotFound() => 
        new(false, "Session not found");

    /// <summary>
    /// Creates a result indicating an invalid operation.
    /// </summary>
    /// <param name="message">Error message describing why the operation is invalid</param>
    /// <returns>Invalid operation OverrideResult</returns>
    public static OverrideResult InvalidOperation(string message) =>
        new(false, message);

    /// <summary>
    /// Creates a result indicating a validation error.
    /// </summary>
    /// <param name="message">Validation error message</param>
    /// <returns>Validation error OverrideResult</returns>
    public static OverrideResult ValidationError(string message) =>
        new(false, $"Validation error: {message}");
}

/// <summary>
/// Contains data about a manager override operation.
/// </summary>
public sealed record OverrideData(
    Guid SessionId,
    OverrideType OverrideType,
    string OriginalValue,
    string NewValue,
    Guid ManagerId,
    DateTime Timestamp
)
{
    /// <summary>
    /// Creates override data for an operation.
    /// </summary>
    /// <param name="sessionId">Session ID</param>
    /// <param name="overrideType">Type of override performed</param>
    /// <param name="originalValue">Original value before override</param>
    /// <param name="newValue">New value after override</param>
    /// <param name="managerId">ID of the manager who performed the override</param>
    /// <returns>New OverrideData instance</returns>
    public static OverrideData Create(
        Guid sessionId,
        OverrideType overrideType,
        string originalValue,
        string newValue,
        Guid managerId)
    {
        return new OverrideData(
            SessionId: sessionId,
            OverrideType: overrideType,
            OriginalValue: originalValue,
            NewValue: newValue,
            ManagerId: managerId,
            Timestamp: DateTime.UtcNow
        );
    }
}
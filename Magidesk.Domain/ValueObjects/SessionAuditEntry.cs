using System;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Represents an immutable audit entry for session actions.
/// </summary>
public record SessionAuditEntry(
    Guid SessionId,
    string Action,
    string Details,
    Guid UserId,
    DateTime Timestamp
)
{
    /// <summary>
    /// Creates a new session audit entry for the current time.
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <param name="action">Action performed</param>
    /// <param name="details">Additional details</param>
    /// <param name="userId">ID of the user who performed the action</param>
    /// <returns>New SessionAuditEntry</returns>
    public static SessionAuditEntry Create(Guid sessionId, string action, string details, Guid userId)
    {
        return new SessionAuditEntry(sessionId, action, details, userId, DateTime.UtcNow);
    }

    /// <summary>
    /// Creates an audit entry for session start.
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <param name="userId">ID of the user</param>
    /// <param name="tableId">ID of the table</param>
    /// <param name="guestCount">Number of guests</param>
    /// <returns>Session start audit entry</returns>
    public static SessionAuditEntry SessionStarted(Guid sessionId, Guid userId, Guid tableId, int guestCount)
    {
        return Create(sessionId, "SessionStarted", $"Table: {tableId}, Guests: {guestCount}", userId);
    }

    /// <summary>
    /// Creates an audit entry for session pause.
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <param name="userId">ID of the user</param>
    /// <param name="reason">Reason for pause</param>
    /// <returns>Session pause audit entry</returns>
    public static SessionAuditEntry SessionPaused(Guid sessionId, Guid userId, string reason)
    {
        return Create(sessionId, "SessionPaused", $"Reason: {reason}", userId);
    }

    /// <summary>
    /// Creates an audit entry for session resume.
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <param name="userId">ID of the user</param>
    /// <returns>Session resume audit entry</returns>
    public static SessionAuditEntry SessionResumed(Guid sessionId, Guid userId)
    {
        return Create(sessionId, "SessionResumed", "Session resumed", userId);
    }

    /// <summary>
    /// Creates an audit entry for session end.
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <param name="userId">ID of the user</param>
    /// <param name="totalCharge">Total charge for the session</param>
    /// <param name="duration">Session duration</param>
    /// <returns>Session end audit entry</returns>
    public static SessionAuditEntry SessionEnded(Guid sessionId, Guid userId, decimal totalCharge, TimeSpan duration)
    {
        return Create(sessionId, "SessionEnded", $"Duration: {duration}, Charge: {totalCharge:C}", userId);
    }

    /// <summary>
    /// Creates an audit entry for guest count update.
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <param name="userId">ID of the user</param>
    /// <param name="oldCount">Previous guest count</param>
    /// <param name="newCount">New guest count</param>
    /// <returns>Guest count update audit entry</returns>
    public static SessionAuditEntry GuestCountUpdated(Guid sessionId, Guid userId, int oldCount, int newCount)
    {
        return Create(sessionId, "GuestCountUpdated", $"From: {oldCount}, To: {newCount}", userId);
    }

    /// <summary>
    /// Creates an audit entry for session transfer.
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <param name="userId">ID of the user</param>
    /// <param name="fromTableId">Source table ID</param>
    /// <param name="toTableId">Destination table ID</param>
    /// <param name="reason">Reason for transfer</param>
    /// <returns>Session transfer audit entry</returns>
    public static SessionAuditEntry SessionTransferred(Guid sessionId, Guid userId, Guid fromTableId, Guid toTableId, string reason)
    {
        return Create(sessionId, "SessionTransferred", $"From: {fromTableId}, To: {toTableId}, Reason: {reason}", userId);
    }
}
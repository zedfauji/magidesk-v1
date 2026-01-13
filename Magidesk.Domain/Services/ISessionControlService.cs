using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Services;

/// <summary>
/// Service interface for session control operations including pause/resume, guest count updates, and session transfers.
/// </summary>
public interface ISessionControlService
{
    /// <summary>
    /// Pauses an active session with reason tracking and audit logging.
    /// </summary>
    /// <param name="sessionId">ID of the session to pause</param>
    /// <param name="reason">Reason for pausing the session</param>
    /// <returns>Result of the pause operation</returns>
    Task<SessionControlResult> PauseSessionAsync(Guid sessionId, string reason);

    /// <summary>
    /// Resumes a paused session with accurate time tracking continuation.
    /// </summary>
    /// <param name="sessionId">ID of the session to resume</param>
    /// <returns>Result of the resume operation</returns>
    Task<SessionControlResult> ResumeSessionAsync(Guid sessionId);

    /// <summary>
    /// Updates the guest count for an active session with staff authorization.
    /// </summary>
    /// <param name="sessionId">ID of the session to update</param>
    /// <param name="newGuestCount">New guest count (must be between 1 and 20)</param>
    /// <param name="staffId">ID of the staff member making the change</param>
    /// <returns>Result of the guest count update</returns>
    Task<SessionControlResult> UpdateGuestCountAsync(Guid sessionId, int newGuestCount, Guid staffId);

    /// <summary>
    /// Transfers an active session between tables with data preservation.
    /// </summary>
    /// <param name="sessionId">ID of the session to transfer</param>
    /// <param name="targetTableId">ID of the target table</param>
    /// <param name="reason">Reason for the transfer</param>
    /// <returns>Result of the session transfer</returns>
    Task<SessionControlResult> TransferSessionAsync(Guid sessionId, Guid targetTableId, string reason);

    /// <summary>
    /// Gets alerts for sessions that require attention (long pauses, capacity issues, etc.).
    /// </summary>
    /// <returns>Collection of session alerts</returns>
    Task<IEnumerable<SessionAlert>> GetSessionAlertsAsync();
}
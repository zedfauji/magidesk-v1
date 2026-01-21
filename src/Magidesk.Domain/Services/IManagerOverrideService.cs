using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Services;

/// <summary>
/// Service interface for manager override operations including authorization, time adjustments, pricing overrides, and session management.
/// </summary>
public interface IManagerOverrideService
{
    /// <summary>
    /// Validates manager authorization using PIN and permission checking.
    /// </summary>
    /// <param name="managerPin">Manager's PIN for authentication</param>
    /// <param name="userId">ID of the user requesting authorization</param>
    /// <returns>Result of the authorization validation</returns>
    Task<OverrideResult> ValidateManagerAuthorizationAsync(string managerPin, Guid userId);

    /// <summary>
    /// Applies time adjustment override to a session with complete audit trail.
    /// </summary>
    /// <param name="sessionId">ID of the session to adjust</param>
    /// <param name="adjustment">Time adjustment (positive to add, negative to subtract)</param>
    /// <param name="reason">Reason for the time adjustment</param>
    /// <param name="managerId">ID of the manager performing the override</param>
    /// <returns>Result of the time adjustment operation</returns>
    Task<OverrideResult> ApplyTimeAdjustmentAsync(Guid sessionId, TimeSpan adjustment, string reason, Guid managerId);

    /// <summary>
    /// Applies pricing override to a session with reason code requirements.
    /// </summary>
    /// <param name="sessionId">ID of the session to override pricing for</param>
    /// <param name="overrideAmount">New pricing amount to apply</param>
    /// <param name="reason">Reason for the pricing override</param>
    /// <param name="managerId">ID of the manager performing the override</param>
    /// <returns>Result of the pricing override operation</returns>
    Task<OverrideResult> ApplyPricingOverrideAsync(Guid sessionId, Money overrideAmount, string reason, Guid managerId);

    /// <summary>
    /// Forces session end for emergency situations regardless of current state.
    /// </summary>
    /// <param name="sessionId">ID of the session to force end</param>
    /// <param name="reason">Reason for forcing session end</param>
    /// <param name="managerId">ID of the manager performing the override</param>
    /// <returns>Result of the force end operation</returns>
    Task<OverrideResult> ForceEndSessionAsync(Guid sessionId, string reason, Guid managerId);

    /// <summary>
    /// Retrieves comprehensive override audit trail for management review.
    /// </summary>
    /// <param name="fromDate">Start date for audit trail query</param>
    /// <param name="toDate">End date for audit trail query</param>
    /// <returns>Collection of override audit entries</returns>
    Task<IEnumerable<OverrideAuditEntry>> GetOverrideAuditTrailAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Retrieves override audit trail for a specific session.
    /// </summary>
    /// <param name="sessionId">ID of the session to get audit trail for</param>
    /// <returns>Collection of override audit entries for the session</returns>
    Task<IEnumerable<OverrideAuditEntry>> GetSessionOverrideAuditTrailAsync(Guid sessionId);

    /// <summary>
    /// Retrieves override audit trail for a specific manager.
    /// </summary>
    /// <param name="managerId">ID of the manager to get audit trail for</param>
    /// <param name="fromDate">Start date for audit trail query</param>
    /// <param name="toDate">End date for audit trail query</param>
    /// <returns>Collection of override audit entries for the manager</returns>
    Task<IEnumerable<OverrideAuditEntry>> GetManagerOverrideAuditTrailAsync(Guid managerId, DateTime fromDate, DateTime toDate);
}
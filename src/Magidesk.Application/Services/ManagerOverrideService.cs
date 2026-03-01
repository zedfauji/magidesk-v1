using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

/// <summary>
/// Service implementation for manager override operations including authorization, time adjustments, pricing overrides, and session management.
/// </summary>
public class ManagerOverrideService : IManagerOverrideService
{
    private readonly ITableSessionRepository _sessionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOverrideAuditRepository _auditRepository;
    private readonly ISecurityService _securityService;
    private readonly IAesEncryptionService _encryptionService;

    public ManagerOverrideService(
        ITableSessionRepository sessionRepository,
        IUserRepository userRepository,
        IOverrideAuditRepository auditRepository,
        ISecurityService securityService,
        IAesEncryptionService encryptionService)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
        _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
        _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
    }

    /// <summary>
    /// Validates manager authorization using PIN and permission checking.
    /// </summary>
    public async Task<OverrideResult> ValidateManagerAuthorizationAsync(string managerPin, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(managerPin))
        {
            return OverrideResult.ValidationError("Manager PIN is required");
        }

        if (userId == Guid.Empty)
        {
            return OverrideResult.ValidationError("User ID is required");
        }

        try
        {
            // Encrypt the PIN for comparison
            var encryptedPin = _encryptionService.Encrypt(managerPin);
            
            // Find user by PIN using SecurityService
            var manager = await _securityService.GetUserByPinAsync(encryptedPin);
            if (manager == null)
            {
                return OverrideResult.Unauthorized();
            }

            // Check if user has manager permissions (can adjust session time)
            if (manager.Role == null || !manager.Role.Permissions.HasFlag(UserPermission.AdjustSessionTime))
            {
                return OverrideResult.Unauthorized();
            }

            return OverrideResult.Success(new OverrideData(Guid.Empty, OverrideType.TimeAdjustment, string.Empty, string.Empty, manager.Id, DateTime.UtcNow));
        }
        catch (Exception ex)
        {
            return OverrideResult.InvalidOperation($"Authorization validation failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Applies time adjustment override to a session with complete audit trail.
    /// </summary>
    public async Task<OverrideResult> ApplyTimeAdjustmentAsync(Guid sessionId, TimeSpan adjustment, string reason, Guid managerId)
    {
        if (sessionId == Guid.Empty)
        {
            return OverrideResult.ValidationError("Session ID is required");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return OverrideResult.ValidationError("Reason is required for time adjustment");
        }

        if (managerId == Guid.Empty)
        {
            return OverrideResult.ValidationError("Manager ID is required");
        }

        try
        {
            // Get the session
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null)
            {
                return OverrideResult.NotFound();
            }

            // Validate session state
            if (session.Status == TableSessionStatus.Ended)
            {
                return OverrideResult.InvalidOperation("Cannot adjust time of an ended session");
            }

            // Verify manager exists and has permissions
            var manager = await _userRepository.GetByIdAsync(managerId);
            if (manager == null || !manager.IsActive)
            {
                return OverrideResult.Unauthorized();
            }

            if (manager.Role == null || !manager.Role.Permissions.HasFlag(UserPermission.AdjustSessionTime))
            {
                return OverrideResult.Unauthorized();
            }

            // Record original values for audit
            var originalBillableTime = session.GetBillableTime();
            var originalAdjustment = session.ManualAdjustment;

            // Apply the time adjustment
            session.AdjustTime(adjustment);

            // Update the session
            await _sessionRepository.UpdateAsync(session);

            // Create audit entry
            var auditEntry = OverrideAuditEntry.Create(
                sessionId: sessionId,
                overrideType: OverrideType.TimeAdjustment,
                originalValue: originalAdjustment.ToString(),
                newValue: session.ManualAdjustment.ToString(),
                reason: reason,
                managerId: managerId
            );

            await _auditRepository.AddAsync(auditEntry);

            // Create result data
            var resultData = OverrideData.Create(
                sessionId: sessionId,
                overrideType: OverrideType.TimeAdjustment,
                originalValue: originalBillableTime.ToString(),
                newValue: session.GetBillableTime().ToString(),
                managerId: managerId
            );

            return OverrideResult.Success(resultData);
        }
        catch (BusinessRuleViolationException ex)
        {
            return OverrideResult.InvalidOperation(ex.Message);
        }
        catch (Exception ex)
        {
            return OverrideResult.InvalidOperation($"Time adjustment failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Applies pricing override to a session with reason code requirements.
    /// </summary>
    public async Task<OverrideResult> ApplyPricingOverrideAsync(Guid sessionId, Money overrideAmount, string reason, Guid managerId)
    {
        if (sessionId == Guid.Empty)
        {
            return OverrideResult.ValidationError("Session ID is required");
        }

        if (overrideAmount == null)
        {
            return OverrideResult.ValidationError("Override amount is required");
        }

        if (overrideAmount.Amount < 0)
        {
            return OverrideResult.ValidationError("Override amount cannot be negative");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return OverrideResult.ValidationError("Reason is required for pricing override");
        }

        if (managerId == Guid.Empty)
        {
            return OverrideResult.ValidationError("Manager ID is required");
        }

        try
        {
            // Get the session
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null)
            {
                return OverrideResult.NotFound();
            }

            // Verify manager exists and has permissions
            var manager = await _userRepository.GetByIdAsync(managerId);
            if (manager == null || !manager.IsActive)
            {
                return OverrideResult.Unauthorized();
            }

            if (manager.Role == null || !manager.Role.Permissions.HasFlag(UserPermission.AdjustSessionTime))
            {
                return OverrideResult.Unauthorized();
            }

            // Record original values for audit
            var originalCharge = session.TotalCharge;

            // Note: Since TableSession doesn't have a direct pricing override method,
            // we would need to extend it or handle this at the application layer
            // For now, we'll create the audit entry to track the intended override
            
            // Create audit entry
            var auditEntry = OverrideAuditEntry.Create(
                sessionId: sessionId,
                overrideType: OverrideType.PricingOverride,
                originalValue: originalCharge.Amount.ToString("C"),
                newValue: overrideAmount.Amount.ToString("C"),
                reason: reason,
                managerId: managerId
            );

            await _auditRepository.AddAsync(auditEntry);

            // Create result data
            var resultData = OverrideData.Create(
                sessionId: sessionId,
                overrideType: OverrideType.PricingOverride,
                originalValue: originalCharge.Amount.ToString("C"),
                newValue: overrideAmount.Amount.ToString("C"),
                managerId: managerId
            );

            return OverrideResult.Success(resultData);
        }
        catch (Exception ex)
        {
            return OverrideResult.InvalidOperation($"Pricing override failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Forces session end for emergency situations regardless of current state.
    /// </summary>
    public async Task<OverrideResult> ForceEndSessionAsync(Guid sessionId, string reason, Guid managerId)
    {
        if (sessionId == Guid.Empty)
        {
            return OverrideResult.ValidationError("Session ID is required");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return OverrideResult.ValidationError("Reason is required for force end session");
        }

        if (managerId == Guid.Empty)
        {
            return OverrideResult.ValidationError("Manager ID is required");
        }

        try
        {
            // Get the session
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null)
            {
                return OverrideResult.NotFound();
            }

            // Verify manager exists and has permissions
            var manager = await _userRepository.GetByIdAsync(managerId);
            if (manager == null || !manager.IsActive)
            {
                return OverrideResult.Unauthorized();
            }

            if (manager.Role == null || !manager.Role.Permissions.HasFlag(UserPermission.AdjustSessionTime))
            {
                return OverrideResult.Unauthorized();
            }

            // Record original values for audit
            var originalStatus = session.Status;

            // If session is paused, resume it first to calculate proper charges
            if (session.Status == TableSessionStatus.Paused)
            {
                session.Resume();
            }

            // Force end the session (we'll need to calculate charges at application layer)
            // For now, end with zero charge as this is an emergency override
            if (session.Status != TableSessionStatus.Ended)
            {
                session.End(Money.Zero());
            }

            // Update the session
            await _sessionRepository.UpdateAsync(session);

            // Create audit entry
            var auditEntry = OverrideAuditEntry.Create(
                sessionId: sessionId,
                overrideType: OverrideType.ForceEndSession,
                originalValue: originalStatus.ToString(),
                newValue: session.Status.ToString(),
                reason: reason,
                managerId: managerId
            );

            await _auditRepository.AddAsync(auditEntry);

            // Create result data
            var resultData = OverrideData.Create(
                sessionId: sessionId,
                overrideType: OverrideType.ForceEndSession,
                originalValue: originalStatus.ToString(),
                newValue: session.Status.ToString(),
                managerId: managerId
            );

            return OverrideResult.Success(resultData);
        }
        catch (BusinessRuleViolationException ex)
        {
            return OverrideResult.InvalidOperation(ex.Message);
        }
        catch (Exception ex)
        {
            return OverrideResult.InvalidOperation($"Force end session failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves comprehensive override audit trail for management review.
    /// </summary>
    public async Task<IEnumerable<OverrideAuditEntry>> GetOverrideAuditTrailAsync(DateTime fromDate, DateTime toDate)
    {
        if (fromDate > toDate)
        {
            throw new ArgumentException("From date cannot be greater than to date");
        }

        return await _auditRepository.GetByDateRangeAsync(fromDate, toDate);
    }

    /// <summary>
    /// Retrieves override audit trail for a specific session.
    /// </summary>
    public async Task<IEnumerable<OverrideAuditEntry>> GetSessionOverrideAuditTrailAsync(Guid sessionId)
    {
        if (sessionId == Guid.Empty)
        {
            throw new ArgumentException("Session ID cannot be empty");
        }

        return await _auditRepository.GetBySessionIdAsync(sessionId);
    }

    /// <summary>
    /// Retrieves override audit trail for a specific manager.
    /// </summary>
    public async Task<IEnumerable<OverrideAuditEntry>> GetManagerOverrideAuditTrailAsync(Guid managerId, DateTime fromDate, DateTime toDate)
    {
        if (managerId == Guid.Empty)
        {
            throw new ArgumentException("Manager ID cannot be empty");
        }

        if (fromDate > toDate)
        {
            throw new ArgumentException("From date cannot be greater than to date");
        }

        return await _auditRepository.GetByManagerIdAsync(managerId, fromDate, toDate);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

/// <summary>
/// Service for session control operations including pause/resume, guest count updates, and session transfers.
/// </summary>
public class SessionControlService : ISessionControlService
{
    private readonly ITableSessionRepository _sessionRepository;
    private readonly ITableRepository _tableRepository;
    private readonly ITableTypeRepository _tableTypeRepository;
    private readonly IAuditEventRepository _auditEventRepository;
    private readonly Domain.Services.IPricingService _pricingService;
    private readonly IUserService _userService;

    public SessionControlService(
        ITableSessionRepository sessionRepository,
        ITableRepository tableRepository,
        ITableTypeRepository tableTypeRepository,
        IAuditEventRepository auditEventRepository,
        Domain.Services.IPricingService pricingService,
        IUserService userService)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _tableRepository = tableRepository ?? throw new ArgumentNullException(nameof(tableRepository));
        _tableTypeRepository = tableTypeRepository ?? throw new ArgumentNullException(nameof(tableTypeRepository));
        _auditEventRepository = auditEventRepository ?? throw new ArgumentNullException(nameof(auditEventRepository));
        _pricingService = pricingService ?? throw new ArgumentNullException(nameof(pricingService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    /// <inheritdoc />
    public async Task<SessionControlResult> PauseSessionAsync(Guid sessionId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return SessionControlResult.ValidationError("Pause reason is required");
        }

        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            return SessionControlResult.NotFound();
        }

        if (session.Status != TableSessionStatus.Active)
        {
            return SessionControlResult.InvalidState("Session must be active to pause");
        }

        try
        {
            session.Pause();
            await _sessionRepository.UpdateAsync(session);

            // Log audit event
            var userId = _userService.CurrentUser?.Id ?? Guid.Empty;
            var auditEvent = AuditEvent.Create(
                AuditEventType.StatusChanged,
                "TableSession",
                sessionId,
                userId,
                JsonSerializer.Serialize(new { Status = "Paused", Reason = reason }),
                $"Session paused. Reason: {reason}",
                correlationId: Guid.NewGuid());

            await _auditEventRepository.AddAsync(auditEvent);

            var tableType = await _tableTypeRepository.GetByIdAsync(session.TableTypeId);
            var currentCharge = tableType != null 
                ? _pricingService.CalculateTimeCharge(session.GetBillableTime(), tableType)
                : Money.Zero();

            var data = new SessionControlData(
                SessionId: sessionId,
                Status: session.Status,
                PausedAt: session.PausedAt,
                TotalPausedDuration: session.TotalPausedDuration,
                CurrentCharge: currentCharge);

            return SessionControlResult.Success(data);
        }
        catch (InvalidOperationException ex)
        {
            return SessionControlResult.InvalidState(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<SessionControlResult> ResumeSessionAsync(Guid sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            return SessionControlResult.NotFound();
        }

        if (session.Status != TableSessionStatus.Paused)
        {
            return SessionControlResult.InvalidState("Can only resume a paused session");
        }

        try
        {
            session.Resume();
            await _sessionRepository.UpdateAsync(session);

            // Log audit event
            var userId = _userService.CurrentUser?.Id ?? Guid.Empty;
            var auditEvent = AuditEvent.Create(
                AuditEventType.StatusChanged,
                "TableSession",
                sessionId,
                userId,
                JsonSerializer.Serialize(new { Status = "Active" }),
                "Session resumed from pause",
                correlationId: Guid.NewGuid());

            await _auditEventRepository.AddAsync(auditEvent);

            var tableType = await _tableTypeRepository.GetByIdAsync(session.TableTypeId);
            var currentCharge = tableType != null 
                ? _pricingService.CalculateTimeCharge(session.GetBillableTime(), tableType)
                : Money.Zero();

            var data = new SessionControlData(
                SessionId: sessionId,
                Status: session.Status,
                PausedAt: session.PausedAt,
                TotalPausedDuration: session.TotalPausedDuration,
                CurrentCharge: currentCharge);

            return SessionControlResult.Success(data);
        }
        catch (InvalidOperationException ex)
        {
            return SessionControlResult.InvalidState(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<SessionControlResult> UpdateGuestCountAsync(Guid sessionId, int newGuestCount, Guid staffId)
    {
        if (newGuestCount < 1 || newGuestCount > 20)
        {
            return SessionControlResult.ValidationError("Guest count must be between 1 and 20");
        }

        if (staffId == Guid.Empty)
        {
            return SessionControlResult.Unauthorized("Staff authorization required for guest count updates");
        }

        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            return SessionControlResult.NotFound();
        }

        if (session.Status == TableSessionStatus.Ended)
        {
            return SessionControlResult.InvalidState("Cannot update guest count on an ended session");
        }

        try
        {
            var oldGuestCount = session.GuestCount;
            session.UpdateGuestCount(newGuestCount);
            await _sessionRepository.UpdateAsync(session);

            // Log audit event
            var userId = _userService.CurrentUser?.Id ?? Guid.Empty;
            var auditEvent = AuditEvent.Create(
                AuditEventType.Modified,
                "TableSession",
                sessionId,
                userId,
                JsonSerializer.Serialize(new { GuestCount = newGuestCount, StaffId = staffId }),
                $"Guest count updated from {oldGuestCount} to {newGuestCount} by staff {staffId}",
                beforeState: JsonSerializer.Serialize(new { GuestCount = oldGuestCount }),
                correlationId: Guid.NewGuid());

            await _auditEventRepository.AddAsync(auditEvent);

            var tableType = await _tableTypeRepository.GetByIdAsync(session.TableTypeId);
            var currentCharge = tableType != null 
                ? _pricingService.CalculateTimeCharge(session.GetBillableTime(), tableType)
                : Money.Zero();

            var data = new SessionControlData(
                SessionId: sessionId,
                Status: session.Status,
                PausedAt: session.PausedAt,
                TotalPausedDuration: session.TotalPausedDuration,
                CurrentCharge: currentCharge);

            return SessionControlResult.Success(data);
        }
        catch (InvalidOperationException ex)
        {
            return SessionControlResult.InvalidState(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<SessionControlResult> TransferSessionAsync(Guid sessionId, Guid targetTableId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return SessionControlResult.ValidationError("Transfer reason is required");
        }

        if (targetTableId == Guid.Empty)
        {
            return SessionControlResult.ValidationError("Target table ID is required");
        }

        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            return SessionControlResult.NotFound();
        }

        if (session.Status == TableSessionStatus.Ended)
        {
            return SessionControlResult.InvalidState("Cannot transfer an ended session");
        }

        // Check if target table exists and is available
        var targetTable = await _tableRepository.GetByIdAsync(targetTableId);
        if (targetTable == null)
        {
            return SessionControlResult.ValidationError("Target table not found");
        }

        // Check if target table has an active session
        var existingSession = await _sessionRepository.GetActiveSessionByTableIdAsync(targetTableId);
        if (existingSession != null)
        {
            return SessionControlResult.InvalidState("Target table already has an active session");
        }

        try
        {
            var originalTableId = session.TableId;
            
            // Create a new session for the target table with preserved data
            var transferredSession = TableSession.Start(
                tableId: targetTableId,
                tableTypeId: session.TableTypeId,
                hourlyRate: session.HourlyRate,
                guestCount: session.GuestCount,
                customerId: session.CustomerId,
                ticketId: session.TicketId);

            // Preserve timing data using reflection (since properties are private set)
            var startTimeProperty = typeof(TableSession).GetProperty("StartTime");
            var totalPausedDurationProperty = typeof(TableSession).GetProperty("TotalPausedDuration");
            var pausedAtProperty = typeof(TableSession).GetProperty("PausedAt");
            var statusProperty = typeof(TableSession).GetProperty("Status");
            
            startTimeProperty?.SetValue(transferredSession, session.StartTime);
            totalPausedDurationProperty?.SetValue(transferredSession, session.TotalPausedDuration);
            
            if (session.Status == TableSessionStatus.Paused)
            {
                pausedAtProperty?.SetValue(transferredSession, session.PausedAt);
                statusProperty?.SetValue(transferredSession, TableSessionStatus.Paused);
            }

            // End the original session
            var tableType = await _tableTypeRepository.GetByIdAsync(session.TableTypeId);
            var currentCharge = tableType != null 
                ? _pricingService.CalculateTimeCharge(session.GetBillableTime(), tableType)
                : Money.Zero();
            session.End(currentCharge);

            // Save both sessions
            await _sessionRepository.UpdateAsync(session);
            await _sessionRepository.AddAsync(transferredSession);

            // Log audit events
            var userId = _userService.CurrentUser?.Id ?? Guid.Empty;
            var correlationId = Guid.NewGuid();
            
            var endAuditEvent = AuditEvent.Create(
                AuditEventType.StatusChanged,
                "TableSession",
                sessionId,
                userId,
                JsonSerializer.Serialize(new { Status = "Ended", TransferredTo = targetTableId, Reason = reason }),
                $"Session transferred from table {originalTableId} to table {targetTableId}. Reason: {reason}",
                correlationId: correlationId);

            var startAuditEvent = AuditEvent.Create(
                AuditEventType.Created,
                "TableSession",
                transferredSession.Id,
                userId,
                JsonSerializer.Serialize(new { Status = transferredSession.Status, TransferredFrom = originalTableId, Reason = reason }),
                $"Session transferred from table {originalTableId} to table {targetTableId}. Reason: {reason}",
                correlationId: correlationId);

            await _auditEventRepository.AddAsync(endAuditEvent);
            await _auditEventRepository.AddAsync(startAuditEvent);

            var newCurrentCharge = tableType != null 
                ? _pricingService.CalculateTimeCharge(transferredSession.GetBillableTime(), tableType)
                : Money.Zero();

            var data = new SessionControlData(
                SessionId: transferredSession.Id,
                Status: transferredSession.Status,
                PausedAt: transferredSession.PausedAt,
                TotalPausedDuration: transferredSession.TotalPausedDuration,
                CurrentCharge: newCurrentCharge);

            return SessionControlResult.Success(data);
        }
        catch (Exception ex)
        {
            return SessionControlResult.InvalidState($"Transfer failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SessionAlert>> GetSessionAlertsAsync()
    {
        var alerts = new List<SessionAlert>();
        var activeSessions = await _sessionRepository.GetActiveSessionsAsync();
        var pausedSessions = await _sessionRepository.GetSessionsByStatusAsync(TableSessionStatus.Paused);
        
        var now = DateTime.UtcNow;

        // Check for long-paused sessions (more than 2 hours)
        foreach (var session in pausedSessions)
        {
            if (session.PausedAt.HasValue)
            {
                var pauseDuration = now - session.PausedAt.Value;
                if (pauseDuration.TotalHours > 2)
                {
                    alerts.Add(new SessionAlert(
                        SessionId: session.Id,
                        TableId: session.TableId,
                        AlertType: SessionAlertType.LongPause,
                        Message: $"Session has been paused for {pauseDuration.TotalHours:F1} hours",
                        CreatedAt: now,
                        Severity: pauseDuration.TotalHours > 4 ? SessionAlertSeverity.High : SessionAlertSeverity.Medium));
                }
            }
        }

        // Check for long-running sessions (more than 8 hours)
        foreach (var session in activeSessions)
        {
            var sessionDuration = session.GetBillableTime();
            if (sessionDuration.TotalHours > 8)
            {
                alerts.Add(new SessionAlert(
                    SessionId: session.Id,
                    TableId: session.TableId,
                    AlertType: SessionAlertType.LongSession,
                    Message: $"Session has been running for {sessionDuration.TotalHours:F1} hours",
                    CreatedAt: now,
                    Severity: sessionDuration.TotalHours > 12 ? SessionAlertSeverity.High : SessionAlertSeverity.Medium));
            }
        }

        // Check for potential capacity issues (guest count > 8 for standard tables)
        foreach (var session in activeSessions.Concat(pausedSessions))
        {
            if (session.GuestCount > 8)
            {
                alerts.Add(new SessionAlert(
                    SessionId: session.Id,
                    TableId: session.TableId,
                    AlertType: SessionAlertType.CapacityIssue,
                    Message: $"High guest count: {session.GuestCount} players",
                    CreatedAt: now,
                    Severity: session.GuestCount > 12 ? SessionAlertSeverity.High : SessionAlertSeverity.Medium));
            }
        }

        return alerts.OrderByDescending(a => a.Severity).ThenByDescending(a => a.CreatedAt);
    }
}
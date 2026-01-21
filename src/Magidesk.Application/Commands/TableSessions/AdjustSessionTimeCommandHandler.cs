using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Handler for AdjustSessionTimeCommand.
/// </summary>
public class AdjustSessionTimeCommandHandler : ICommandHandler<AdjustSessionTimeCommand, AdjustSessionTimeResult>
{
    private readonly ITableSessionRepository _sessionRepository;
    private readonly ISecurityService _securityService;
    private readonly IUserService _userService;
    private readonly IAuditEventRepository _auditEventRepository;

    public AdjustSessionTimeCommandHandler(
        ITableSessionRepository sessionRepository,
        ISecurityService securityService,
        IUserService userService,
        IAuditEventRepository auditEventRepository)
    {
        _sessionRepository = sessionRepository;
        _securityService = securityService;
        _userService = userService;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<AdjustSessionTimeResult> HandleAsync(
        AdjustSessionTimeCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.SessionId == Guid.Empty)
        {
            throw new ArgumentException("Session ID cannot be empty.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.Reason))
        {
            throw new ArgumentException("Reason is required for time adjustment.", nameof(command));
        }

        // 1. Verify Permission
        var currentUser = _userService.CurrentUser;
        if (currentUser == null)
        {
             throw new UnauthorizedAccessException("User context is required.");
        }

        var userId = new UserId(currentUser.Id);
        var hasPermission = await _securityService.HasPermissionAsync(userId, UserPermission.AdjustSessionTime, cancellationToken);
        
        if (!hasPermission)
        {
            throw new UnauthorizedAccessException("User does not have permission to adjust session time.");
        }

        // 2. Get Session
        var session = await _sessionRepository.GetByIdAsync(command.SessionId);
        if (session == null)
        {
            throw new InvalidOperationException($"Table session {command.SessionId} not found.");
        }

        var oldBillableTime = session.GetBillableTime();

        // 3. Apply Adjustment
        session.AdjustTime(command.AdjustmentAmount);

        // 4. Update Session Repository
        await _sessionRepository.UpdateAsync(session);

        // 5. Audit Log
        var auditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            nameof(TableSession),
            session.Id,
            currentUser.Id,
            afterState: $"ManualAdjustment: {session.ManualAdjustment}",
            description: $"Manual time adjustment: {command.AdjustmentAmount}. Reason: {command.Reason}",
            beforeState: $"ManualAdjustment: {session.ManualAdjustment - command.AdjustmentAmount}"
        );

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new AdjustSessionTimeResult(
            session.Id,
            session.GetBillableTime(),
            session.ManualAdjustment
        );
    }
}

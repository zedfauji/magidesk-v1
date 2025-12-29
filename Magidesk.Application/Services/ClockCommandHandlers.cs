using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

public class ClockInCommandHandler : ICommandHandler<ClockInCommand>
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public ClockInCommandHandler(
        IAttendanceRepository attendanceRepository,
        IAuditEventRepository auditEventRepository)
    {
        _attendanceRepository = attendanceRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(ClockInCommand command, CancellationToken cancellationToken = default)
    {
        // Check if already clocked in? (Requires query)
        // For simple implementation:
        
        var attendance = AttendanceHistory.Create(command.UserId);
        await _attendanceRepository.AddAsync(attendance, cancellationToken);

        var audit = AuditEvent.Create(
            AuditEventType.StatusChanged,
            nameof(AttendanceHistory),
            attendance.Id,
            command.UserId.Value,
            "ClockedIn",
            $"User {command.UserId} clocked in."
        );
        await _auditEventRepository.AddAsync(audit, cancellationToken);
    }
}

public class ClockOutCommandHandler : ICommandHandler<ClockOutCommand>
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public ClockOutCommandHandler(
        IAttendanceRepository attendanceRepository,
        IAuditEventRepository auditEventRepository)
    {
        _attendanceRepository = attendanceRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(ClockOutCommand command, CancellationToken cancellationToken = default)
    {
        // Need to find open attendance record
        var openRecord = await _attendanceRepository.GetOpenByUserIdAsync(command.UserId, cancellationToken);
        
        if (openRecord == null)
        {
             throw new Domain.Exceptions.BusinessRuleViolationException("No open attendance record found.");
        }

        openRecord.ClockOut();
        await _attendanceRepository.UpdateAsync(openRecord, cancellationToken);

        var audit = AuditEvent.Create(
            AuditEventType.StatusChanged,
            nameof(AttendanceHistory),
            openRecord.Id,
            command.UserId.Value,
            "ClockedOut",
            $"User {command.UserId} clocked out."
        );
        await _auditEventRepository.AddAsync(audit, cancellationToken);
    }
}

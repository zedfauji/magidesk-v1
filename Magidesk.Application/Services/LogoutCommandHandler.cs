using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Services;

public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IAuditEventRepository _auditEventRepository;

    public LogoutCommandHandler(IAuditEventRepository auditEventRepository)
    {
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(LogoutCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Audit
        var audit = AuditEvent.Create(
            AuditEventType.StatusChanged,
            "UserSession",
            command.UserId,
            command.UserId,
            "LoggedOut",
            $"User {command.UserId} logged out"
        );
        await _auditEventRepository.AddAsync(audit, cancellationToken);
    }
}

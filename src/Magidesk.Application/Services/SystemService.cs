using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

public class SystemService : ISystemService
{
    private readonly ISecurityService _securityService;
    private readonly IAuditEventRepository _auditEventRepository;

    public SystemService(
        ISecurityService securityService,
        IAuditEventRepository auditEventRepository)
    {
        _securityService = securityService;
        _auditEventRepository = auditEventRepository;
    }

    public async Task ShutdownAsync(UserId initiatedBy, CancellationToken cancellationToken = default)
    {
        // 1. Check Permissions
        // Using SystemConfiguration as a proxy for Shutdown permission given current enum definition
        if (!await _securityService.HasPermissionAsync(initiatedBy, UserPermission.SystemConfiguration, cancellationToken))
        {
            throw new Domain.Exceptions.BusinessRuleViolationException("User does not have permission to shut down the system.");
        }

        // 2. Log Shutdown Event
        // This ensures tracking of who turned off the POS and when.
        // The await ensures it is committed to the DB before we return.
        var auditEvent = AuditEvent.Create(
            AuditEventType.SystemShutdown,
            "System",
            Guid.Empty, // No specific entity target
            initiatedBy.Value,
            "System Shutdown Initiated",
            $"Shutdown initiated by user",
            correlationId: Guid.NewGuid());

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        // 3. Graceful Resource Release
        // In this architecture, disposing the Scope (handled by DI/UI) closes the DB connection.
        // We just need to ensure the methods return successfully.
        
        // Potential: Release Printer resources (Mock services don't need this)
    }
}

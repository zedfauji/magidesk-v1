using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for ReleaseTableCommand.
/// </summary>
public class ReleaseTableCommandHandler : ICommandHandler<ReleaseTableCommand, ReleaseTableResult>
{
    private readonly ITableRepository _tableRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public ReleaseTableCommandHandler(
        ITableRepository tableRepository,
        IAuditEventRepository auditEventRepository)
    {
        _tableRepository = tableRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<ReleaseTableResult> HandleAsync(ReleaseTableCommand command, CancellationToken cancellationToken = default)
    {
        // Get table
        var table = await _tableRepository.GetByIdAsync(command.TableId, cancellationToken);
        if (table == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Table {command.TableId} not found.");
        }

        // Release table
        table.ReleaseTicket();

        // Update table
        await _tableRepository.UpdateAsync(table, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Table),
            table.Id,
            Guid.Empty, // System operation
            System.Text.Json.JsonSerializer.Serialize(new
            {
                TableId = table.Id,
                TableNumber = table.TableNumber,
                Status = table.Status
            }),
            $"Table {table.TableNumber} released",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new ReleaseTableResult
        {
            TableId = table.Id
        };
    }
}


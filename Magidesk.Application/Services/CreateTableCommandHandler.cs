using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for CreateTableCommand.
/// </summary>
public class CreateTableCommandHandler : ICommandHandler<CreateTableCommand, CreateTableResult>
{
    private readonly ITableRepository _tableRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public CreateTableCommandHandler(
        ITableRepository tableRepository,
        IAuditEventRepository auditEventRepository)
    {
        _tableRepository = tableRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<CreateTableResult> HandleAsync(CreateTableCommand command, CancellationToken cancellationToken = default)
    {
        // Check if table number already exists
        var existingTable = await _tableRepository.GetByTableNumberAsync(command.TableNumber, cancellationToken);
        if (existingTable != null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Table number {command.TableNumber} already exists.");
        }

        // Create table
        var table = Table.Create(
            command.TableNumber,
            command.Capacity,
            command.X,
            command.Y,
            command.FloorId,
            command.IsActive);

        // Save table
        await _tableRepository.AddAsync(table, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.Created,
            nameof(Table),
            table.Id,
            Guid.Empty, // System operation
            System.Text.Json.JsonSerializer.Serialize(new
            {
                TableNumber = table.TableNumber,
                Capacity = table.Capacity,
                X = table.X,
                Y = table.Y,
                FloorId = table.FloorId,
                Status = table.Status,
                IsActive = table.IsActive
            }),
            $"Table {table.TableNumber} created",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new CreateTableResult
        {
            TableId = table.Id,
            TableNumber = table.TableNumber
        };
    }
}


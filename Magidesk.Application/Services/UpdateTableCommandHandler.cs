using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for UpdateTableCommand.
/// </summary>
public class UpdateTableCommandHandler : ICommandHandler<UpdateTableCommand, UpdateTableResult>
{
    private readonly ITableRepository _tableRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public UpdateTableCommandHandler(
        ITableRepository tableRepository,
        IAuditEventRepository auditEventRepository)
    {
        _tableRepository = tableRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<UpdateTableResult> HandleAsync(UpdateTableCommand command, CancellationToken cancellationToken = default)
    {
        // Get table
        var table = await _tableRepository.GetByIdAsync(command.TableId, cancellationToken);
        if (table == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Table {command.TableId} not found.");
        }

        // Update table properties
        if (command.TableNumber.HasValue)
        {
            // Check if new table number already exists
            var existingTable = await _tableRepository.GetByTableNumberAsync(command.TableNumber.Value, cancellationToken);
            if (existingTable != null && existingTable.Id != table.Id)
            {
                throw new Domain.Exceptions.BusinessRuleViolationException($"Table number {command.TableNumber.Value} already exists.");
            }

            table.UpdateTableNumber(command.TableNumber.Value);
        }

        if (command.Capacity.HasValue)
        {
            table.UpdateCapacity(command.Capacity.Value);
        }

        if (command.X.HasValue && command.Y.HasValue)
        {
            table.UpdatePosition(command.X.Value, command.Y.Value);
        }

        if (command.FloorId.HasValue || command.FloorId == null)
        {
            table.UpdateFloor(command.FloorId);
        }

        if (command.IsActive.HasValue)
        {
            if (command.IsActive.Value)
            {
                table.Activate();
            }
            else
            {
                table.Deactivate();
            }
        }

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
                TableNumber = table.TableNumber,
                Capacity = table.Capacity,
                X = table.X,
                Y = table.Y,
                FloorId = table.FloorId,
                Status = table.Status,
                IsActive = table.IsActive
            }),
            $"Table {table.TableNumber} updated",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new UpdateTableResult
        {
            TableId = table.Id,
            TableNumber = table.TableNumber
        };
    }
}


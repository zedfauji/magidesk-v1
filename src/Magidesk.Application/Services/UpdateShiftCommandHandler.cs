using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for UpdateShiftCommand.
/// </summary>
public class UpdateShiftCommandHandler : ICommandHandler<UpdateShiftCommand, UpdateShiftResult>
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public UpdateShiftCommandHandler(
        IShiftRepository shiftRepository,
        IAuditEventRepository auditEventRepository)
    {
        _shiftRepository = shiftRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<UpdateShiftResult> HandleAsync(UpdateShiftCommand command, CancellationToken cancellationToken = default)
    {
        // Get shift
        var shift = await _shiftRepository.GetByIdAsync(command.ShiftId, cancellationToken);
        if (shift == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Shift {command.ShiftId} not found.");
        }

        // Update shift properties
        if (command.Name != null)
        {
            shift.UpdateName(command.Name);
        }

        if (command.StartTime.HasValue && command.EndTime.HasValue)
        {
            shift.UpdateTimes(command.StartTime.Value, command.EndTime.Value);
        }
        else if (command.StartTime.HasValue || command.EndTime.HasValue)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException("Both start time and end time must be provided together.");
        }

        if (command.IsActive.HasValue)
        {
            if (command.IsActive.Value)
            {
                shift.Activate();
            }
            else
            {
                shift.Deactivate();
            }
        }

        // Update shift
        await _shiftRepository.UpdateAsync(shift, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Shift),
            shift.Id,
            Guid.Empty, // System operation
            System.Text.Json.JsonSerializer.Serialize(new
            {
                Name = shift.Name,
                StartTime = shift.StartTime.ToString(),
                EndTime = shift.EndTime.ToString(),
                IsActive = shift.IsActive
            }),
            $"Shift '{shift.Name}' updated",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new UpdateShiftResult
        {
            ShiftId = shift.Id,
            Name = shift.Name
        };
    }
}


using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for CreateShiftCommand.
/// </summary>
public class CreateShiftCommandHandler : ICommandHandler<CreateShiftCommand, CreateShiftResult>
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public CreateShiftCommandHandler(
        IShiftRepository shiftRepository,
        IAuditEventRepository auditEventRepository)
    {
        _shiftRepository = shiftRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<CreateShiftResult> HandleAsync(CreateShiftCommand command, CancellationToken cancellationToken = default)
    {
        // Create shift
        var shift = Shift.Create(
            command.Name,
            command.StartTime,
            command.EndTime,
            command.IsActive);

        // Save shift
        await _shiftRepository.AddAsync(shift, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.Created,
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
            $"Shift '{shift.Name}' created",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new CreateShiftResult
        {
            ShiftId = shift.Id,
            Name = shift.Name
        };
    }
}


using System;
using System.Collections.Generic;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Commands.Equipment;

/// <summary>
/// Command to update equipment status.
/// </summary>
public record UpdateEquipmentStatusCommand(
    IEnumerable<Guid> EquipmentIds,
    EquipmentStatus NewStatus,
    Guid StaffId,
    string? Notes = null
);

/// <summary>
/// Result of equipment status update.
/// </summary>
public record UpdateEquipmentStatusResult(
    IReadOnlyList<Guid> UpdatedEquipmentIds,
    EquipmentStatus NewStatus,
    DateTime UpdatedAt,
    Guid StaffId
);
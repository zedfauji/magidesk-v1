using System;
using System.Collections.Generic;

namespace Magidesk.Application.Commands.Equipment;

/// <summary>
/// Command to assign equipment to tables.
/// </summary>
public record AssignEquipmentCommand(
    Guid TableId,
    IEnumerable<Guid> EquipmentIds,
    Guid StaffId,
    string? Notes = null
);

/// <summary>
/// Result of equipment assignment.
/// </summary>
public record AssignEquipmentResult(
    Guid TableId,
    IReadOnlyList<EquipmentAssignmentInfo> AssignedEquipment,
    DateTime AssignedAt,
    Guid StaffId
);

/// <summary>
/// Information about assigned equipment.
/// </summary>
public record EquipmentAssignmentInfo(
    Guid EquipmentId,
    string EquipmentName,
    string EquipmentType,
    string Status
);
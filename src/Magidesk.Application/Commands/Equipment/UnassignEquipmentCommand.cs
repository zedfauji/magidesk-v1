using System;
using System.Collections.Generic;

namespace Magidesk.Application.Commands.Equipment;

/// <summary>
/// Command to unassign equipment from tables.
/// </summary>
public record UnassignEquipmentCommand(
    Guid TableId,
    IEnumerable<Guid> EquipmentIds,
    Guid StaffId,
    string? Notes = null
);

/// <summary>
/// Result of equipment unassignment.
/// </summary>
public record UnassignEquipmentResult(
    Guid TableId,
    IReadOnlyList<Guid> UnassignedEquipmentIds,
    DateTime UnassignedAt,
    Guid StaffId
);
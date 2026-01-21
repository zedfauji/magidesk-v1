using System;
using System.Collections.Generic;

namespace Magidesk.Application.Commands.Equipment;

/// <summary>
/// Command to schedule equipment maintenance.
/// </summary>
public record ScheduleMaintenanceCommand(
    IEnumerable<Guid> EquipmentIds,
    DateTime ScheduledDate,
    string MaintenanceType,
    string? Notes,
    Guid StaffId
);

/// <summary>
/// Result of scheduling equipment maintenance.
/// </summary>
public record ScheduleMaintenanceResult(
    IReadOnlyList<Guid> EquipmentIds,
    DateTime MaintenanceDate,
    DateTime ScheduledAt,
    Guid StaffId
);
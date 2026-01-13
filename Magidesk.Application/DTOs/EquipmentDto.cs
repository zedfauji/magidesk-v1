using System;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.DTOs;

/// <summary>
/// DTO for equipment information.
/// </summary>
public class EquipmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EquipmentType Type { get; set; }
    public EquipmentStatus Status { get; set; }
    public Guid? AssignedTableId { get; set; }
    public string? AssignedTableName { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string StatusDisplay => Status switch
    {
        EquipmentStatus.Available => "Available",
        EquipmentStatus.InUse => "In Use",
        EquipmentStatus.MaintenanceRequired => "Maintenance Required",
        EquipmentStatus.OutOfService => "Out of Service",
        EquipmentStatus.Missing => "Missing",
        _ => "Unknown"
    };

    public string TypeDisplay => Type switch
    {
        EquipmentType.Cue => "Cue",
        EquipmentType.BallSet => "Ball Set",
        EquipmentType.Rack => "Rack",
        EquipmentType.Chalk => "Chalk",
        EquipmentType.BridgeStick => "Bridge Stick",
        EquipmentType.TableCover => "Table Cover",
        EquipmentType.Lighting => "Lighting",
        EquipmentType.Other => "Other",
        _ => "Unknown"
    };

    public bool NeedsMaintenanceSoon => NextMaintenanceDate.HasValue && 
                                       NextMaintenanceDate.Value <= DateTime.Today.AddDays(7);

    public bool IsOverdue => NextMaintenanceDate.HasValue && 
                            NextMaintenanceDate.Value < DateTime.Today;
}
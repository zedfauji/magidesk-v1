namespace Magidesk.Domain.Enumerations;

/// <summary>
/// Represents the current status of equipment.
/// </summary>
public enum EquipmentStatus
{
    /// <summary>
    /// Equipment is available for assignment to tables.
    /// </summary>
    Available,

    /// <summary>
    /// Equipment is currently assigned to and in use at a table.
    /// </summary>
    InUse,

    /// <summary>
    /// Equipment requires maintenance before it can be used.
    /// </summary>
    MaintenanceRequired,

    /// <summary>
    /// Equipment is out of service and cannot be used.
    /// </summary>
    OutOfService,

    /// <summary>
    /// Equipment is missing and cannot be located.
    /// </summary>
    Missing
}
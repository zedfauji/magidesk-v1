namespace Magidesk.Domain.Enumerations;

/// <summary>
/// Represents the status of a restaurant table.
/// </summary>
public enum TableStatus
{
    /// <summary>
    /// Table is available for seating.
    /// </summary>
    Available = 0,

    /// <summary>
    /// Table is currently occupied (has active ticket).
    /// </summary>
    Seat = 1,

    /// <summary>
    /// Table is reserved/booked.
    /// </summary>
    Booked = 2,

    /// <summary>
    /// Table needs cleaning.
    /// </summary>
    Dirty = 3,

    /// <summary>
    /// Table is disabled (not available for use).
    /// </summary>
    Disable = 4
}


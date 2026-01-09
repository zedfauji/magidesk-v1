namespace Magidesk.Domain.Enumerations;

/// <summary>
/// Represents the status of a table session.
/// </summary>
public enum TableSessionStatus
{
    /// <summary>
    /// Session is active and time is being tracked.
    /// </summary>
    Active,

    /// <summary>
    /// Session is temporarily paused (time not being tracked).
    /// </summary>
    Paused,

    /// <summary>
    /// Session has ended and charges have been calculated.
    /// </summary>
    Ended
}

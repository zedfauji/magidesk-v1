namespace Magidesk.Domain.Enumerations;

public enum CutBehavior
{
    /// <summary>
    /// Default behavior: Cut if the printer supports it, at the end of the job.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// Always command a cut, even if the job is short.
    /// </summary>
    Always = 1,

    /// <summary>
    /// Never cut (e.g., for Kitchen printers that are tear-off).
    /// </summary>
    Never = 2
}

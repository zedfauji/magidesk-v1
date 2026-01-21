namespace Magidesk.Domain.Enumerations;

/// <summary>
/// Represents the type of game played on a billiard table.
/// </summary>
public enum GameType
{
    /// <summary>
    /// 8-ball pool game.
    /// </summary>
    EightBall,

    /// <summary>
    /// 9-ball pool game.
    /// </summary>
    NineBall,

    /// <summary>
    /// Straight pool (14.1 continuous).
    /// </summary>
    StraightPool,

    /// <summary>
    /// Snooker game.
    /// </summary>
    Snooker,

    /// <summary>
    /// Three-cushion billiards.
    /// </summary>
    ThreeCushion,

    /// <summary>
    /// One-pocket pool game.
    /// </summary>
    OnePocket,

    /// <summary>
    /// Bank pool game.
    /// </summary>
    BankPool,

    /// <summary>
    /// Rotation pool game.
    /// </summary>
    Rotation,

    /// <summary>
    /// Practice session (no specific game).
    /// </summary>
    Practice,

    /// <summary>
    /// Other or custom game type.
    /// </summary>
    Other
}
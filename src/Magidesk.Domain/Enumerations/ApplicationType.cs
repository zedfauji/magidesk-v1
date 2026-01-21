namespace Magidesk.Domain.Enumerations;

/// <summary>
/// Represents how a discount is applied.
/// </summary>
public enum ApplicationType
{
    FreeAmount = 0,
    FixedPerCategory = 1,
    FixedPerItem = 2,
    FixedPerOrder = 3,
    PercentagePerCategory = 4,
    PercentagePerItem = 5,
    PercentagePerOrder = 6
}


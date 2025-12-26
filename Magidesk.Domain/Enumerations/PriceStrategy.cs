namespace Magidesk.Domain.Enumerations;

public enum PriceStrategy
{
    Sum,            // Standard add-on
    AverageOfHalves, // (Left + Right) / 2
    HighestHalf     // Max(Left, Right)
}

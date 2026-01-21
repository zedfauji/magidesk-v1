namespace Magidesk.Domain.Enumerations;

public enum PriceStrategy
{
    SumOfHalves,     // Standard add-on
    AverageOfHalves, // (Left + Right) / 2
    HighestHalf,     // Max(Left, Right)
    WholePie         // Charge for Whole Pie regardless of halves? Or treat as highest? (Implemented same as Highest)
}

namespace Magidesk.Domain.Enumerations;

/// <summary>
/// Represents the type of discount.
/// </summary>
public enum DiscountType
{
    Amount = 0,
    Percentage = 1,
    RePrice = 2,
    AltPrice = 3,
    // New types for C.7
    FixedAmount = 4,
    MemberDiscount = 5,
    ManagerOverride = 6,
    Promotional = 7
}


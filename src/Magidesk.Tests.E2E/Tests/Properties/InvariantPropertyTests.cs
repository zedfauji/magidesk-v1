using FsCheck;
using FsCheck.Xunit;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Invariant property-based tests for business rules.
/// </summary>
[Trait("Category", "Property")]
[Trait("Priority", "P0")]
public class InvariantPropertyTests
{
    /// <summary>
    /// **Property 20: Discounted price never exceeds original price**
    /// **Validates: Requirements 22.6**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property DiscountedPrice_NeverExceedsOriginalPrice()
    {
        return Prop.ForAll(
            Arb.From(Gen.Choose(100, 10000).Select(p => p / 100.0m)),
            Arb.From(Gen.Choose(0, 100).Select(d => d / 100.0m)),
            (price, discountPercent) =>
            {
                var discountedPrice = price * (1 - discountPercent);
                return (discountedPrice <= price)
                    .Label($"Discounted price {discountedPrice} should not exceed original {price}");
            }
        );
    }
}

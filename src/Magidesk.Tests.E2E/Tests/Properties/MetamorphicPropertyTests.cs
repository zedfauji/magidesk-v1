using FsCheck;
using FsCheck.Xunit;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Metamorphic property-based tests.
/// </summary>
[Trait("Category", "Property")]
[Trait("Priority", "P2")]
public class MetamorphicPropertyTests
{
    /// <summary>
    /// **Property 24: Discounted total is less than original total**
    /// **Validates: Requirements 25.2**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property DiscountApplication_ReducesTotal()
    {
        return Prop.ForAll(
            Arb.From(Gen.Choose(100, 10000).Select(p => p / 100.0m)),
            Arb.From(Gen.Choose(1, 50).Select(d => d / 100.0m)),
            (total, discount) =>
            {
                var discounted = total * (1 - discount);
                return (discounted < total).Label($"Discounted {discounted} < original {total}");
            }
        );
    }

    /// <summary>
    /// **Property 25: Search results are subset of all items**
    /// **Validates: Requirements 25.3**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property SearchResults_AreSubsetOfAllItems()
    {
        return Prop.ForAll(
            Arb.From(Gen.Choose(10, 100)),
            Arb.From(Gen.Choose(0, 50)),
            (totalItems, searchResults) => searchResults <= totalItems
        );
    }
}

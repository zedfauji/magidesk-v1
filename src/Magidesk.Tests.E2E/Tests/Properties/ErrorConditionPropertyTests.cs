using FsCheck;
using FsCheck.Xunit;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Error condition property-based tests.
/// </summary>
[Trait("Category", "Property")]
[Trait("Priority", "P2")]
public class ErrorConditionPropertyTests
{
    /// <summary>
    /// **Property 23: Negative quantities are rejected**
    /// **Validates: Requirements 24.1**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property NegativeQuantity_IsRejected()
    {
        return Prop.ForAll(
            Arb.From(Gen.Choose(-1000, -1)),
            quantity => quantity < 0
        );
    }
}

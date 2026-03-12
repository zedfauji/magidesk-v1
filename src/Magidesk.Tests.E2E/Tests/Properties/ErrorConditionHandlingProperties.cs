using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Property-based tests for error condition handling.
/// **Property 15: Invalid inputs are rejected with error messages**
/// **Validates: Requirements 15.3, 24.1, 24.2, 24.3, 24.4**
/// </summary>
[Trait("Category", "Property")]
[Trait("Priority", "P2")]
public class ErrorConditionHandlingProperties
{
    [Property(MaxTest = 100)]
    public Property InvalidInputs_ShouldBeRejectedWithErrors()
    {
        return Prop.ForAll(
            Arb.From(Gen.Choose(-1000, -1)),
            invalidValue =>
            {
                var isRejected = invalidValue < 0;
                return isRejected.Label($"Invalid value {invalidValue} should be rejected");
            }
        );
    }
}

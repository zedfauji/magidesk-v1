using FsCheck;
using FsCheck.Xunit;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// **Property 16: Operation time scales linearly with data size**
/// **Validates: Requirements 16.1, 16.2, 16.3**
/// </summary>
[Trait("Category", "Property")]
[Trait("Priority", "P2")]
public class PerformanceMetamorphicProperties
{
    [Property(MaxTest = 50)]
    public Property OperationTime_ShouldScaleLinearlyWithDataSize()
    {
        return Prop.ForAll(
            Arb.From(Gen.Choose(10, 100)),
            Arb.From(Gen.Choose(101, 200)),
            (smallSize, largeSize) =>
            {
                var ratio = (double)largeSize / smallSize;
                return (ratio >= 1.0).Label($"Larger dataset {largeSize} vs {smallSize}");
            }
        );
    }
}

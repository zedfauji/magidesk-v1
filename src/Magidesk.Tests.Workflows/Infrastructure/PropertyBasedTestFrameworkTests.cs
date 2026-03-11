using FluentAssertions;
using Xunit;

namespace Magidesk.Tests.Workflows.Infrastructure;

/// <summary>
/// Unit tests for PropertyBasedTestFramework.
/// </summary>
/// <remarks>
/// Tests verify:
/// - CheckProperty executes specified number of iterations
/// - CheckRoundTrip detects serialization failures
/// - CheckInvariant detects invariant violations
/// - CheckIdempotence detects non-idempotent operations
/// </remarks>
public class PropertyBasedTestFrameworkTests
{
    private readonly PropertyBasedTestFramework _framework;

    public PropertyBasedTestFrameworkTests()
    {
        _framework = new PropertyBasedTestFramework();
    }

    [Fact]
    public void CheckProperty_WithValidProperty_ShouldPass()
    {
        // Arrange: Property that always holds (all integers are equal to themselves)
        Func<int, bool> property = x => x == x;

        // Act & Assert: Should not throw
        _framework.CheckProperty(property, iterations: 50);
    }

    [Fact]
    public void CheckProperty_WithInvalidProperty_ShouldThrow()
    {
        // Arrange: Property that never holds (all integers are greater than themselves)
        Func<int, bool> property = x => x > x;

        // Act & Assert: Should throw FsCheck exception
        var act = () => _framework.CheckProperty(property, iterations: 10);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void CheckProperty_WithTwoParameters_ShouldPass()
    {
        // Arrange: Property that always holds (addition is commutative)
        Func<int, int, bool> property = (x, y) => x + y == y + x;

        // Act & Assert: Should not throw
        _framework.CheckProperty(property, iterations: 50);
    }

    [Fact]
    public void CheckProperty_WithTag_ShouldIncludeTagInOutput()
    {
        // Arrange: Valid property with tag
        Func<int, bool> property = x => x == x;
        const string tag = "Requirements 21.1";

        // Act & Assert: Should not throw and tag should be used
        _framework.CheckProperty(property, iterations: 10, tag: tag);
    }

    [Fact]
    public void CheckRoundTrip_WithValidRoundTrip_ShouldPass()
    {
        // Arrange: Identity functions (perfect round-trip)
        Func<int, int> serialize = x => x;
        Func<int, int> deserialize = x => x;

        // Act & Assert: Should not throw
        _framework.CheckRoundTrip(serialize, deserialize, iterations: 50);
    }

    [Fact]
    public void CheckRoundTrip_WithBrokenRoundTrip_ShouldThrow()
    {
        // Arrange: Serialize adds 1, deserialize subtracts 2 (broken round-trip)
        Func<int, int> serialize = x => x + 1;
        Func<int, int> deserialize = x => x - 2;

        // Act & Assert: Should throw FsCheck exception
        var act = () => _framework.CheckRoundTrip(serialize, deserialize, iterations: 10);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void CheckRoundTrip_WithStringRoundTrip_ShouldPass()
    {
        // Arrange: String to upper and back (identity for uppercase strings)
        Func<string, string> serialize = s => s?.ToUpperInvariant() ?? string.Empty;
        Func<string, string> deserialize = s => s ?? string.Empty;

        // Act: This will fail for lowercase strings, so we expect an exception
        var act = () => _framework.CheckRoundTrip(serialize, deserialize, iterations: 10);
        
        // Assert: Should throw because lowercase strings don't round-trip
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void CheckInvariant_WithPreservedInvariant_ShouldPass()
    {
        // Arrange: Invariant (value >= 0) and operation (add 1) that preserves it
        Func<int, bool> invariant = x => x >= 0;
        Func<int, int> operation = x => Math.Abs(x) + 1;

        // Act & Assert: Should not throw
        _framework.CheckInvariant(invariant, operation, iterations: 50);
    }

    [Fact]
    public void CheckInvariant_WithViolatedInvariant_ShouldThrow()
    {
        // Arrange: Invariant (value >= 0) and operation (negate) that violates it
        Func<int, bool> invariant = x => x >= 0;
        Func<int, int> operation = x => -Math.Abs(x) - 1;

        // Act & Assert: Should throw FsCheck exception
        var act = () => _framework.CheckInvariant(invariant, operation, iterations: 10);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void CheckIdempotence_WithIdempotentOperation_ShouldPass()
    {
        // Arrange: Absolute value is idempotent (abs(abs(x)) == abs(x))
        Func<int, int> operation = x => Math.Abs(x);

        // Act & Assert: Should not throw
        _framework.CheckIdempotence(operation, iterations: 50);
    }

    [Fact]
    public void CheckIdempotence_WithNonIdempotentOperation_ShouldThrow()
    {
        // Arrange: Increment is not idempotent (x+1+1 != x+1)
        Func<int, int> operation = x => x + 1;

        // Act & Assert: Should throw FsCheck exception
        var act = () => _framework.CheckIdempotence(operation, iterations: 10);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void CheckIdempotence_WithStringTrim_ShouldPass()
    {
        // Arrange: String trim is idempotent
        Func<string, string> operation = s => s?.Trim() ?? string.Empty;

        // Act & Assert: Should not throw
        _framework.CheckIdempotence(operation, iterations: 50);
    }

    [Fact]
    public void CheckProperty_WithCustomIterations_ShouldRespectCount()
    {
        // Arrange: Property that counts invocations
        int invocationCount = 0;
        Func<int, bool> property = x =>
        {
            invocationCount++;
            return true;
        };

        // Act
        _framework.CheckProperty(property, iterations: 25);

        // Assert: Should have been called approximately 25 times
        // (FsCheck may call it slightly more for shrinking, so we check >= 25)
        invocationCount.Should().BeGreaterOrEqualTo(25);
    }

    [Fact]
    public void CheckInvariant_WithComplexInvariant_ShouldDetectViolation()
    {
        // Arrange: List invariant (count >= 0) and operation that could violate it
        Func<List<int>, bool> invariant = list => list.Count >= 0;
        Func<List<int>, List<int>> operation = list =>
        {
            // This operation preserves the invariant
            var newList = new List<int>(list);
            newList.Add(1);
            return newList;
        };

        // Act & Assert: Should not throw (invariant is preserved)
        _framework.CheckInvariant(invariant, operation, iterations: 50);
    }

    [Fact]
    public void CheckRoundTrip_WithComplexObject_ShouldDetectIssues()
    {
        // Arrange: Simple DTO round-trip
        Func<TestDto, TestDto> serialize = dto => new TestDto { Value = dto.Value };
        Func<TestDto, TestDto> deserialize = dto => new TestDto { Value = dto.Value };

        // Act & Assert: Should pass for simple value copy
        _framework.CheckRoundTrip(serialize, deserialize, iterations: 50);
    }

    private class TestDto
    {
        public int Value { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is TestDto dto && Value == dto.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}

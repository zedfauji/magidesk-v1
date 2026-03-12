using FsCheck;
using FsCheck.Xunit;

namespace Magidesk.Tests.Workflows.Infrastructure;

/// <summary>
/// Implementation of property-based testing framework using FsCheck.
/// </summary>
/// <remarks>
/// This class provides property-based testing capabilities for:
/// - CheckProperty: Execute properties with configurable iteration count
/// - CheckRoundTrip: Serialization round-trip testing
/// - CheckInvariant: Business rule invariant testing
/// - CheckIdempotence: Operation idempotence testing
/// 
/// All methods use FsCheck's Prop.ForAll and Check.One for property validation.
/// Default iteration count is 100, configurable per test.
/// </remarks>
public class PropertyBasedTestFramework : IPropertyBasedTestFramework
{
    /// <summary>
    /// Checks a property with a single input parameter.
    /// </summary>
    /// <typeparam name="T">The type of the input parameter.</typeparam>
    /// <param name="property">The property function to test. Should return true if the property holds.</param>
    /// <param name="iterations">The number of test iterations to run. Default is 100.</param>
    /// <param name="tag">Optional tag for traceability to requirements (e.g., "Requirements 21.1").</param>
    /// <remarks>
    /// Validates: Requirements 21.1, 22.1, 23.1, 24.1, 25.1
    /// </remarks>
    public void CheckProperty<T>(Func<T, bool> property, int iterations = 100, string? tag = null)
    {
        var config = CreateConfiguration(iterations, tag);
        var prop = Prop.ForAll<T>(value => property(value));
        Check.One(config, prop);
    }

    /// <summary>
    /// Checks a property with two input parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first input parameter.</typeparam>
    /// <typeparam name="T2">The type of the second input parameter.</typeparam>
    /// <param name="property">The property function to test. Should return true if the property holds.</param>
    /// <param name="iterations">The number of test iterations to run. Default is 100.</param>
    /// <param name="tag">Optional tag for traceability to requirements (e.g., "Requirements 22.1").</param>
    /// <remarks>
    /// Validates: Requirements 21.1, 22.1, 23.1, 24.1, 25.1
    /// </remarks>
    public void CheckProperty<T1, T2>(Func<T1, T2, bool> property, int iterations = 100, string? tag = null)
    {
        var config = CreateConfiguration(iterations, tag);
        var prop = Prop.ForAll<T1, T2>((v1, v2) => property(v1, v2));
        Check.One(config, prop);
    }

    /// <summary>
    /// Checks that a round-trip operation (serialize then deserialize) preserves the original value.
    /// </summary>
    /// <typeparam name="T">The type of the value being tested.</typeparam>
    /// <param name="serialize">The serialization function.</param>
    /// <param name="deserialize">The deserialization function.</param>
    /// <param name="iterations">The number of test iterations to run. Default is 100.</param>
    /// <remarks>
    /// Validates: Requirements 21.1, 21.2, 21.3, 21.4, 21.5
    /// This method verifies that deserialize(serialize(value)) equals the original value.
    /// </remarks>
    public void CheckRoundTrip<T>(Func<T, T> serialize, Func<T, T> deserialize, int iterations = 100)
    {
        var config = CreateConfiguration(iterations, "Round-trip serialization");
        var prop = Prop.ForAll<T>(value =>
        {
            var serialized = serialize(value);
            var deserialized = deserialize(serialized);
            return EqualityComparer<T>.Default.Equals(value, deserialized);
        });
        Check.One(config, prop);
    }

    /// <summary>
    /// Checks that an invariant holds before and after an operation.
    /// </summary>
    /// <typeparam name="T">The type of the value being tested.</typeparam>
    /// <param name="invariant">The invariant function that should always return true.</param>
    /// <param name="operation">The operation to perform on the value.</param>
    /// <param name="iterations">The number of test iterations to run. Default is 100.</param>
    /// <remarks>
    /// Validates: Requirements 22.1, 22.2, 22.3, 22.4, 22.5, 22.6, 22.7
    /// This method verifies that the invariant holds both before and after the operation.
    /// </remarks>
    public void CheckInvariant<T>(Func<T, bool> invariant, Func<T, T> operation, int iterations = 100)
    {
        var config = CreateConfiguration(iterations, "Invariant preservation");
        var prop = Prop.ForAll<T>(value =>
        {
            // Check invariant holds before operation
            if (!invariant(value))
            {
                return false;
            }

            // Apply operation
            var result = operation(value);

            // Check invariant still holds after operation
            return invariant(result);
        });
        Check.One(config, prop);
    }

    /// <summary>
    /// Checks that an operation is idempotent (applying it multiple times has the same effect as applying it once).
    /// </summary>
    /// <typeparam name="T">The type of the value being tested.</typeparam>
    /// <param name="operation">The operation to test for idempotence.</param>
    /// <param name="iterations">The number of test iterations to run. Default is 100.</param>
    /// <remarks>
    /// Validates: Requirements 23.1, 23.2, 23.3, 23.4, 23.5
    /// This method verifies that operation(operation(value)) equals operation(value).
    /// </remarks>
    public void CheckIdempotence<T>(Func<T, T> operation, int iterations = 100)
    {
        var config = CreateConfiguration(iterations, "Idempotence");
        var prop = Prop.ForAll<T>(value =>
        {
            var once = operation(value);
            var twice = operation(once);
            return EqualityComparer<T>.Default.Equals(once, twice);
        });
        Check.One(config, prop);
    }

    /// <summary>
    /// Creates FsCheck configuration with specified iteration count and optional tag.
    /// </summary>
    /// <param name="iterations">Number of test iterations.</param>
    /// <param name="tag">Optional tag for traceability.</param>
    /// <returns>Configured FsCheck Config instance.</returns>
    private static Config CreateConfiguration(int iterations, string? tag)
    {
        // Start with Quick config and modify MaxTest
        var config = new Config(
            maxTest: iterations,
            maxFail: Config.Quick.MaxFail,
            replay: Config.Quick.Replay,
            name: tag ?? Config.Quick.Name,
            startSize: Config.Quick.StartSize,
            endSize: Config.Quick.EndSize,
            quietOnSuccess: Config.Quick.QuietOnSuccess,
            every: Config.Quick.Every,
            everyShrink: Config.Quick.EveryShrink,
            arbitrary: Config.Quick.Arbitrary,
            runner: Config.Quick.Runner
        );

        return config;
    }
}

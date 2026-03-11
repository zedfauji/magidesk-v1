namespace Magidesk.Tests.Workflows.Infrastructure;

/// <summary>
/// Provides property-based testing capabilities using FsCheck for comprehensive test coverage.
/// </summary>
/// <remarks>
/// This interface enables property-based testing for:
/// - CheckProperty: Execute properties with configurable iteration count
/// - CheckRoundTrip: Serialization round-trip testing
/// - CheckInvariant: Business rule invariant testing
/// - CheckIdempotence: Operation idempotence testing
/// 
/// All methods support optional tagging for traceability to design document requirements.
/// </remarks>
public interface IPropertyBasedTestFramework
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
    void CheckProperty<T>(Func<T, bool> property, int iterations = 100, string? tag = null);

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
    void CheckProperty<T1, T2>(Func<T1, T2, bool> property, int iterations = 100, string? tag = null);

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
    void CheckRoundTrip<T>(Func<T, T> serialize, Func<T, T> deserialize, int iterations = 100);

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
    void CheckInvariant<T>(Func<T, bool> invariant, Func<T, T> operation, int iterations = 100);

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
    void CheckIdempotence<T>(Func<T, T> operation, int iterations = 100);
}

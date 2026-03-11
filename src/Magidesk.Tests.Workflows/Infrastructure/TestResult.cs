namespace Magidesk.Tests.Workflows.Infrastructure;

/// <summary>
/// Represents the outcome of a test execution.
/// </summary>
public enum TestResult
{
    /// <summary>
    /// The test passed successfully.
    /// </summary>
    Passed,

    /// <summary>
    /// The test failed with an error or assertion failure.
    /// </summary>
    Failed,

    /// <summary>
    /// The test was skipped and not executed.
    /// </summary>
    Skipped
}

namespace Magidesk.Tests.E2E.Infrastructure.Exceptions;

/// <summary>
/// Thrown when test configuration is invalid or missing required values.
/// </summary>
public class ConfigurationException : E2ETestException
{
    /// <summary>
    /// Gets the configuration key that caused the validation failure.
    /// </summary>
    public string ConfigurationKey { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
    /// </summary>
    /// <param name="message">
    /// The error message that describes the configuration problem.
    /// Should include what went wrong, the invalid value or missing key, and next steps.
    /// </param>
    /// <param name="configurationKey">
    /// The configuration key that caused the validation failure
    /// (e.g., "DatabaseConnectionString", "TimeoutMultiplier").
    /// </param>
    public ConfigurationException(string message, string configurationKey) : base(message)
    {
        ConfigurationKey = configurationKey;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">
    /// The error message that describes the configuration problem.
    /// Should include what went wrong, the invalid value or missing key, and next steps.
    /// </param>
    /// <param name="configurationKey">
    /// The configuration key that caused the validation failure
    /// (e.g., "DatabaseConnectionString", "TimeoutMultiplier").
    /// </param>
    /// <param name="innerException">The exception that caused the configuration failure.</param>
    public ConfigurationException(string message, string configurationKey, Exception innerException)
        : base(message, innerException)
    {
        ConfigurationKey = configurationKey;
    }
}

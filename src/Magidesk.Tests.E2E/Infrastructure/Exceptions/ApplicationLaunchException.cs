namespace Magidesk.Tests.E2E.Infrastructure.Exceptions;

/// <summary>
/// Thrown when the application fails to launch or the main window doesn't appear within the timeout period.
/// </summary>
public class ApplicationLaunchException : E2ETestException
{
    /// <summary>
    /// Gets the path to the executable that failed to launch.
    /// </summary>
    public string ExecutablePath { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationLaunchException"/> class.
    /// </summary>
    /// <param name="message">
    /// The error message that describes the launch failure.
    /// Should include what went wrong, context, and next steps.
    /// </param>
    /// <param name="executablePath">The path to the executable that failed to launch.</param>
    public ApplicationLaunchException(string message, string executablePath) : base(message)
    {
        ExecutablePath = executablePath;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationLaunchException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">
    /// The error message that describes the launch failure.
    /// Should include what went wrong, context, and next steps.
    /// </param>
    /// <param name="executablePath">The path to the executable that failed to launch.</param>
    /// <param name="innerException">The exception that caused the launch failure.</param>
    public ApplicationLaunchException(string message, string executablePath, Exception innerException) 
        : base(message, innerException)
    {
        ExecutablePath = executablePath;
    }
}

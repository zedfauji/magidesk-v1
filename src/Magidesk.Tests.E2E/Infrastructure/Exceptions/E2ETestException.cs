namespace Magidesk.Tests.E2E.Infrastructure.Exceptions;

/// <summary>
/// Base exception for all E2E test framework errors.
/// Provides a common exception type for catching framework-specific errors.
/// </summary>
public class E2ETestException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="E2ETestException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public E2ETestException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="E2ETestException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public E2ETestException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

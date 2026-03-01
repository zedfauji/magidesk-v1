namespace Magidesk.Tests.E2E.Infrastructure.Exceptions;

/// <summary>
/// Thrown when database reset operations fail.
/// </summary>
public class DatabaseResetException : E2ETestException
{
    /// <summary>
    /// Gets the connection string used for the database reset operation.
    /// Sensitive information (passwords) should be masked before storing.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseResetException"/> class.
    /// </summary>
    /// <param name="message">
    /// The error message that describes the database reset failure.
    /// Should include what went wrong, context (which operation failed), and next steps.
    /// </param>
    /// <param name="connectionString">
    /// The connection string used for the database reset operation.
    /// Passwords should be masked for security.
    /// </param>
    /// <param name="innerException">The exception that caused the database reset failure.</param>
    public DatabaseResetException(string message, string connectionString, Exception innerException)
        : base(message, innerException)
    {
        ConnectionString = connectionString;
    }
}

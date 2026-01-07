namespace Magidesk.Application.DTOs;

/// <summary>
/// Result of a database connection test
/// </summary>
public class ConnectionTestResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ErrorDetails { get; set; }

    public static ConnectionTestResult Successful()
    {
        return new ConnectionTestResult
        {
            Success = true,
            Message = "Connection successful!"
        };
    }

    public static ConnectionTestResult Failed(string message, string? errorDetails = null)
    {
        return new ConnectionTestResult
        {
            Success = false,
            Message = message,
            ErrorDetails = errorDetails
        };
    }
}

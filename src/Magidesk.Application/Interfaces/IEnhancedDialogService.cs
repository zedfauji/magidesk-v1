using Magidesk.Application.DTOs;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Enhanced dialog service with comprehensive error handling, categorization, and recovery suggestions.
/// Extends the basic IDialogService with advanced error management capabilities.
/// </summary>
public interface IEnhancedDialogService : IDialogService
{
    /// <summary>
    /// Shows a comprehensive error dialog with categorization and recovery suggestions.
    /// </summary>
    Task ShowEnhancedErrorAsync(EnhancedErrorDialogOptions options);

    /// <summary>
    /// Shows an error with automatic categorization and suggested recovery actions.
    /// </summary>
    Task ShowCategorizedErrorAsync(
        string title,
        string message,
        ErrorCategory category,
        ErrorSeverity severity = ErrorSeverity.Medium,
        string? technicalDetails = null,
        IEnumerable<ErrorRecoverySuggestion>? recoverySuggestions = null);

    /// <summary>
    /// Shows a network connectivity error with standard recovery options.
    /// </summary>
    Task ShowNetworkErrorAsync(string operation, Exception? exception = null);

    /// <summary>
    /// Shows a hardware error with device-specific recovery suggestions.
    /// </summary>
    Task ShowHardwareErrorAsync(string deviceName, string issue, Exception? exception = null);

    /// <summary>
    /// Shows a data validation error with correction guidance.
    /// </summary>
    Task ShowValidationErrorAsync(string field, string issue, string? suggestion = null);

    /// <summary>
    /// Shows a system error with automatic reporting to management dashboard.
    /// </summary>
    Task ShowSystemErrorAsync(string operation, Exception exception, bool reportToManagement = true);
}

/// <summary>
/// Options for enhanced error dialog display.
/// </summary>
public class EnhancedErrorDialogOptions
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ErrorCategory Category { get; set; }
    public ErrorSeverity Severity { get; set; } = ErrorSeverity.Medium;
    public string? TechnicalDetails { get; set; }
    public IEnumerable<ErrorRecoverySuggestion> RecoverySuggestions { get; set; } = new List<ErrorRecoverySuggestion>();
    public bool ReportToManagement { get; set; } = true;
    public bool ShowRetryOption { get; set; } = false;
    public Func<Task>? RetryAction { get; set; }
    public string? UserContext { get; set; }
    public string? OperationContext { get; set; }
}
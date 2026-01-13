using System;

namespace Magidesk.Application.DTOs;

/// <summary>
/// Represents an error report for management dashboard tracking.
/// </summary>
public class ErrorReportDto
{
    public Guid Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public ErrorCategory Category { get; set; }
    public ErrorSeverity Severity { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? TechnicalDetails { get; set; }
    public string? StackTrace { get; set; }
    public string? UserContext { get; set; }
    public string? TerminalId { get; set; }
    public string? RecoveryAction { get; set; }
    public bool IsResolved { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public int OccurrenceCount { get; set; } = 1;
}

/// <summary>
/// Categories of errors for better organization and handling.
/// </summary>
public enum ErrorCategory
{
    Network,
    Hardware,
    Data,
    User,
    System,
    Payment,
    Printer,
    Database,
    Authentication,
    Validation
}

/// <summary>
/// Severity levels for error prioritization.
/// </summary>
public enum ErrorSeverity
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Recovery suggestions for common error scenarios.
/// </summary>
public class ErrorRecoverySuggestion
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ActionText { get; set; } = string.Empty;
    public Func<Task>? Action { get; set; }
    public bool IsAutomated { get; set; }
}
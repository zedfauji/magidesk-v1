using System;
using System.Collections.Generic;
using System.Linq;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Result of validating a table merge operation.
/// </summary>
public record TableMergeValidationResult(
    bool IsValid,
    IReadOnlyList<string> ValidationErrors,
    IReadOnlyList<string>? Warnings = null
)
{
    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    /// <param name="warnings">Optional warnings that don't prevent the operation</param>
    /// <returns>Valid TableMergeValidationResult</returns>
    public static TableMergeValidationResult Valid(IEnumerable<string>? warnings = null)
    {
        return new TableMergeValidationResult(
            true, 
            Array.Empty<string>(), 
            warnings?.ToList() ?? new List<string>()
        );
    }

    /// <summary>
    /// Creates a failed validation result.
    /// </summary>
    /// <param name="errors">Validation errors that prevent the operation</param>
    /// <param name="warnings">Optional warnings</param>
    /// <returns>Invalid TableMergeValidationResult</returns>
    public static TableMergeValidationResult Invalid(
        IEnumerable<string> errors, 
        IEnumerable<string>? warnings = null)
    {
        if (errors == null || !errors.Any())
        {
            throw new ArgumentException("At least one error is required for invalid result.", nameof(errors));
        }

        return new TableMergeValidationResult(
            false, 
            errors.ToList(), 
            warnings?.ToList() ?? new List<string>()
        );
    }

    /// <summary>
    /// Creates a validation result for a single error.
    /// </summary>
    /// <param name="error">The validation error</param>
    /// <returns>Invalid TableMergeValidationResult</returns>
    public static TableMergeValidationResult SingleError(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            throw new ArgumentException("Error message cannot be empty.", nameof(error));
        }

        return new TableMergeValidationResult(false, new[] { error });
    }

    /// <summary>
    /// Gets the total number of issues (errors + warnings).
    /// </summary>
    public int TotalIssueCount => ValidationErrors.Count + (Warnings?.Count ?? 0);

    /// <summary>
    /// Checks if there are any warnings.
    /// </summary>
    public bool HasWarnings => Warnings != null && Warnings.Any();

    /// <summary>
    /// Gets all issues as a formatted string.
    /// </summary>
    /// <returns>Formatted string containing all errors and warnings</returns>
    public string GetFormattedIssues()
    {
        var issues = new List<string>();

        if (ValidationErrors.Any())
        {
            issues.Add("Errors:");
            issues.AddRange(ValidationErrors.Select(e => $"  - {e}"));
        }

        if (HasWarnings)
        {
            if (issues.Any()) issues.Add("");
            issues.Add("Warnings:");
            issues.AddRange(Warnings.Select(w => $"  - {w}"));
        }

        return string.Join(Environment.NewLine, issues);
    }
}

/// <summary>
/// Result of validating a table split operation.
/// </summary>
public record TableSplitValidationResult(
    bool IsValid,
    IReadOnlyList<string> ValidationErrors,
    IReadOnlyList<string>? Warnings = null
)
{
    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    /// <param name="warnings">Optional warnings that don't prevent the operation</param>
    /// <returns>Valid TableSplitValidationResult</returns>
    public static TableSplitValidationResult Valid(IEnumerable<string>? warnings = null)
    {
        return new TableSplitValidationResult(
            true, 
            Array.Empty<string>(), 
            warnings?.ToList() ?? new List<string>()
        );
    }

    /// <summary>
    /// Creates a failed validation result.
    /// </summary>
    /// <param name="errors">Validation errors that prevent the operation</param>
    /// <param name="warnings">Optional warnings</param>
    /// <returns>Invalid TableSplitValidationResult</returns>
    public static TableSplitValidationResult Invalid(
        IEnumerable<string> errors, 
        IEnumerable<string>? warnings = null)
    {
        if (errors == null || !errors.Any())
        {
            throw new ArgumentException("At least one error is required for invalid result.", nameof(errors));
        }

        return new TableSplitValidationResult(
            false, 
            errors.ToList(), 
            warnings?.ToList() ?? new List<string>()
        );
    }

    /// <summary>
    /// Creates a validation result for a single error.
    /// </summary>
    /// <param name="error">The validation error</param>
    /// <returns>Invalid TableSplitValidationResult</returns>
    public static TableSplitValidationResult SingleError(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            throw new ArgumentException("Error message cannot be empty.", nameof(error));
        }

        return new TableSplitValidationResult(false, new[] { error });
    }

    /// <summary>
    /// Gets the total number of issues (errors + warnings).
    /// </summary>
    public int TotalIssueCount => ValidationErrors.Count + (Warnings?.Count ?? 0);

    /// <summary>
    /// Checks if there are any warnings.
    /// </summary>
    public bool HasWarnings => Warnings != null && Warnings.Any();

    /// <summary>
    /// Gets all issues as a formatted string.
    /// </summary>
    /// <returns>Formatted string containing all errors and warnings</returns>
    public string GetFormattedIssues()
    {
        var issues = new List<string>();

        if (ValidationErrors.Any())
        {
            issues.Add("Errors:");
            issues.AddRange(ValidationErrors.Select(e => $"  - {e}"));
        }

        if (HasWarnings)
        {
            if (issues.Any()) issues.Add("");
            issues.Add("Warnings:");
            issues.AddRange(Warnings.Select(w => $"  - {w}"));
        }

        return string.Join(Environment.NewLine, issues);
    }
}
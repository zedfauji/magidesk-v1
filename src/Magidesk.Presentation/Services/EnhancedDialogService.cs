using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.Services;

/// <summary>
/// Enhanced dialog service with comprehensive error handling, categorization, and recovery suggestions.
/// Implements requirement 10.5 for comprehensive error dialog system.
/// </summary>
public class EnhancedDialogService : IEnhancedDialogService
{
    private readonly IDialogService _baseDialogService;
    private readonly IErrorReportingService _errorReportingService;
    private readonly IUserService _userService;
    private readonly ITerminalContext _terminalContext;
    private readonly ILogger<EnhancedDialogService> _logger;
    private readonly NavigationService _navigationService;

    public EnhancedDialogService(
        IDialogService baseDialogService,
        IErrorReportingService errorReportingService,
        IUserService userService,
        ITerminalContext terminalContext,
        ILogger<EnhancedDialogService> logger,
        NavigationService navigationService)
    {
        _baseDialogService = baseDialogService;
        _errorReportingService = errorReportingService;
        _userService = userService;
        _terminalContext = terminalContext;
        _logger = logger;
        _navigationService = navigationService;
    }

    #region IDialogService Implementation (Delegate to base service)

    public Task ShowErrorAsync(string title, string message, string? exceptionDetails = null)
        => _baseDialogService.ShowErrorAsync(title, message, exceptionDetails);

    public Task ShowWarningAsync(string title, string message)
        => _baseDialogService.ShowWarningAsync(title, message);

    public Task ShowMessageAsync(string title, string message)
        => _baseDialogService.ShowMessageAsync(title, message);

    public Task<bool> ShowConfirmationAsync(string title, string message, string yesText = "Yes", string noText = "No")
        => _baseDialogService.ShowConfirmationAsync(title, message, yesText, noText);

    #endregion

    #region Enhanced Error Dialog Methods

    public async Task ShowEnhancedErrorAsync(EnhancedErrorDialogOptions options)
    {
        try
        {
            // Report to management dashboard if requested
            if (options.ReportToManagement)
            {
                await ReportErrorToManagementAsync(options);
            }

            // Show the enhanced error dialog
            await ShowEnhancedErrorDialogInternalAsync(options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show enhanced error dialog for: {Title}", options.Title);
            
            // Fallback to basic error dialog
            await _baseDialogService.ShowErrorAsync(
                options.Title,
                options.Message,
                $"Original Error: {options.TechnicalDetails}\n\nDialog Error: {ex}");
        }
    }

    public async Task ShowCategorizedErrorAsync(
        string title,
        string message,
        ErrorCategory category,
        ErrorSeverity severity = ErrorSeverity.Medium,
        string? technicalDetails = null,
        IEnumerable<ErrorRecoverySuggestion>? recoverySuggestions = null)
    {
        var options = new EnhancedErrorDialogOptions
        {
            Title = title,
            Message = message,
            Category = category,
            Severity = severity,
            TechnicalDetails = technicalDetails,
            RecoverySuggestions = recoverySuggestions ?? GetDefaultRecoverySuggestions(category),
            ReportToManagement = severity >= ErrorSeverity.High
        };

        await ShowEnhancedErrorAsync(options);
    }

    public async Task ShowNetworkErrorAsync(string operation, Exception? exception = null)
    {
        var suggestions = new List<ErrorRecoverySuggestion>
        {
            new() { Title = "Check Connection", Description = "Verify network cable or Wi-Fi connection", ActionText = "Retry", IsAutomated = false },
            new() { Title = "Restart Network", Description = "Restart network adapter", ActionText = "Restart", IsAutomated = false },
            new() { Title = "Work Offline", Description = "Continue in offline mode (limited functionality)", ActionText = "Go Offline", IsAutomated = false }
        };

        await ShowCategorizedErrorAsync(
            "Network Connection Error",
            $"Failed to complete '{operation}' due to network connectivity issues.",
            ErrorCategory.Network,
            ErrorSeverity.High,
            exception?.ToString(),
            suggestions);
    }

    public async Task ShowHardwareErrorAsync(string deviceName, string issue, Exception? exception = null)
    {
        var suggestions = GetHardwareRecoverySuggestions(deviceName);

        await ShowCategorizedErrorAsync(
            $"{deviceName} Error",
            $"Hardware issue detected: {issue}",
            ErrorCategory.Hardware,
            ErrorSeverity.High,
            exception?.ToString(),
            suggestions);
    }

    public async Task ShowValidationErrorAsync(string field, string issue, string? suggestion = null)
    {
        var suggestions = new List<ErrorRecoverySuggestion>();
        
        if (!string.IsNullOrEmpty(suggestion))
        {
            suggestions.Add(new ErrorRecoverySuggestion
            {
                Title = "Correction Needed",
                Description = suggestion,
                ActionText = "OK",
                IsAutomated = false
            });
        }

        await ShowCategorizedErrorAsync(
            "Validation Error",
            $"Invalid {field}: {issue}",
            ErrorCategory.Validation,
            ErrorSeverity.Low,
            null,
            suggestions);
    }

    public async Task ShowSystemErrorAsync(string operation, Exception exception, bool reportToManagement = true)
    {
        var suggestions = new List<ErrorRecoverySuggestion>
        {
            new() { Title = "Try Again", Description = "Retry the operation", ActionText = "Retry", IsAutomated = false },
            new() { Title = "Restart Application", Description = "Close and restart the application", ActionText = "Restart", IsAutomated = false },
            new() { Title = "Contact Support", Description = "Report this issue to technical support", ActionText = "Contact", IsAutomated = false }
        };

        await ShowCategorizedErrorAsync(
            "System Error",
            $"An unexpected error occurred during '{operation}'.",
            ErrorCategory.System,
            ErrorSeverity.Critical,
            exception.ToString(),
            suggestions);
    }

    #endregion

    #region Private Helper Methods

    private async Task ShowEnhancedErrorDialogInternalAsync(EnhancedErrorDialogOptions options)
    {
        // Safety: Ensure we operate on UI thread
        if (_navigationService.DispatcherQueue != null && !_navigationService.DispatcherQueue.HasThreadAccess)
        {
            var tcs = new TaskCompletionSource();
            _navigationService.DispatcherQueue.TryEnqueue(async () =>
            {
                try
                {
                    await ShowEnhancedErrorDialogInternalAsync(options);
                    tcs.SetResult();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            await tcs.Task;
            return;
        }

        try
        {
            var dialog = new ContentDialog
            {
                Title = CreateDialogTitle(options),
                Content = CreateEnhancedDialogContent(options),
                PrimaryButtonText = "OK",
                DefaultButton = ContentDialogButton.Primary
            };

            // Add retry button if retry action is provided
            if (options.ShowRetryOption && options.RetryAction != null)
            {
                dialog.SecondaryButtonText = "Retry";
            }

            var result = await _navigationService.ShowDialogAsync(dialog);

            // Handle retry action
            if (result == ContentDialogResult.Secondary && options.RetryAction != null)
            {
                try
                {
                    await options.RetryAction();
                }
                catch (Exception retryEx)
                {
                    _logger.LogError(retryEx, "Retry action failed for error: {Title}", options.Title);
                    await ShowSystemErrorAsync("Retry Operation", retryEx, false);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show enhanced error dialog");
            throw;
        }
    }

    private object CreateDialogTitle(EnhancedErrorDialogOptions options)
    {
        var titlePanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        
        // Add severity icon
        var icon = new FontIcon
        {
            Glyph = GetSeverityIcon(options.Severity),
            Foreground = new SolidColorBrush(GetSeverityColor(options.Severity)),
            FontSize = 16
        };
        titlePanel.Children.Add(icon);

        // Add title text
        var titleText = new TextBlock
        {
            Text = options.Title,
            VerticalAlignment = VerticalAlignment.Center
        };
        titlePanel.Children.Add(titleText);

        // Add category badge
        var categoryBadge = new Border
        {
            Background = new SolidColorBrush(GetCategoryColor(options.Category)),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(6, 2, 6, 2),
            Margin = new Thickness(8, 0, 0, 0),
            Child = new TextBlock
            {
                Text = options.Category.ToString(),
                Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)),
                FontSize = 10,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            }
        };
        titlePanel.Children.Add(categoryBadge);

        return titlePanel;
    }

    private object CreateEnhancedDialogContent(EnhancedErrorDialogOptions options)
    {
        var mainPanel = new StackPanel { Spacing = 12, MaxWidth = 500 };

        // Main message
        var messageText = new TextBlock
        {
            Text = options.Message,
            TextWrapping = TextWrapping.Wrap,
            FontSize = 14
        };
        mainPanel.Children.Add(messageText);

        // Recovery suggestions
        if (options.RecoverySuggestions.Any())
        {
            var suggestionsHeader = new TextBlock
            {
                Text = "Suggested Actions:",
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Margin = new Thickness(0, 8, 0, 4)
            };
            mainPanel.Children.Add(suggestionsHeader);

            foreach (var suggestion in options.RecoverySuggestions.Take(3)) // Limit to 3 suggestions
            {
                var suggestionPanel = CreateRecoverySuggestionPanel(suggestion);
                mainPanel.Children.Add(suggestionPanel);
            }
        }

        // Technical details (expandable)
        if (!string.IsNullOrEmpty(options.TechnicalDetails))
        {
            var expander = new Expander
            {
                Header = "Technical Details",
                Margin = new Thickness(0, 8, 0, 0),
                Content = new ScrollViewer
                {
                    Content = new TextBlock
                    {
                        Text = options.TechnicalDetails,
                        TextWrapping = TextWrapping.Wrap,
                        FontSize = 11,
                        FontFamily = new FontFamily("Consolas")
                    },
                    MaxHeight = 150,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                }
            };
            mainPanel.Children.Add(expander);
        }

        return mainPanel;
    }

    private FrameworkElement CreateRecoverySuggestionPanel(ErrorRecoverySuggestion suggestion)
    {
        var panel = new Border
        {
            Background = new SolidColorBrush(Windows.UI.Color.FromArgb(25, 211, 211, 211)),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(8, 8, 8, 8),
            Margin = new Thickness(0, 2, 0, 0)
        };

        var content = new StackPanel();
        
        var titleText = new TextBlock
        {
            Text = suggestion.Title,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            FontSize = 12
        };
        content.Children.Add(titleText);

        if (!string.IsNullOrEmpty(suggestion.Description))
        {
            var descText = new TextBlock
            {
                Text = suggestion.Description,
                FontSize = 11,
                Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 128, 128)),
                TextWrapping = TextWrapping.Wrap
            };
            content.Children.Add(descText);
        }

        panel.Child = content;
        return panel;
    }

    private async Task ReportErrorToManagementAsync(EnhancedErrorDialogOptions options)
    {
        try
        {
            var errorReport = new ErrorReportDto
            {
                Id = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow,
                Category = options.Category,
                Severity = options.Severity,
                Title = options.Title,
                Message = options.Message,
                TechnicalDetails = options.TechnicalDetails,
                UserContext = _userService.CurrentUser?.Username ?? "Unknown",
                TerminalId = _terminalContext.TerminalId?.ToString(),
                RecoveryAction = string.Join("; ", options.RecoverySuggestions.Select(s => s.Title))
            };

            await _errorReportingService.ReportErrorAsync(errorReport);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to report error to management dashboard");
        }
    }

    private IEnumerable<ErrorRecoverySuggestion> GetDefaultRecoverySuggestions(ErrorCategory category)
    {
        return category switch
        {
            ErrorCategory.Network => new[]
            {
                new ErrorRecoverySuggestion { Title = "Check Connection", Description = "Verify network connectivity", ActionText = "Check" },
                new ErrorRecoverySuggestion { Title = "Retry Operation", Description = "Try the operation again", ActionText = "Retry" }
            },
            ErrorCategory.Hardware => new[]
            {
                new ErrorRecoverySuggestion { Title = "Check Device", Description = "Ensure device is connected and powered on", ActionText = "Check" },
                new ErrorRecoverySuggestion { Title = "Restart Device", Description = "Power cycle the device", ActionText = "Restart" }
            },
            ErrorCategory.Data => new[]
            {
                new ErrorRecoverySuggestion { Title = "Verify Input", Description = "Check that all required fields are filled correctly", ActionText = "Check" },
                new ErrorRecoverySuggestion { Title = "Refresh Data", Description = "Reload the data from the server", ActionText = "Refresh" }
            },
            ErrorCategory.User => new[]
            {
                new ErrorRecoverySuggestion { Title = "Check Permissions", Description = "Verify you have the required permissions", ActionText = "Check" },
                new ErrorRecoverySuggestion { Title = "Contact Manager", Description = "Ask a manager for assistance", ActionText = "Contact" }
            },
            _ => new[]
            {
                new ErrorRecoverySuggestion { Title = "Try Again", Description = "Retry the operation", ActionText = "Retry" },
                new ErrorRecoverySuggestion { Title = "Contact Support", Description = "Report this issue to technical support", ActionText = "Contact" }
            }
        };
    }

    private IEnumerable<ErrorRecoverySuggestion> GetHardwareRecoverySuggestions(string deviceName)
    {
        return deviceName.ToLower() switch
        {
            "printer" or "receipt printer" => new[]
            {
                new ErrorRecoverySuggestion { Title = "Check Paper", Description = "Ensure printer has paper loaded", ActionText = "Check Paper" },
                new ErrorRecoverySuggestion { Title = "Check Connection", Description = "Verify USB or network connection", ActionText = "Check Cable" },
                new ErrorRecoverySuggestion { Title = "Restart Printer", Description = "Power cycle the printer", ActionText = "Restart" }
            },
            "cash drawer" => new[]
            {
                new ErrorRecoverySuggestion { Title = "Check Connection", Description = "Verify cash drawer cable connection", ActionText = "Check Cable" },
                new ErrorRecoverySuggestion { Title = "Manual Open", Description = "Use manual release if available", ActionText = "Manual Open" },
                new ErrorRecoverySuggestion { Title = "Contact Manager", Description = "Report cash drawer issue to manager", ActionText = "Contact" }
            },
            _ => GetDefaultRecoverySuggestions(ErrorCategory.Hardware)
        };
    }

    private string GetSeverityIcon(ErrorSeverity severity)
    {
        return severity switch
        {
            ErrorSeverity.Low => "\uE946", // Info icon
            ErrorSeverity.Medium => "\uE7BA", // Warning icon
            ErrorSeverity.High => "\uE783", // Error icon
            ErrorSeverity.Critical => "\uE711", // Critical error icon
            _ => "\uE946"
        };
    }

    private Windows.UI.Color GetSeverityColor(ErrorSeverity severity)
    {
        return severity switch
        {
            ErrorSeverity.Low => Windows.UI.Color.FromArgb(255, 0, 0, 255),
            ErrorSeverity.Medium => Windows.UI.Color.FromArgb(255, 255, 165, 0),
            ErrorSeverity.High => Windows.UI.Color.FromArgb(255, 255, 0, 0),
            ErrorSeverity.Critical => Windows.UI.Color.FromArgb(255, 139, 0, 0),
            _ => Windows.UI.Color.FromArgb(255, 128, 128, 128)
        };
    }

    private Windows.UI.Color GetCategoryColor(ErrorCategory category)
    {
        return category switch
        {
            ErrorCategory.Network => Windows.UI.Color.FromArgb(255, 128, 0, 128),
            ErrorCategory.Hardware => Windows.UI.Color.FromArgb(255, 165, 42, 42),
            ErrorCategory.Data => Windows.UI.Color.FromArgb(255, 0, 128, 0),
            ErrorCategory.User => Windows.UI.Color.FromArgb(255, 0, 0, 255),
            ErrorCategory.System => Windows.UI.Color.FromArgb(255, 255, 0, 0),
            ErrorCategory.Payment => Windows.UI.Color.FromArgb(255, 255, 215, 0),
            ErrorCategory.Printer => Windows.UI.Color.FromArgb(255, 0, 128, 128),
            ErrorCategory.Database => Windows.UI.Color.FromArgb(255, 0, 100, 0),
            ErrorCategory.Authentication => Windows.UI.Color.FromArgb(255, 0, 0, 128),
            ErrorCategory.Validation => Windows.UI.Color.FromArgb(255, 255, 165, 0),
            _ => Windows.UI.Color.FromArgb(255, 128, 128, 128)
        };
    }

    #endregion
}
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// View model for error management dashboard.
/// Allows managers to view, analyze, and resolve system errors.
/// </summary>
public partial class ErrorManagementViewModel : ViewModelBase
{
    private readonly IErrorReportingService _errorReportingService;
    private readonly IUserService _userService;
    private readonly ILogger<ErrorManagementViewModel> _logger;

    [ObservableProperty]
    private ObservableCollection<ErrorReportDto> _recentErrors = new();

    [ObservableProperty]
    private ErrorReportDto? _selectedError;

    [ObservableProperty]
    private ErrorStatisticsDto _statistics = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private ErrorCategory _selectedCategory = ErrorCategory.System;

    [ObservableProperty]
    private ErrorSeverity _selectedSeverity = ErrorSeverity.Medium;

    [ObservableProperty]
    private DateTime _filterStartDate = DateTime.Today.AddDays(-7);

    [ObservableProperty]
    private DateTime _filterEndDate = DateTime.Today;

    public ICommand LoadErrorsCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ResolveErrorCommand { get; }
    public ICommand FilterByCategoryCommand { get; }
    public ICommand FilterBySeverityCommand { get; }
    public ICommand ExportErrorsCommand { get; }

    public ErrorManagementViewModel(
        IErrorReportingService errorReportingService,
        IUserService userService,
        ILogger<ErrorManagementViewModel> logger)
    {
        _errorReportingService = errorReportingService;
        _userService = userService;
        _logger = logger;

        LoadErrorsCommand = new AsyncRelayCommand(LoadErrorsAsync);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        ResolveErrorCommand = new AsyncRelayCommand(ResolveSelectedErrorAsync, () => SelectedError != null && !SelectedError.IsResolved);
        FilterByCategoryCommand = new AsyncRelayCommand<ErrorCategory>(FilterByCategoryAsync);
        FilterBySeverityCommand = new AsyncRelayCommand<ErrorSeverity>(FilterBySeverityAsync);
        ExportErrorsCommand = new AsyncRelayCommand(ExportErrorsAsync);

        // Load initial data
        _ = LoadErrorsAsync();
    }

    private async Task LoadErrorsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading errors...";

            // Load recent errors
            var errors = await _errorReportingService.GetRecentErrorsAsync(100);
            RecentErrors = new ObservableCollection<ErrorReportDto>(errors);

            // Load statistics
            Statistics = await _errorReportingService.GetErrorStatisticsAsync(FilterStartDate);

            StatusMessage = $"Loaded {RecentErrors.Count} errors";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load errors");
            StatusMessage = $"Failed to load errors: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task RefreshAsync()
    {
        await LoadErrorsAsync();
    }

    private async Task ResolveSelectedErrorAsync()
    {
        if (SelectedError == null || SelectedError.IsResolved)
            return;

        try
        {
            var currentUser = _userService.CurrentUser?.Username ?? "Unknown";
            await _errorReportingService.ResolveErrorAsync(SelectedError.Id, currentUser, "Resolved by manager");

            SelectedError.IsResolved = true;
            SelectedError.ResolvedAt = DateTime.UtcNow;
            SelectedError.ResolvedBy = currentUser;

            StatusMessage = $"Error '{SelectedError.Title}' marked as resolved";

            // Refresh statistics
            Statistics = await _errorReportingService.GetErrorStatisticsAsync(FilterStartDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve error: {ErrorId}", SelectedError.Id);
            StatusMessage = $"Failed to resolve error: {ex.Message}";
        }
    }

    private async Task FilterByCategoryAsync(ErrorCategory category)
    {
        try
        {
            IsLoading = true;
            SelectedCategory = category;
            StatusMessage = $"Filtering by {category}...";

            var errors = await _errorReportingService.GetErrorsByCategoryAsync(category, FilterStartDate);
            RecentErrors = new ObservableCollection<ErrorReportDto>(errors);

            StatusMessage = $"Found {RecentErrors.Count} {category} errors";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to filter errors by category: {Category}", category);
            StatusMessage = $"Failed to filter errors: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task FilterBySeverityAsync(ErrorSeverity severity)
    {
        try
        {
            IsLoading = true;
            SelectedSeverity = severity;
            StatusMessage = $"Filtering by {severity} severity...";

            var allErrors = await _errorReportingService.GetRecentErrorsAsync(1000);
            var filteredErrors = allErrors.Where(e => e.Severity == severity);
            RecentErrors = new ObservableCollection<ErrorReportDto>(filteredErrors);

            StatusMessage = $"Found {RecentErrors.Count} {severity} severity errors";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to filter errors by severity: {Severity}", severity);
            StatusMessage = $"Failed to filter errors: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ExportErrorsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Exporting errors...";

            // In a real implementation, this would export to CSV or Excel
            var errors = await _errorReportingService.GetRecentErrorsAsync(1000);
            var exportData = errors.Select(e => new
            {
                e.OccurredAt,
                e.Category,
                e.Severity,
                e.Title,
                e.Message,
                e.TerminalId,
                e.UserContext,
                e.IsResolved,
                e.OccurrenceCount
            }).ToList();

            // For now, just log the export (in real implementation, save to file)
            _logger.LogInformation("Exported {Count} errors to report", exportData.Count);
            StatusMessage = $"Exported {exportData.Count} errors";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export errors");
            StatusMessage = $"Failed to export errors: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedErrorChanged(ErrorReportDto? value)
    {
        ((AsyncRelayCommand)ResolveErrorCommand).NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Gets error summary by category for dashboard display.
    /// </summary>
    public Dictionary<ErrorCategory, int> GetErrorSummaryByCategory()
    {
        return RecentErrors
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.OccurrenceCount));
    }

    /// <summary>
    /// Gets error summary by severity for dashboard display.
    /// </summary>
    public Dictionary<ErrorSeverity, int> GetErrorSummaryBySeverity()
    {
        return RecentErrors
            .GroupBy(e => e.Severity)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.OccurrenceCount));
    }

    /// <summary>
    /// Gets the most frequent errors for dashboard display.
    /// </summary>
    public IEnumerable<ErrorReportDto> GetMostFrequentErrors(int count = 5)
    {
        return RecentErrors
            .OrderByDescending(e => e.OccurrenceCount)
            .Take(count);
    }
}
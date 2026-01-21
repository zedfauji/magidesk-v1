using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Queries.TableSessions;
using Magidesk.Application.Interfaces;
using Magidesk.Application.DTOs;
using Magidesk.Domain.Enumerations;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// ViewModel for real-time session monitoring dashboard with live updates and status indicators.
/// </summary>
public partial class RealTimeSessionMonitoringViewModel : ViewModelBase, IDisposable
{
    private readonly IQueryHandler<GetActiveSessionsQuery, IEnumerable<ActiveSessionDto>> _getActiveSessionsHandler;
    private readonly IQueryHandler<GetSessionAlertsQuery, IEnumerable<SessionAlertDto>> _getSessionAlertsHandler;
    private readonly ILogger<RealTimeSessionMonitoringViewModel> _logger;
    private readonly DispatcherQueue _dispatcherQueue;
    private System.Timers.Timer? _refreshTimer;
    private System.Timers.Timer? _uiUpdateTimer;

    [ObservableProperty]
    private bool _isRealTimeEnabled = true;

    [ObservableProperty]
    private int _refreshIntervalSeconds = 5;

    [ObservableProperty]
    private DateTime _lastRefresh = DateTime.MinValue;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private int _totalActiveSessions;

    [ObservableProperty]
    private int _pausedSessions;

    [ObservableProperty]
    private int _longRunningSessions;

    [ObservableProperty]
    private int _alertCount;

    [ObservableProperty]
    private decimal _totalRevenue;

    [ObservableProperty]
    private TimeSpan _averageSessionDuration;

    [ObservableProperty]
    private SessionMonitoringFilter _currentFilter = SessionMonitoringFilter.All;

    [ObservableProperty]
    private SessionSortOrder _sortOrder = SessionSortOrder.StartTimeDescending;

    public ObservableCollection<ActiveSessionDto> ActiveSessions { get; } = new();
    public ObservableCollection<SessionAlertDto> SessionAlerts { get; } = new();
    public ObservableCollection<ActiveSessionDto> FilteredSessions { get; } = new();

    public ObservableCollection<SessionMonitoringFilter> AvailableFilters { get; } = new()
    {
        SessionMonitoringFilter.All,
        SessionMonitoringFilter.Active,
        SessionMonitoringFilter.Paused,
        SessionMonitoringFilter.LongRunning,
        SessionMonitoringFilter.HighValue
    };

    public ObservableCollection<SessionSortOrder> AvailableSortOrders { get; } = new()
    {
        SessionSortOrder.StartTimeDescending,
        SessionSortOrder.StartTimeAscending,
        SessionSortOrder.DurationDescending,
        SessionSortOrder.DurationAscending,
        SessionSortOrder.ChargeDescending,
        SessionSortOrder.ChargeAscending,
        SessionSortOrder.TableNumber
    };

    public string FilterDisplay => CurrentFilter switch
    {
        SessionMonitoringFilter.All => "All Sessions",
        SessionMonitoringFilter.Active => "Active Only",
        SessionMonitoringFilter.Paused => "Paused Only",
        SessionMonitoringFilter.LongRunning => "Long Running (>3h)",
        SessionMonitoringFilter.HighValue => "High Value (>$50)",
        _ => "Unknown"
    };

    public string SortOrderDisplay => SortOrder switch
    {
        SessionSortOrder.StartTimeDescending => "Newest First",
        SessionSortOrder.StartTimeAscending => "Oldest First",
        SessionSortOrder.DurationDescending => "Longest Duration",
        SessionSortOrder.DurationAscending => "Shortest Duration",
        SessionSortOrder.ChargeDescending => "Highest Charge",
        SessionSortOrder.ChargeAscending => "Lowest Charge",
        SessionSortOrder.TableNumber => "Table Number",
        _ => "Unknown"
    };

    public bool HasActiveSessions => TotalActiveSessions > 0;
    public bool HasAlerts => AlertCount > 0;

    public event EventHandler<SessionSelectedEventArgs>? SessionSelected;

    public RealTimeSessionMonitoringViewModel(
        IQueryHandler<GetActiveSessionsQuery, IEnumerable<ActiveSessionDto>> getActiveSessionsHandler,
        IQueryHandler<GetSessionAlertsQuery, IEnumerable<SessionAlertDto>> getSessionAlertsHandler,
        ILogger<RealTimeSessionMonitoringViewModel> logger)
    {
        _getActiveSessionsHandler = getActiveSessionsHandler ?? throw new ArgumentNullException(nameof(getActiveSessionsHandler));
        _getSessionAlertsHandler = getSessionAlertsHandler ?? throw new ArgumentNullException(nameof(getSessionAlertsHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        RefreshCommand = new AsyncRelayCommand(RefreshDataAsync);
        ToggleRealTimeCommand = new RelayCommand(ToggleRealTime);
        SelectSessionCommand = new RelayCommand<ActiveSessionDto>(SelectSession);
        ClearAlertsCommand = new AsyncRelayCommand(ClearAlertsAsync);
    }

    public AsyncRelayCommand RefreshCommand { get; }
    public RelayCommand ToggleRealTimeCommand { get; }
    public RelayCommand<ActiveSessionDto> SelectSessionCommand { get; }
    public AsyncRelayCommand ClearAlertsCommand { get; }

    /// <summary>
    /// Initializes the monitoring dashboard and starts real-time updates.
    /// </summary>
    public async Task InitializeAsync()
    {
        await RefreshDataAsync();
        StartRealTimeUpdates();
        StartUIUpdates();
    }

    public async Task RefreshDataAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            // Load active sessions
            var sessionsQuery = new GetActiveSessionsQuery();
            var sessions = await _getActiveSessionsHandler.HandleAsync(sessionsQuery);

            // Load session alerts
            var alertsQuery = new GetSessionAlertsQuery();
            var alerts = await _getSessionAlertsHandler.HandleAsync(alertsQuery);

            // Update collections on UI thread
            _dispatcherQueue.TryEnqueue(() =>
            {
                ActiveSessions.Clear();
                foreach (var session in sessions)
                {
                    ActiveSessions.Add(session);
                }

                SessionAlerts.Clear();
                foreach (var alert in alerts)
                {
                    SessionAlerts.Add(alert);
                }

                UpdateStatistics();
                ApplyFilterAndSort();
                LastRefresh = DateTime.UtcNow;
            });

            _logger.LogDebug("Refreshed session monitoring data: {SessionCount} sessions, {AlertCount} alerts",
                sessions.Count(), alerts.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh session monitoring data");
            _dispatcherQueue.TryEnqueue(() =>
            {
                HasError = true;
                ErrorMessage = $"Failed to refresh data: {ex.Message}";
            });
        }
        finally
        {
            _dispatcherQueue.TryEnqueue(() => IsLoading = false);
        }
    }

    private void UpdateStatistics()
    {
        TotalActiveSessions = ActiveSessions.Count;
        PausedSessions = ActiveSessions.Count(s => s.Status == TableSessionStatus.Paused);
        LongRunningSessions = ActiveSessions.Count(s => s.ElapsedTime > TimeSpan.FromHours(3));
        AlertCount = SessionAlerts.Count;
        TotalRevenue = ActiveSessions.Sum(s => s.CurrentCharge);
        
        if (ActiveSessions.Any())
        {
            var totalDuration = ActiveSessions.Aggregate(TimeSpan.Zero, (sum, s) => sum + s.ElapsedTime);
            AverageSessionDuration = TimeSpan.FromTicks(totalDuration.Ticks / ActiveSessions.Count);
        }
        else
        {
            AverageSessionDuration = TimeSpan.Zero;
        }

        OnPropertyChanged(nameof(HasActiveSessions));
        OnPropertyChanged(nameof(HasAlerts));
    }

    private void ApplyFilterAndSort()
    {
        var filtered = ActiveSessions.AsEnumerable();

        // Apply filter
        filtered = CurrentFilter switch
        {
            SessionMonitoringFilter.Active => filtered.Where(s => s.Status == TableSessionStatus.Active),
            SessionMonitoringFilter.Paused => filtered.Where(s => s.Status == TableSessionStatus.Paused),
            SessionMonitoringFilter.LongRunning => filtered.Where(s => s.ElapsedTime > TimeSpan.FromHours(3)),
            SessionMonitoringFilter.HighValue => filtered.Where(s => s.CurrentCharge > 50m),
            _ => filtered
        };

        // Apply sort
        filtered = SortOrder switch
        {
            SessionSortOrder.StartTimeDescending => filtered.OrderByDescending(s => s.StartTime),
            SessionSortOrder.StartTimeAscending => filtered.OrderBy(s => s.StartTime),
            SessionSortOrder.DurationDescending => filtered.OrderByDescending(s => s.ElapsedTime),
            SessionSortOrder.DurationAscending => filtered.OrderBy(s => s.ElapsedTime),
            SessionSortOrder.ChargeDescending => filtered.OrderByDescending(s => s.CurrentCharge),
            SessionSortOrder.ChargeAscending => filtered.OrderBy(s => s.CurrentCharge),
            SessionSortOrder.TableNumber => filtered.OrderBy(s => s.TableNumber),
            _ => filtered.OrderByDescending(s => s.StartTime)
        };

        FilteredSessions.Clear();
        foreach (var session in filtered)
        {
            FilteredSessions.Add(session);
        }
    }

    private void StartRealTimeUpdates()
    {
        if (!IsRealTimeEnabled) return;

        _refreshTimer = new System.Timers.Timer(RefreshIntervalSeconds * 1000);
        _refreshTimer.Elapsed += async (s, e) =>
        {
            if (!IsLoading)
            {
                await RefreshDataAsync();
            }
        };
        _refreshTimer.Start();
    }

    private void StartUIUpdates()
    {
        // Update UI every second for elapsed time calculations
        _uiUpdateTimer = new System.Timers.Timer(1000);
        _uiUpdateTimer.Elapsed += (s, e) =>
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                // Force property change notifications for calculated properties
                foreach (var session in ActiveSessions.Where(s => s.Status == TableSessionStatus.Active))
                {
                    // This will trigger UI updates for elapsed time and current charge
                    session.NotifyElapsedTimeChanged();
                }
                
                UpdateStatistics();
            });
        };
        _uiUpdateTimer.Start();
    }

    private void StopRealTimeUpdates()
    {
        _refreshTimer?.Stop();
        _refreshTimer?.Dispose();
        _refreshTimer = null;
    }

    private void StopUIUpdates()
    {
        _uiUpdateTimer?.Stop();
        _uiUpdateTimer?.Dispose();
        _uiUpdateTimer = null;
    }

    private void ToggleRealTime()
    {
        IsRealTimeEnabled = !IsRealTimeEnabled;
        
        if (IsRealTimeEnabled)
        {
            StartRealTimeUpdates();
        }
        else
        {
            StopRealTimeUpdates();
        }
    }

    private void SelectSession(ActiveSessionDto? session)
    {
        if (session != null)
        {
            SessionSelected?.Invoke(this, new SessionSelectedEventArgs(session));
        }
    }

    private async Task ClearAlertsAsync()
    {
        try
        {
            // TODO: Implement clear alerts command
            SessionAlerts.Clear();
            AlertCount = 0;
            OnPropertyChanged(nameof(HasAlerts));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear session alerts");
            HasError = true;
            ErrorMessage = $"Failed to clear alerts: {ex.Message}";
        }
    }

    partial void OnCurrentFilterChanged(SessionMonitoringFilter value)
    {
        ApplyFilterAndSort();
        OnPropertyChanged(nameof(FilterDisplay));
    }

    partial void OnSortOrderChanged(SessionSortOrder value)
    {
        ApplyFilterAndSort();
        OnPropertyChanged(nameof(SortOrderDisplay));
    }

    partial void OnRefreshIntervalSecondsChanged(int value)
    {
        if (IsRealTimeEnabled && _refreshTimer != null)
        {
            _refreshTimer.Interval = value * 1000;
        }
    }

    public void Dispose()
    {
        StopRealTimeUpdates();
        StopUIUpdates();
    }
}

/// <summary>
/// Session monitoring filter options.
/// </summary>
public enum SessionMonitoringFilter
{
    All,
    Active,
    Paused,
    LongRunning,
    HighValue
}

/// <summary>
/// Session sort order options.
/// </summary>
public enum SessionSortOrder
{
    StartTimeDescending,
    StartTimeAscending,
    DurationDescending,
    DurationAscending,
    ChargeDescending,
    ChargeAscending,
    TableNumber
}

/// <summary>
/// Event arguments for session selection.
/// </summary>
public class SessionSelectedEventArgs : EventArgs
{
    public ActiveSessionDto Session { get; }

    public SessionSelectedEventArgs(ActiveSessionDto session)
    {
        Session = session;
    }
}
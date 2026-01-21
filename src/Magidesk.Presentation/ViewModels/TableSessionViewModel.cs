using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.Commands.ManagerOverrides;
using Magidesk.Application.Queries.TableSessions;
using Magidesk.Application.Interfaces;
using Magidesk.Application.DTOs;
using Magidesk.Domain.Enumerations;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// ViewModel for managing active table sessions with control operations.
/// Provides session control (pause, resume, end), transfer, and guest count updates.
/// </summary>
public partial class TableSessionViewModel : ViewModelBase, IDisposable
{
    private readonly IQueryHandler<GetActiveSessionsQuery, IEnumerable<ActiveSessionDto>> _getActiveSessionsHandler;
    private readonly ICommandHandler<EnhancedPauseSessionCommand, EnhancedPauseSessionResult> _pauseSessionHandler;
    private readonly ICommandHandler<EnhancedResumeSessionCommand, EnhancedResumeSessionResult> _resumeSessionHandler;
    private readonly ICommandHandler<ForceEndSessionCommand, ForceEndSessionResult> _forceEndSessionHandler;
    private readonly ICommandHandler<TransferSessionCommand, TransferSessionResult> _transferSessionHandler;
    private readonly ICommandHandler<UpdateGuestCountCommand, UpdateGuestCountResult> _updateGuestCountHandler;
    private readonly ILogger<TableSessionViewModel> _logger;
    private readonly DispatcherQueue _dispatcherQueue;
    private System.Timers.Timer? _refreshTimer;
    private System.Timers.Timer? _uiUpdateTimer;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private ActiveSessionDto? _selectedSession;

    [ObservableProperty]
    private bool _isRealTimeEnabled = true;

    [ObservableProperty]
    private int _refreshIntervalSeconds = 5;

    [ObservableProperty]
    private DateTime _lastRefresh = DateTime.MinValue;

    [ObservableProperty]
    private int _totalActiveSessions;

    [ObservableProperty]
    private int _pausedSessions;

    [ObservableProperty]
    private decimal _totalRevenue;

    public ObservableCollection<ActiveSessionDto> ActiveSessions { get; } = new();

    public bool HasSelectedSession => SelectedSession != null;
    public bool CanPauseSession => SelectedSession != null && SelectedSession.Status == TableSessionStatus.Active;
    public bool CanResumeSession => SelectedSession != null && SelectedSession.Status == TableSessionStatus.Paused;
    public bool CanEndSession => SelectedSession != null && (SelectedSession.Status == TableSessionStatus.Active || SelectedSession.Status == TableSessionStatus.Paused);
    public bool CanTransferSession => SelectedSession != null && (SelectedSession.Status == TableSessionStatus.Active || SelectedSession.Status == TableSessionStatus.Paused);
    public bool CanUpdateGuestCount => SelectedSession != null && (SelectedSession.Status == TableSessionStatus.Active || SelectedSession.Status == TableSessionStatus.Paused);

    public TableSessionViewModel(
        IQueryHandler<GetActiveSessionsQuery, IEnumerable<ActiveSessionDto>> getActiveSessionsHandler,
        ICommandHandler<EnhancedPauseSessionCommand, EnhancedPauseSessionResult> pauseSessionHandler,
        ICommandHandler<EnhancedResumeSessionCommand, EnhancedResumeSessionResult> resumeSessionHandler,
        ICommandHandler<ForceEndSessionCommand, ForceEndSessionResult> forceEndSessionHandler,
        ICommandHandler<TransferSessionCommand, TransferSessionResult> transferSessionHandler,
        ICommandHandler<UpdateGuestCountCommand, UpdateGuestCountResult> updateGuestCountHandler,
        ILogger<TableSessionViewModel> logger)
    {
        _getActiveSessionsHandler = getActiveSessionsHandler ?? throw new ArgumentNullException(nameof(getActiveSessionsHandler));
        _pauseSessionHandler = pauseSessionHandler ?? throw new ArgumentNullException(nameof(pauseSessionHandler));
        _resumeSessionHandler = resumeSessionHandler ?? throw new ArgumentNullException(nameof(resumeSessionHandler));
        _forceEndSessionHandler = forceEndSessionHandler ?? throw new ArgumentNullException(nameof(forceEndSessionHandler));
        _transferSessionHandler = transferSessionHandler ?? throw new ArgumentNullException(nameof(transferSessionHandler));
        _updateGuestCountHandler = updateGuestCountHandler ?? throw new ArgumentNullException(nameof(updateGuestCountHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        LoadSessionsCommand = new AsyncRelayCommand(LoadActiveSessionsAsync);
        PauseSessionCommand = new AsyncRelayCommand(PauseSessionAsync, () => CanPauseSession);
        ResumeSessionCommand = new AsyncRelayCommand(ResumeSessionAsync, () => CanResumeSession);
        EndSessionCommand = new AsyncRelayCommand(EndSessionAsync, () => CanEndSession);
        TransferSessionCommand = new AsyncRelayCommand<Guid>(TransferSessionAsync, (targetTableId) => CanTransferSession);
        UpdateGuestCountCommand = new AsyncRelayCommand<int>(UpdateGuestCountAsync, (newCount) => CanUpdateGuestCount);
        SelectSessionCommand = new RelayCommand<ActiveSessionDto>(SelectSession);
        ToggleRealTimeCommand = new RelayCommand(ToggleRealTime);
    }

    public AsyncRelayCommand LoadSessionsCommand { get; }
    public AsyncRelayCommand PauseSessionCommand { get; }
    public AsyncRelayCommand ResumeSessionCommand { get; }
    public AsyncRelayCommand EndSessionCommand { get; }
    public AsyncRelayCommand<Guid> TransferSessionCommand { get; }
    public AsyncRelayCommand<int> UpdateGuestCountCommand { get; }
    public RelayCommand<ActiveSessionDto> SelectSessionCommand { get; }
    public RelayCommand ToggleRealTimeCommand { get; }

    /// <summary>
    /// Initializes the view model and starts real-time monitoring.
    /// </summary>
    public async Task InitializeAsync()
    {
        await LoadActiveSessionsAsync();
        StartRealTimeUpdates();
        StartUIUpdates();
    }

    /// <summary>
    /// Loads all active table sessions.
    /// </summary>
    public async Task LoadActiveSessionsAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            var query = new GetActiveSessionsQuery();
            var sessions = await _getActiveSessionsHandler.HandleAsync(query);

            _dispatcherQueue.TryEnqueue(() =>
            {
                ActiveSessions.Clear();
                foreach (var session in sessions)
                {
                    ActiveSessions.Add(session);
                }

                UpdateStatistics();
                LastRefresh = DateTime.UtcNow;
            });

            _logger.LogInformation("Loaded {Count} active sessions", sessions.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load active sessions");
            _dispatcherQueue.TryEnqueue(() =>
            {
                HasError = true;
                ErrorMessage = $"Failed to load sessions: {ex.Message}";
            });
        }
        finally
        {
            _dispatcherQueue.TryEnqueue(() => IsLoading = false);
        }
    }

    /// <summary>
    /// Pauses the selected session.
    /// </summary>
    public async Task PauseSessionAsync()
    {
        if (SelectedSession == null) return;

        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            var command = new EnhancedPauseSessionCommand(
                SelectedSession.SessionId,
                "Paused by operator",
                null // TODO: Get current staff ID from user context
            );

            var result = await _pauseSessionHandler.HandleAsync(command);

            _logger.LogInformation("Session {SessionId} paused successfully", result.SessionId);

            // Refresh sessions to get updated state
            await LoadActiveSessionsAsync();

            _dispatcherQueue.TryEnqueue(() =>
            {
                StatusMessage = $"Session paused. Current charge: {result.CurrentCharge:C}";
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to pause session {SessionId}", SelectedSession.SessionId);
            _dispatcherQueue.TryEnqueue(() =>
            {
                HasError = true;
                ErrorMessage = $"Failed to pause session: {ex.Message}";
            });
        }
        finally
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                IsLoading = false;
                UpdateCommandStates();
            });
        }
    }

    /// <summary>
    /// Resumes the selected paused session.
    /// </summary>
    public async Task ResumeSessionAsync()
    {
        if (SelectedSession == null) return;

        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            var command = new EnhancedResumeSessionCommand(
                SelectedSession.SessionId,
                null // TODO: Get current staff ID from user context
            );

            var result = await _resumeSessionHandler.HandleAsync(command);

            _logger.LogInformation("Session {SessionId} resumed successfully", result.SessionId);

            // Refresh sessions to get updated state
            await LoadActiveSessionsAsync();

            _dispatcherQueue.TryEnqueue(() =>
            {
                StatusMessage = $"Session resumed. Current charge: {result.CurrentCharge:C}";
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resume session {SessionId}", SelectedSession.SessionId);
            _dispatcherQueue.TryEnqueue(() =>
            {
                HasError = true;
                ErrorMessage = $"Failed to resume session: {ex.Message}";
            });
        }
        finally
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                IsLoading = false;
                UpdateCommandStates();
            });
        }
    }

    /// <summary>
    /// Ends the selected session (requires manager authorization).
    /// </summary>
    public async Task EndSessionAsync()
    {
        if (SelectedSession == null) return;

        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            // TODO: Show manager PIN dialog to get authorization
            var managerPin = ""; // Placeholder - should come from dialog
            var managerId = Guid.Empty; // Placeholder - should come from dialog

            var command = new ForceEndSessionCommand(
                SelectedSession.SessionId,
                "Session ended by operator",
                managerPin,
                managerId
            );

            var result = await _forceEndSessionHandler.HandleAsync(command);

            _logger.LogInformation("Session {SessionId} ended successfully. Final charge: {FinalCharge:C}",
                result.SessionId, result.FinalCharge);

            // Refresh sessions to remove ended session
            await LoadActiveSessionsAsync();

            _dispatcherQueue.TryEnqueue(() =>
            {
                StatusMessage = $"Session ended. Final charge: {result.FinalCharge:C}, Duration: {result.TotalDuration:hh\\:mm\\:ss}";
                SelectedSession = null;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to end session {SessionId}", SelectedSession.SessionId);
            _dispatcherQueue.TryEnqueue(() =>
            {
                HasError = true;
                ErrorMessage = $"Failed to end session: {ex.Message}";
            });
        }
        finally
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                IsLoading = false;
                UpdateCommandStates();
            });
        }
    }

    /// <summary>
    /// Transfers the selected session to a different table.
    /// </summary>
    public async Task TransferSessionAsync(Guid targetTableId)
    {
        if (SelectedSession == null) return;

        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            var command = new TransferSessionCommand(
                SelectedSession.SessionId,
                targetTableId,
                "Session transferred by operator",
                Guid.Empty // TODO: Get current staff ID from user context
            );

            var result = await _transferSessionHandler.HandleAsync(command);

            _logger.LogInformation("Session {OriginalSessionId} transferred to table {NewTableId}. New session: {NewSessionId}",
                result.OriginalSessionId, result.NewTableId, result.NewSessionId);

            // Refresh sessions to get updated state
            await LoadActiveSessionsAsync();

            _dispatcherQueue.TryEnqueue(() =>
            {
                StatusMessage = $"Session transferred successfully. Preserved charge: {result.PreservedCharge:C}";
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to transfer session {SessionId}", SelectedSession.SessionId);
            _dispatcherQueue.TryEnqueue(() =>
            {
                HasError = true;
                ErrorMessage = $"Failed to transfer session: {ex.Message}";
            });
        }
        finally
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                IsLoading = false;
                UpdateCommandStates();
            });
        }
    }

    /// <summary>
    /// Updates the guest count for the selected session.
    /// </summary>
    public async Task UpdateGuestCountAsync(int newGuestCount)
    {
        if (SelectedSession == null) return;

        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            var command = new UpdateGuestCountCommand(
                SelectedSession.SessionId,
                newGuestCount,
                null // TODO: Get current staff ID from user context
            );

            var result = await _updateGuestCountHandler.HandleAsync(command);

            _logger.LogInformation("Guest count updated for session {SessionId}: {PreviousCount} -> {NewCount}",
                result.SessionId, result.PreviousGuestCount, result.NewGuestCount);

            // Refresh sessions to get updated state
            await LoadActiveSessionsAsync();

            _dispatcherQueue.TryEnqueue(() =>
            {
                StatusMessage = $"Guest count updated from {result.PreviousGuestCount} to {result.NewGuestCount}";
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update guest count for session {SessionId}", SelectedSession.SessionId);
            _dispatcherQueue.TryEnqueue(() =>
            {
                HasError = true;
                ErrorMessage = $"Failed to update guest count: {ex.Message}";
            });
        }
        finally
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                IsLoading = false;
                UpdateCommandStates();
            });
        }
    }

    /// <summary>
    /// Selects a session for operations.
    /// </summary>
    private void SelectSession(ActiveSessionDto? session)
    {
        SelectedSession = session;
        UpdateCommandStates();
    }

    /// <summary>
    /// Updates statistics based on current sessions.
    /// </summary>
    private void UpdateStatistics()
    {
        TotalActiveSessions = ActiveSessions.Count;
        PausedSessions = ActiveSessions.Count(s => s.Status == TableSessionStatus.Paused);
        TotalRevenue = ActiveSessions.Sum(s => s.CurrentCharge);
    }

    /// <summary>
    /// Updates command can-execute states.
    /// </summary>
    private void UpdateCommandStates()
    {
        OnPropertyChanged(nameof(HasSelectedSession));
        OnPropertyChanged(nameof(CanPauseSession));
        OnPropertyChanged(nameof(CanResumeSession));
        OnPropertyChanged(nameof(CanEndSession));
        OnPropertyChanged(nameof(CanTransferSession));
        OnPropertyChanged(nameof(CanUpdateGuestCount));

        PauseSessionCommand.NotifyCanExecuteChanged();
        ResumeSessionCommand.NotifyCanExecuteChanged();
        EndSessionCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Starts real-time session updates.
    /// </summary>
    private void StartRealTimeUpdates()
    {
        if (!IsRealTimeEnabled) return;

        _refreshTimer = new System.Timers.Timer(RefreshIntervalSeconds * 1000);
        _refreshTimer.Elapsed += async (s, e) =>
        {
            if (!IsLoading)
            {
                await LoadActiveSessionsAsync();
            }
        };
        _refreshTimer.Start();
    }

    /// <summary>
    /// Starts UI updates for elapsed time calculations.
    /// </summary>
    private void StartUIUpdates()
    {
        _uiUpdateTimer = new System.Timers.Timer(1000);
        _uiUpdateTimer.Elapsed += (s, e) =>
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                // Update elapsed time for active sessions
                foreach (var session in ActiveSessions.Where(s => s.Status == TableSessionStatus.Active))
                {
                    session.NotifyElapsedTimeChanged();
                }

                UpdateStatistics();
            });
        };
        _uiUpdateTimer.Start();
    }

    /// <summary>
    /// Stops real-time session updates.
    /// </summary>
    private void StopRealTimeUpdates()
    {
        _refreshTimer?.Stop();
        _refreshTimer?.Dispose();
        _refreshTimer = null;
    }

    /// <summary>
    /// Stops UI updates.
    /// </summary>
    private void StopUIUpdates()
    {
        _uiUpdateTimer?.Stop();
        _uiUpdateTimer?.Dispose();
        _uiUpdateTimer = null;
    }

    /// <summary>
    /// Toggles real-time monitoring on/off.
    /// </summary>
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

    partial void OnSelectedSessionChanged(ActiveSessionDto? value)
    {
        UpdateCommandStates();
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

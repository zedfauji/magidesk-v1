using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.Interfaces;
using Magidesk.Application.DTOs;
using Magidesk.Domain.Enumerations;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// ViewModel for session control operations including pause/resume and guest count updates.
/// </summary>
public partial class SessionControlDialogViewModel : ViewModelBase
{
    private readonly ICommandHandler<PauseTableSessionCommand, PauseTableSessionResult> _pauseHandler;
    private readonly ICommandHandler<ResumeTableSessionCommand, ResumeTableSessionResult> _resumeHandler;
    private readonly ICommandHandler<UpdateGuestCountCommand, UpdateGuestCountResult> _updateGuestCountHandler;
    private readonly ILogger<SessionControlDialogViewModel> _logger;

    [ObservableProperty]
    private Guid _sessionId;

    [ObservableProperty]
    private string _tableName = string.Empty;

    [ObservableProperty]
    private TableSessionStatus _sessionStatus;

    [ObservableProperty]
    private int _currentGuestCount;

    [ObservableProperty]
    private int _newGuestCount;

    [ObservableProperty]
    private string _pauseReason = string.Empty;

    [ObservableProperty]
    private TimeSpan _elapsedTime;

    [ObservableProperty]
    private TimeSpan _pausedDuration;

    [ObservableProperty]
    private decimal _currentCharge;

    [ObservableProperty]
    private bool _isPaused;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isLoading;

    public ObservableCollection<string> PauseReasons { get; } = new()
    {
        "Customer break",
        "Equipment issue",
        "Staff assistance needed",
        "Customer request",
        "Technical problem",
        "Other"
    };

    public event EventHandler? RequestClose;
    public event EventHandler<SessionControlEventArgs>? SessionControlCompleted;

    public SessionControlDialogViewModel(
        ICommandHandler<PauseTableSessionCommand, PauseTableSessionResult> pauseHandler,
        ICommandHandler<ResumeTableSessionCommand, ResumeTableSessionResult> resumeHandler,
        ICommandHandler<UpdateGuestCountCommand, UpdateGuestCountResult> updateGuestCountHandler,
        ILogger<SessionControlDialogViewModel> logger)
    {
        _pauseHandler = pauseHandler ?? throw new ArgumentNullException(nameof(pauseHandler));
        _resumeHandler = resumeHandler ?? throw new ArgumentNullException(nameof(resumeHandler));
        _updateGuestCountHandler = updateGuestCountHandler ?? throw new ArgumentNullException(nameof(updateGuestCountHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        PauseSessionCommand = new AsyncRelayCommand(PauseSessionAsync, () => !IsPaused && !IsLoading);
        ResumeSessionCommand = new AsyncRelayCommand(ResumeSessionAsync, () => IsPaused && !IsLoading);
        UpdateGuestCountCommand = new AsyncRelayCommand(UpdateGuestCountAsync, () => NewGuestCount != CurrentGuestCount && NewGuestCount > 0 && NewGuestCount <= 20 && !IsLoading);
        CancelCommand = new RelayCommand(Cancel);
    }

    public AsyncRelayCommand PauseSessionCommand { get; }
    public AsyncRelayCommand ResumeSessionCommand { get; }
    public AsyncRelayCommand UpdateGuestCountCommand { get; }
    public RelayCommand CancelCommand { get; }

    /// <summary>
    /// Initializes the dialog with session information.
    /// </summary>
    public void Initialize(
        Guid sessionId,
        string tableName,
        TableSessionStatus sessionStatus,
        int currentGuestCount,
        TimeSpan elapsedTime,
        TimeSpan pausedDuration,
        decimal currentCharge)
    {
        SessionId = sessionId;
        TableName = tableName;
        SessionStatus = sessionStatus;
        CurrentGuestCount = currentGuestCount;
        NewGuestCount = currentGuestCount;
        ElapsedTime = elapsedTime;
        PausedDuration = pausedDuration;
        CurrentCharge = currentCharge;
        IsPaused = sessionStatus == TableSessionStatus.Paused;
        
        // Reset state
        PauseReason = string.Empty;
        HasError = false;
        ErrorMessage = null;
        
        // Update command states
        PauseSessionCommand.NotifyCanExecuteChanged();
        ResumeSessionCommand.NotifyCanExecuteChanged();
        UpdateGuestCountCommand.NotifyCanExecuteChanged();
    }

    private async Task PauseSessionAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            if (string.IsNullOrWhiteSpace(PauseReason))
            {
                ErrorMessage = "Please select or enter a reason for pausing the session.";
                HasError = true;
                return;
            }

            var command = new PauseTableSessionCommand(SessionId);
            // Reason currently ignored by command
            // var command = new PauseTableSessionCommand(SessionId, PauseReason);
            var result = await _pauseHandler.HandleAsync(command);

            _logger.LogInformation("Session {SessionId} paused at {PausedAt}", result.SessionId, result.PausedAt);

            // Update UI state
            IsPaused = true;
            SessionStatus = TableSessionStatus.Paused;

            // Notify completion
            SessionControlCompleted?.Invoke(this, new SessionControlEventArgs(
                SessionId, SessionControlOperation.Pause, true, $"Session paused at {result.PausedAt:HH:mm:ss}"));

            RequestClose?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to pause session {SessionId}", SessionId);
            HasError = true;
            ErrorMessage = $"Failed to pause session: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            PauseSessionCommand.NotifyCanExecuteChanged();
            ResumeSessionCommand.NotifyCanExecuteChanged();
        }
    }

    private async Task ResumeSessionAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            var command = new ResumeTableSessionCommand(SessionId);
            var result = await _resumeHandler.HandleAsync(command);

            _logger.LogInformation("Session {SessionId} resumed at {ResumedAt}. Total paused duration: {TotalPausedDuration}",
                result.SessionId, result.ResumedAt, result.TotalPausedDuration);

            // Update UI state
            IsPaused = false;
            SessionStatus = TableSessionStatus.Active;
            PausedDuration = result.TotalPausedDuration;

            // Notify completion
            SessionControlCompleted?.Invoke(this, new SessionControlEventArgs(
                SessionId, SessionControlOperation.Resume, true, $"Session resumed at {result.ResumedAt:HH:mm:ss}"));

            RequestClose?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resume session {SessionId}", SessionId);
            HasError = true;
            ErrorMessage = $"Failed to resume session: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            PauseSessionCommand.NotifyCanExecuteChanged();
            ResumeSessionCommand.NotifyCanExecuteChanged();
        }
    }

    private async Task UpdateGuestCountAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            if (NewGuestCount <= 0 || NewGuestCount > 20)
            {
                ErrorMessage = "Guest count must be between 1 and 20.";
                HasError = true;
                return;
            }

            var command = new UpdateGuestCountCommand(SessionId, NewGuestCount);
            var result = await _updateGuestCountHandler.HandleAsync(command);

            _logger.LogInformation("Guest count updated for session {SessionId}: {OldCount} -> {NewCount}",
                SessionId, CurrentGuestCount, NewGuestCount);

            // Update UI state
            CurrentGuestCount = NewGuestCount;

            // Notify completion
            SessionControlCompleted?.Invoke(this, new SessionControlEventArgs(
                SessionId, SessionControlOperation.UpdateGuestCount, true, $"Guest count updated to {NewGuestCount}"));

            RequestClose?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update guest count for session {SessionId}", SessionId);
            HasError = true;
            ErrorMessage = $"Failed to update guest count: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            UpdateGuestCountCommand.NotifyCanExecuteChanged();
        }
    }

    private void Cancel()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    partial void OnNewGuestCountChanged(int value)
    {
        UpdateGuestCountCommand.NotifyCanExecuteChanged();
    }

    partial void OnPauseReasonChanged(string value)
    {
        if (HasError && !string.IsNullOrWhiteSpace(value))
        {
            HasError = false;
            ErrorMessage = null;
        }
    }
}

/// <summary>
/// Event arguments for session control operations.
/// </summary>
public class SessionControlEventArgs : EventArgs
{
    public Guid SessionId { get; }
    public SessionControlOperation Operation { get; }
    public bool Success { get; }
    public string Message { get; }

    public SessionControlEventArgs(Guid sessionId, SessionControlOperation operation, bool success, string message)
    {
        SessionId = sessionId;
        Operation = operation;
        Success = success;
        Message = message;
    }
}

/// <summary>
/// Types of session control operations.
/// </summary>
public enum SessionControlOperation
{
    Pause,
    Resume,
    UpdateGuestCount
}
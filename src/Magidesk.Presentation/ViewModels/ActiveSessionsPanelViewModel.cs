using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.TableSessions;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// ViewModel for the Active Sessions Panel.
/// </summary>
public partial class ActiveSessionsPanelViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetActiveSessionsQuery, IEnumerable<ActiveSessionDto>> _getActiveSessionsHandler;
    private readonly ICommandHandler<EndTableSessionCommand, EndTableSessionResult> _endSessionHandler;
    private readonly ILogger<ActiveSessionsPanelViewModel> _logger;
    private DispatcherTimer? _refreshTimer;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _hasError;

    public ObservableCollection<ActiveSessionDto> ActiveSessions { get; } = new();

    public ActiveSessionsPanelViewModel(
        IQueryHandler<GetActiveSessionsQuery, IEnumerable<ActiveSessionDto>> getActiveSessionsHandler,
        ICommandHandler<EndTableSessionCommand, EndTableSessionResult> endSessionHandler,
        ILogger<ActiveSessionsPanelViewModel> logger)
    {
        _getActiveSessionsHandler = getActiveSessionsHandler ?? throw new ArgumentNullException(nameof(getActiveSessionsHandler));
        _endSessionHandler = endSessionHandler ?? throw new ArgumentNullException(nameof(endSessionHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        RefreshCommand = new AsyncRelayCommand(LoadActiveSessionsAsync);
        EndSessionCommand = new AsyncRelayCommand<Guid>(EndSessionAsync);
    }

    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand<Guid> EndSessionCommand { get; }

    /// <summary>
    /// Initializes the panel and starts auto-refresh.
    /// </summary>
    public async Task InitializeAsync()
    {
        await LoadActiveSessionsAsync();
        StartAutoRefresh();
    }

    private async Task LoadActiveSessionsAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            var query = new GetActiveSessionsQuery();
            var sessions = await _getActiveSessionsHandler.HandleAsync(query);

            // Update collection
            ActiveSessions.Clear();
            foreach (var session in sessions)
            {
                ActiveSessions.Add(session);
            }

            _logger.LogInformation("Loaded {Count} active sessions", ActiveSessions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load active sessions");
            HasError = true;
            ErrorMessage = $"Failed to load active sessions: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task EndSessionAsync(Guid sessionId)
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            var command = new EndTableSessionCommand(sessionId, CreateTicket: true);
            var result = await _endSessionHandler.HandleAsync(command);

            _logger.LogInformation("Session {SessionId} ended successfully", sessionId);

            // Refresh the list
            await LoadActiveSessionsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to end session {SessionId}", sessionId);
            HasError = true;
            ErrorMessage = $"Failed to end session: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void StartAutoRefresh()
    {
        _refreshTimer = new DispatcherTimer();
        _refreshTimer.Interval = TimeSpan.FromSeconds(5); // Refresh every 5 seconds
        _refreshTimer.Tick += async (s, e) =>
        {
            if (!IsLoading)
            {
                await LoadActiveSessionsAsync();
            }
        };
        _refreshTimer.Start();
    }

    private void StopAutoRefresh()
    {
        _refreshTimer?.Stop();
        _refreshTimer = null;
    }

    public void Dispose()
    {
        StopAutoRefresh();
    }
}

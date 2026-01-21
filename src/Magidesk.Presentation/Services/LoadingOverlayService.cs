using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.Services;

/// <summary>
/// Service for managing loading overlay state with timeout protection.
/// </summary>
public class LoadingOverlayService : ILoadingOverlayService
{
    private readonly ILogger<LoadingOverlayService>? _logger;
    private readonly TimeSpan _maxLoadingDuration = TimeSpan.FromSeconds(30);
    private bool _isLoading;
    private string _loadingMessage = string.Empty;
    private bool _isCancellable;

    public LoadingOverlayService(ILogger<LoadingOverlayService>? logger = null)
    {
        _logger = logger;
    }

    public bool IsLoading => _isLoading;
    public string LoadingMessage => _loadingMessage;
    public bool IsCancellable => _isCancellable;

    public event EventHandler<LoadingStateChangedEventArgs>? LoadingStateChanged;
    public event EventHandler? CancelRequested;

    public void Show(string message, bool isCancellable = false)
    {
        _isLoading = true;
        _loadingMessage = message;
        _isCancellable = isCancellable;

        _logger?.LogDebug("Loading overlay shown: {Message}", message);

        LoadingStateChanged?.Invoke(this, new LoadingStateChangedEventArgs
        {
            IsLoading = true,
            Message = message,
            IsCancellable = isCancellable
        });
    }

    public void Hide()
    {
        _isLoading = false;
        _loadingMessage = string.Empty;
        _isCancellable = false;

        _logger?.LogDebug("Loading overlay hidden");

        LoadingStateChanged?.Invoke(this, new LoadingStateChangedEventArgs
        {
            IsLoading = false,
            Message = string.Empty,
            IsCancellable = false
        });
    }

    public async Task ShowDuringOperationAsync(Func<Task> operation, string message, bool isCancellable = false)
    {
        var cts = new CancellationTokenSource(_maxLoadingDuration);

        try
        {
            Show(message, isCancellable);

            if (isCancellable)
            {
                EventHandler? cancelHandler = null;
                cancelHandler = (s, e) =>
                {
                    cts.Cancel();
                    CancelRequested -= cancelHandler;
                };
                CancelRequested += cancelHandler;
            }

            await operation();
        }
        catch (OperationCanceledException)
        {
            _logger?.LogWarning("Loading operation timed out or was cancelled after {Duration}", _maxLoadingDuration);
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during loading operation: {Message}", message);
            throw;
        }
        finally
        {
            Hide();
        }
    }

    public void RequestCancel()
    {
        if (_isCancellable)
        {
            _logger?.LogInformation("Cancel requested for loading operation");
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}

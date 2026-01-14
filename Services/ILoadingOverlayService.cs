namespace Magidesk.Presentation.Services;

/// <summary>
/// Service for managing loading overlay state during asynchronous operations.
/// </summary>
public interface ILoadingOverlayService
{
    /// <summary>
    /// Gets whether the loading overlay is currently displayed.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Gets the current loading message.
    /// </summary>
    string LoadingMessage { get; }

    /// <summary>
    /// Gets whether the current operation is cancellable.
    /// </summary>
    bool IsCancellable { get; }

    /// <summary>
    /// Shows the loading overlay with a message.
    /// </summary>
    void Show(string message, bool isCancellable = false);

    /// <summary>
    /// Hides the loading overlay.
    /// </summary>
    void Hide();

    /// <summary>
    /// Executes an async operation with loading overlay displayed.
    /// </summary>
    Task ShowDuringOperationAsync(Func<Task> operation, string message, bool isCancellable = false);

    /// <summary>
    /// Event raised when the loading state changes.
    /// </summary>
    event EventHandler<LoadingStateChangedEventArgs>? LoadingStateChanged;

    /// <summary>
    /// Event raised when the user requests to cancel the operation.
    /// </summary>
    event EventHandler? CancelRequested;
}

/// <summary>
/// Event args for loading state changes.
/// </summary>
public class LoadingStateChangedEventArgs : EventArgs
{
    public bool IsLoading { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsCancellable { get; set; }
}

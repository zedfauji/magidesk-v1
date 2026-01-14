namespace Magidesk.Presentation.Services;

/// <summary>
/// Service for displaying toast notifications to provide user feedback.
/// </summary>
public interface IToastNotificationService
{
    /// <summary>
    /// Displays a success toast notification.
    /// </summary>
    void ShowSuccess(string message, string title = "Success");

    /// <summary>
    /// Displays an error toast notification.
    /// </summary>
    void ShowError(string message, string title = "Error", string? details = null);

    /// <summary>
    /// Displays an informational toast notification.
    /// </summary>
    void ShowInfo(string message, string title = "Information");

    /// <summary>
    /// Displays a warning toast notification.
    /// </summary>
    void ShowWarning(string message, string title = "Warning");

    /// <summary>
    /// Gets the collection of active toast notifications.
    /// </summary>
    System.Collections.ObjectModel.ObservableCollection<ToastNotification> ActiveToasts { get; }
}

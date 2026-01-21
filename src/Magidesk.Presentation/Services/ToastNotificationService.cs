using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;

namespace Magidesk.Presentation.Services;

/// <summary>
/// Service for displaying toast notifications with auto-dismissal.
/// </summary>
public class ToastNotificationService : IToastNotificationService
{
    private readonly ObservableCollection<ToastNotification> _activeToasts = new();
    private readonly ILogger<ToastNotificationService>? _logger;
    private readonly DispatcherQueue? _dispatcherQueue;
    private const int MaxVisibleToasts = 3;

    public ToastNotificationService(ILogger<ToastNotificationService>? logger = null, DispatcherQueue? dispatcherQueue = null)
    {
        _logger = logger;
        _dispatcherQueue = dispatcherQueue;
    }

    public ObservableCollection<ToastNotification> ActiveToasts => _activeToasts;

    public void ShowSuccess(string message, string title = "Success")
    {
        try
        {
            var toast = new ToastNotification
            {
                Type = ToastType.Success,
                Title = title,
                Message = message,
                Icon = "\uE73E", // Checkmark icon
                Duration = TimeSpan.FromSeconds(4)
            };

            AddToast(toast);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to display success toast notification");
        }
    }

    public void ShowError(string message, string title = "Error", string? details = null)
    {
        try
        {
            var fullMessage = details != null ? $"{message}\n{details}" : message;
            var toast = new ToastNotification
            {
                Type = ToastType.Error,
                Title = title,
                Message = fullMessage,
                Icon = "\uE783", // Error icon
                Duration = TimeSpan.FromSeconds(8)
            };

            AddToast(toast);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to display error toast notification");
        }
    }

    public void ShowInfo(string message, string title = "Information")
    {
        try
        {
            var toast = new ToastNotification
            {
                Type = ToastType.Info,
                Title = title,
                Message = message,
                Icon = "\uE946", // Info icon
                Duration = TimeSpan.FromSeconds(4)
            };

            AddToast(toast);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to display info toast notification");
        }
    }

    public void ShowWarning(string message, string title = "Warning")
    {
        try
        {
            var toast = new ToastNotification
            {
                Type = ToastType.Warning,
                Title = title,
                Message = message,
                Icon = "\uE7BA", // Warning icon
                Duration = TimeSpan.FromSeconds(6)
            };

            AddToast(toast);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to display warning toast notification");
        }
    }

    private void AddToast(ToastNotification toast)
    {
        void AddToastOnUIThread()
        {
            // Enforce maximum visible toasts limit
            if (_activeToasts.Count >= MaxVisibleToasts)
            {
                _activeToasts.RemoveAt(0);
            }

            _activeToasts.Add(toast);

            // Set up auto-dismissal timer
            var timer = new System.Threading.Timer(_ =>
            {
                RemoveToast(toast);
            }, null, toast.Duration, Timeout.InfiniteTimeSpan);
        }

        if (_dispatcherQueue != null && !_dispatcherQueue.HasThreadAccess)
        {
            _dispatcherQueue.TryEnqueue(AddToastOnUIThread);
        }
        else
        {
            AddToastOnUIThread();
        }
    }

    private void RemoveToast(ToastNotification toast)
    {
        void RemoveToastOnUIThread()
        {
            _activeToasts.Remove(toast);
        }

        if (_dispatcherQueue != null && !_dispatcherQueue.HasThreadAccess)
        {
            _dispatcherQueue.TryEnqueue(RemoveToastOnUIThread);
        }
        else
        {
            RemoveToastOnUIThread();
        }
    }
}

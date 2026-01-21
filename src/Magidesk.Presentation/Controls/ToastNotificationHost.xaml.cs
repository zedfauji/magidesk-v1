using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.Services;

namespace Magidesk.Presentation.Controls;

/// <summary>
/// UserControl that hosts and displays toast notifications in the top-right corner.
/// Implements auto-dismiss timer logic and manual dismiss functionality.
/// </summary>
public sealed partial class ToastNotificationHost : UserControl
{
    /// <summary>
    /// Dependency property for the ToastNotificationService.
    /// </summary>
    public static readonly DependencyProperty ToastServiceProperty =
        DependencyProperty.Register(
            nameof(ToastService),
            typeof(IToastNotificationService),
            typeof(ToastNotificationHost),
            new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the toast notification service.
    /// </summary>
    public IToastNotificationService ToastService
    {
        get => (IToastNotificationService)GetValue(ToastServiceProperty);
        set => SetValue(ToastServiceProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the ToastNotificationHost control.
    /// </summary>
    public ToastNotificationHost()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Handles the dismiss button click event to manually remove a toast notification.
    /// </summary>
    private void DismissButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is ToastNotification toast)
        {
            ToastService?.ActiveToasts.Remove(toast);
        }
    }
}

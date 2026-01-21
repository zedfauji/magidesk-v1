using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;

namespace Magidesk.Presentation.Services;

/// <summary>
/// Represents a toast notification with type, message, and display properties.
/// </summary>
public partial class ToastNotification : ObservableObject
{
    [ObservableProperty]
    private ToastType _type;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _message = string.Empty;

    [ObservableProperty]
    private string _icon = string.Empty;

    [ObservableProperty]
    private TimeSpan _duration;

    /// <summary>
    /// Gets the background brush based on the toast type.
    /// </summary>
    public Brush BackgroundBrush => Type switch
    {
        ToastType.Success => new SolidColorBrush(Colors.Green),
        ToastType.Error => new SolidColorBrush(Colors.Red),
        ToastType.Warning => new SolidColorBrush(Colors.Orange),
        ToastType.Info => new SolidColorBrush(Colors.Blue),
        _ => new SolidColorBrush(Colors.Gray)
    };
}

/// <summary>
/// Defines the types of toast notifications.
/// </summary>
public enum ToastType
{
    Success,
    Error,
    Warning,
    Info
}

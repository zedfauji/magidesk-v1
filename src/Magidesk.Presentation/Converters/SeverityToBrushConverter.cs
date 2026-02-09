using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using Magidesk.Application.DTOs;

namespace Magidesk.Presentation.Converters;

/// <summary>
/// Converts ErrorSeverity enum to appropriate brush color for UI display.
/// </summary>
public class SeverityToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is ErrorSeverity severity)
        {
            return severity switch
            {
                ErrorSeverity.Low => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 255)),
                ErrorSeverity.Medium => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 165, 0)),
                ErrorSeverity.High => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0)),
                ErrorSeverity.Critical => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 139, 0, 0)),
                _ => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 128, 128))
            };
        }
        return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 128, 128));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}
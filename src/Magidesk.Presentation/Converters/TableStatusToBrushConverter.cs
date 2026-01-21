using System;
using Magidesk.Domain.Enumerations;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using Microsoft.UI;

namespace Magidesk.Presentation.Converters;

/// <summary>
/// Converts TableStatus and optionally TableSessionStatus to a color brush.
/// Supports session-aware coloring to show paused sessions distinctly.
/// </summary>
public class TableStatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not TableStatus status)
        {
            return new SolidColorBrush(Colors.LightGray);
        }

        // Check if session status is provided as parameter
        TableSessionStatus? sessionStatus = parameter as TableSessionStatus?;

        // If session is paused, override with yellow regardless of table status
        if (sessionStatus == TableSessionStatus.Paused)
        {
            return new SolidColorBrush(Colors.Gold); // Yellow for paused
        }

        // Otherwise, use table status colors
        return status switch
        {
            TableStatus.Available => new SolidColorBrush(Colors.LightGreen),
            TableStatus.Seat => new SolidColorBrush(Colors.LightBlue), // Changed from Orange to LightBlue for better contrast
            TableStatus.Dirty => new SolidColorBrush(Colors.LightGray),
            TableStatus.Booked => new SolidColorBrush(Colors.LightSkyBlue),
            TableStatus.Disable => new SolidColorBrush(Colors.LightCoral), // Softer red for better accessibility
            _ => new SolidColorBrush(Colors.LightGray)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}

using System;
using Magidesk.Domain.Enumerations;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;

namespace Magidesk.Presentation.Converters;

public class TableStatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TableStatus status)
        {
            return status switch
            {
                TableStatus.Available => new SolidColorBrush(Microsoft.UI.Colors.LightGreen),
                TableStatus.Seat => new SolidColorBrush(Microsoft.UI.Colors.Orange),
                TableStatus.Dirty => new SolidColorBrush(Microsoft.UI.Colors.LightGray),
                TableStatus.Booked => new SolidColorBrush(Microsoft.UI.Colors.LightBlue),
                TableStatus.Disable => new SolidColorBrush(Microsoft.UI.Colors.Red),
                _ => new SolidColorBrush(Microsoft.UI.Colors.LightGray)
            };
        }
        
        return new SolidColorBrush(Microsoft.UI.Colors.LightGray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}

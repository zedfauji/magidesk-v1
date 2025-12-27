using Magidesk.Domain.Enumerations;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using System;

namespace Magidesk.Presentation.Converters;

public class TableStatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TableStatus status)
        {
            return status switch
            {
                TableStatus.Available => new SolidColorBrush(Colors.LightGreen),
                TableStatus.Seat => new SolidColorBrush(Colors.IndianRed),
                TableStatus.Booked => new SolidColorBrush(Colors.Orange),
                TableStatus.Dirty => new SolidColorBrush(Colors.Yellow),
                TableStatus.Disable => new SolidColorBrush(Colors.Gray),
                _ => new SolidColorBrush(Colors.LightGray)
            };
        }
        return new SolidColorBrush(Colors.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

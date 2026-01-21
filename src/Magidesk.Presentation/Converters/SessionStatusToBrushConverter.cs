using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Presentation.Converters;

/// <summary>
/// Converts TableSessionStatus to appropriate brush color.
/// </summary>
public class SessionStatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TableSessionStatus status)
        {
            return status switch
            {
                TableSessionStatus.Active => new SolidColorBrush(Colors.Green),
                TableSessionStatus.Paused => new SolidColorBrush(Colors.Orange),
                TableSessionStatus.Ended => new SolidColorBrush(Colors.Gray),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

/// <summary>
/// Converts a TimeSpan to hours (double) and vice versa.
/// </summary>
public class TimeSpanToHoursConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TimeSpan timeSpan)
        {
            return timeSpan.TotalHours;
        }

        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is double hours)
        {
            return TimeSpan.FromHours(hours);
        }

        return TimeSpan.Zero;
    }
}
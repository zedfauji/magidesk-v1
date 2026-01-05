using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

public class DateTimeToTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
            {
                return "Never";
            }

            var now = DateTime.Now;
            var diff = now - dateTime;

            if (diff.TotalSeconds < 60)
            {
                return "Just now";
            }
            else if (diff.TotalMinutes < 60)
            {
                return $"{(int)diff.TotalMinutes}m ago";
            }
            else if (diff.TotalHours < 24)
            {
                return $"{(int)diff.TotalHours}h ago";
            }
            else
            {
                return dateTime.ToString("MMM dd, HH:mm");
            }
        }

        return "Never";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}

using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;

namespace Magidesk.Presentation.Converters;

/// <summary>
/// Converts a boolean value to a success/error color brush.
/// True returns green (success), False returns red (error).
/// </summary>
public class BoolToSuccessColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
        {
            return boolValue 
                ? new SolidColorBrush(Colors.Green)
                : new SolidColorBrush(Colors.Red);
        }

        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}
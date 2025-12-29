using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool booleanValue)
        {
            return !booleanValue;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is bool booleanValue)
        {
            return !booleanValue;
        }
        return false;
    }
}

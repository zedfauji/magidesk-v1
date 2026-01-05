using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

public class BoolToDesignModeTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isDesignMode)
        {
            return isDesignMode ? "Design" : "View";
        }

        return "View";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}



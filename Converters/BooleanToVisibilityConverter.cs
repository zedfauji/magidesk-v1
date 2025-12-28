using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool flag = false;
        if (value is bool boolValue)
        {
            flag = boolValue;
        }
        
        if (parameter is string paramStr && paramStr.Equals("Reverse", StringComparison.OrdinalIgnoreCase))
        {
             flag = !flag;
        }

        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value is Visibility visibility && visibility == Visibility.Visible;
    }
}

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace Magidesk.Presentation.Converters;

public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool isNotNull = value != null;
        
        if (parameter is string paramStr && paramStr.Equals("Reverse", StringComparison.OrdinalIgnoreCase))
        {
            isNotNull = !isNotNull;
        }

        return isNotNull ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

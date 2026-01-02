using Microsoft.UI.Xaml.Data;
using System;

namespace Magidesk.Presentation.Converters;

public class IntToDoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int i)
        {
            return (double)i;
        }
        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is double d)
        {
            return (int)Math.Round(d);
        }
        return 0;
    }
}

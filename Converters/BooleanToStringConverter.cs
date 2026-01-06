using Microsoft.UI.Xaml.Data;
using System;

namespace Magidesk.Presentation.Converters;

public class BooleanToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
        {
            var options = parameter?.ToString()?.Split('|');
            if (options != null && options.Length == 2)
            {
                return boolValue ? options[0] : options[1];
            }
            return boolValue.ToString();
        }
        return "False";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

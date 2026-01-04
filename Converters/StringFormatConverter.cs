using System;
using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

public class StringFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter == null)
            return value?.ToString() ?? string.Empty;

        return string.Format(parameter.ToString(), value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

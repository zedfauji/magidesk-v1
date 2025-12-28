using System;
using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

public class StringFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null)
            return string.Empty;

        if (parameter == null)
            return value.ToString() ?? string.Empty;

        if (parameter is string formatString)
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, formatString, value);
        }

        return value.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

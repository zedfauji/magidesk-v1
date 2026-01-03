using System;
using Microsoft.UI.Xaml.Data;
using System.Globalization;

namespace Magidesk.Presentation.Converters;

public class CurrencyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is decimal decimalValue)
        {
            return decimalValue.ToString("C", CultureInfo.CurrentCulture);
        }
        if (value is double doubleValue)
        {
            return doubleValue.ToString("C", CultureInfo.CurrentCulture);
        }
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

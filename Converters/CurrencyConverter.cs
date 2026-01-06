using System;
using Microsoft.UI.Xaml.Data;
using System.Globalization;

using Microsoft.UI.Xaml;

namespace Magidesk.Presentation.Converters;

public class CurrencyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        try
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
        catch (Exception ex)
        {
            // T007: Log converter exception and return fallback
            System.Diagnostics.Debug.WriteLine($"CurrencyConverter Error: {ex.Message}");
            return "$0.00"; // Fallback value
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}

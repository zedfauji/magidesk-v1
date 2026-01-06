using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

public class StringFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        try
        {
            if (parameter == null)
                return value?.ToString() ?? string.Empty;

            return string.Format(parameter.ToString(), value);
        }
        catch (Exception ex)
        {
            // T007: Log converter exception and return fallback
            System.Diagnostics.Debug.WriteLine($"StringFormatConverter Error: {ex.Message}");
            return value?.ToString() ?? string.Empty; // Fallback value
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}

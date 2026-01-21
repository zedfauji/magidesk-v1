using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

public class StringFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter == null)
            return value?.ToString() ?? string.Empty;

        try
        {
            return string.Format(parameter.ToString(), value);
        }
        catch (FormatException ex)
        {
            // TICKET-012: Log format errors for debugging
            System.Diagnostics.Debug.WriteLine($"[StringFormatConverter] Invalid format string '{parameter}' for value '{value}': {ex.Message}");
            // Fallback to ToString instead of crashing
            return value?.ToString() ?? string.Empty;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}

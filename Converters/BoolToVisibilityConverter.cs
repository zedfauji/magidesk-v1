using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;

namespace Magidesk.Presentation.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        try
        {
            bool isReversed = parameter is string s && (s.Equals("Reverse", StringComparison.OrdinalIgnoreCase) || s.Equals("Invert", StringComparison.OrdinalIgnoreCase));

            if (value is bool boolValue)
            {
                if (isReversed)
                {
                    return !boolValue ? Visibility.Visible : Visibility.Collapsed;
                }
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            
            return Visibility.Collapsed;
        }
        catch (Exception ex)
        {
            // T007: Log converter exception and return fallback
            System.Diagnostics.Debug.WriteLine($"BoolToVisibilityConverter Error: {ex.Message}");
            return Visibility.Collapsed; // Fallback value
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        try
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            // T007: Log converter exception and return fallback
            System.Diagnostics.Debug.WriteLine($"BoolToVisibilityConverter ConvertBack Error: {ex.Message}");
            return false; // Fallback value
        }
    }
}

/// <summary>
/// Alias for BoolToVisibilityConverter to support legacy XAML references.
/// </summary>
public class BooleanToVisibilityConverter : BoolToVisibilityConverter { }

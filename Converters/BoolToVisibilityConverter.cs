using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;

namespace Magidesk.Presentation.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool isReversed = parameter is string s && (s.Equals("Reverse", StringComparison.OrdinalIgnoreCase) || s.Equals("Invert", StringComparison.OrdinalIgnoreCase) || s.Equals("Inverse", StringComparison.OrdinalIgnoreCase));

        bool boolValue;
        
        if (value is bool b)
        {
            boolValue = b;
        }
        else if (value is int intValue)
        {
            boolValue = intValue > 0;
        }
        else if (value is long longValue)
        {
            boolValue = longValue > 0;
        }
        else if (value is double doubleValue)
        {
            boolValue = doubleValue > 0;
        }
        else if (value is string stringValue)
        {
            boolValue = !string.IsNullOrEmpty(stringValue);
        }
        else
        {
            return Visibility.Collapsed;
        }

        if (isReversed)
        {
            return !boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        return boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }
        
        return false;
    }
}

/// <summary>
/// Alias for BoolToVisibilityConverter to support legacy XAML references.
/// </summary>
public class BooleanToVisibilityConverter : BoolToVisibilityConverter { }

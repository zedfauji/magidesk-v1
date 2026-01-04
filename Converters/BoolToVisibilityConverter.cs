using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;

namespace Magidesk.Presentation.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
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

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

public class StringVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var input = value as string;
        bool isVisible = !string.IsNullOrEmpty(input);

        if (parameter is string param && (param.Equals("Reverse", StringComparison.OrdinalIgnoreCase) || param.Equals("Invert", StringComparison.OrdinalIgnoreCase)))
        {
            isVisible = !isVisible;
        }

        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}

using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.UI.Xaml;

namespace Magidesk.Presentation.Converters;

public class StringColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string colorString && !string.IsNullOrEmpty(colorString))
        {
            try
            {
                if (colorString.StartsWith("#"))
                {
                    colorString = colorString.Substring(1);
                    if (colorString.Length == 6)
                        colorString = "FF" + colorString;

                    byte a = System.Convert.ToByte(colorString.Substring(0, 2), 16);
                    byte r = System.Convert.ToByte(colorString.Substring(2, 2), 16);
                    byte g = System.Convert.ToByte(colorString.Substring(4, 2), 16);
                    byte b = System.Convert.ToByte(colorString.Substring(6, 2), 16);

                    return new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
                }
            }
            catch
            {
                // Silently fail and return default
            }
        }

        return new SolidColorBrush(Colors.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}

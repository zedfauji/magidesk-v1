using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Microsoft.UI;
using Windows.UI;

namespace Magidesk.Presentation.Converters;

public class StringColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string colorCode && !string.IsNullOrWhiteSpace(colorCode))
        {
            try
            {
                // Attempt to parse Hex like #RRGGBB or #AARRGGBB
                if (colorCode.StartsWith("#"))
                {
                    colorCode = colorCode.Replace("#", "");
                    byte a = 255;
                    byte r = 0;
                    byte g = 0;
                    byte b = 0;

                    if (colorCode.Length == 6)
                    {
                        r = System.Convert.ToByte(colorCode.Substring(0, 2), 16);
                        g = System.Convert.ToByte(colorCode.Substring(2, 2), 16);
                        b = System.Convert.ToByte(colorCode.Substring(4, 2), 16);
                    }
                    else if (colorCode.Length == 8)
                    {
                        a = System.Convert.ToByte(colorCode.Substring(0, 2), 16);
                        r = System.Convert.ToByte(colorCode.Substring(2, 2), 16);
                        g = System.Convert.ToByte(colorCode.Substring(4, 2), 16);
                        b = System.Convert.ToByte(colorCode.Substring(6, 2), 16);
                    }

                    return new SolidColorBrush(Color.FromArgb(a, r, g, b));
                }
            }
            catch
            {
                // Fallback
            }
        }

        // Fallback default
        return new SolidColorBrush(Colors.White); 
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Magidesk.Presentation.Converters;

/// <summary>
/// Converts boolean resolved status to appropriate brush color.
/// </summary>
public class BooleanToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isResolved)
        {
            return isResolved 
                ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 128, 0)) 
                : new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 165, 0));
        }
        return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 128, 128));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
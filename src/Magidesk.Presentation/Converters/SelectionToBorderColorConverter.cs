using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Magidesk.Presentation.Converters;

/// <summary>
/// Converts boolean IsSelected to border color (Blue if selected, Gray if not).
/// </summary>
public class SelectionToBorderColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isSelected)
        {
            return isSelected ? Colors.DodgerBlue : Colors.Gray;
        }
        return Colors.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

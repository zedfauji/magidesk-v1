using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

public class DecimalToDoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is decimal d)
        {
            return (double)d;
        }
        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is double d)
        {
            // Handle NaN or Infinity if necessary, though NumberBox usually restricts
            if (double.IsNaN(d) || double.IsInfinity(d)) return 0m;
            return (decimal)d;
        }
        return 0m;
    }
}

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

public class EnumToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null || parameter == null)
            return false;

        var enumValue = value.ToString();
        var parameterValue = parameter.ToString();

        return enumValue.Equals(parameterValue, StringComparison.OrdinalIgnoreCase);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue && boolValue && parameter != null)
        {
            return Enum.Parse(targetType, parameter.ToString());
        }

        return DependencyProperty.UnsetValue;
    }
}

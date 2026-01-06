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
            // F-CONV-002 FIX (TICKET-007): Wrap Enum.Parse in try-catch
            try
            {
                return Enum.Parse(targetType, parameter.ToString());
            }
            catch (Exception ex)
            {
                // Log the error with details
                System.Diagnostics.Debug.WriteLine(
                    $"[EnumToBoolConverter] Failed to parse enum value. " +
                    $"TargetType: {targetType?.Name}, Parameter: '{parameter}', Error: {ex.Message}");
            }
        }

        return DependencyProperty.UnsetValue;
    }
}

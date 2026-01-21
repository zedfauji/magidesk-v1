using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

/// <summary>
/// Converts an enum value to a user-friendly string representation.
/// </summary>
public class EnumToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null)
            return string.Empty;

        // Convert enum to friendly string
        var enumString = value.ToString();
        
        // Handle specific enum types with custom formatting
        if (value is ViewModels.TimeRoundingRule roundingRule)
        {
            return roundingRule switch
            {
                ViewModels.TimeRoundingRule.FifteenMinutes => "15 minutes",
                ViewModels.TimeRoundingRule.ThirtyMinutes => "30 minutes",
                ViewModels.TimeRoundingRule.SixtyMinutes => "60 minutes",
                _ => enumString
            };
        }

        // Default: Add spaces before capital letters
        return System.Text.RegularExpressions.Regex.Replace(enumString, "([a-z])([A-Z])", "$1 $2");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
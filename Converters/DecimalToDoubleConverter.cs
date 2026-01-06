using System;
using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

/// <summary>
/// Converts between decimal and double for UI binding purposes.
/// </summary>
/// <remarks>
/// TICKET-016: PRECISION LOSS WARNING
/// 
/// This converter is used EXCLUSIVELY for WinUI NumberBox controls which require double values.
/// Precision loss may occur for very large decimal values during conversion.
/// 
/// ACCEPTABLE USAGE:
/// - TwoWay binding with NumberBox for user input (Price, Amount, Percentages)
/// - Values are immediately converted back to decimal via ConvertBack
/// - Precision loss is acceptable for UI display and input purposes
/// 
/// UNACCEPTABLE USAGE:
/// - Financial calculations (use decimal directly)
/// - Data persistence (use decimal directly)
/// - Any scenario where exact precision is required
/// 
/// CURRENT USAGE (verified safe):
/// - MiscItemDialog.xaml: Price field (TwoWay binding)
/// - TicketFeeDialog.xaml: Amount field (TwoWay binding)
/// - SystemConfigPage.xaml: Service charge %, Gratuity %, Signature threshold (TwoWay binding)
/// </remarks>
public class DecimalToDoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is decimal decimalValue)
        {
            return (double)decimalValue;
        }
        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is double doubleValue)
        {
            return (decimal)doubleValue;
        }
        return 0m;
    }
}

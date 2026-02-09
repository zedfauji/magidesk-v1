using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Presentation.Converters;

/// <summary>
/// Converts TicketStatus to appropriate brush color for visual indicators.
/// </summary>
public class TicketStatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TicketStatus status)
        {
            return status switch
            {
                TicketStatus.Draft => new SolidColorBrush(Colors.LightGray),
                TicketStatus.Open => new SolidColorBrush(Colors.Green),
                TicketStatus.Held => new SolidColorBrush(Colors.Orange),
                TicketStatus.Paid => new SolidColorBrush(Colors.Blue),
                TicketStatus.Closed => new SolidColorBrush(Colors.Gray),
                TicketStatus.Voided => new SolidColorBrush(Colors.Red),
                TicketStatus.Refunded => new SolidColorBrush(Colors.Purple),
                TicketStatus.Scheduled => new SolidColorBrush(Colors.CornflowerBlue),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}

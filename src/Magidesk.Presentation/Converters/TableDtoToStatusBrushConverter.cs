using System;
using Magidesk.Application.DTOs;
using Magidesk.Domain.Enumerations;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace Magidesk.Presentation.Converters;

/// <summary>
/// Converts a TableDto to a color brush based on both table status and session status.
/// This converter takes the entire TableDto to access both Status and SessionStatus properties.
/// </summary>
public class TableDtoToStatusBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not TableDto table)
        {
            return new SolidColorBrush(Colors.LightGray);
        }

        // If session is paused, override with yellow regardless of table status
        if (table.SessionStatus == TableSessionStatus.Paused)
        {
            return new SolidColorBrush(Colors.Gold); // Yellow for paused
        }

        // Otherwise, use table status colors
        return table.Status switch
        {
            TableStatus.Available => new SolidColorBrush(Colors.LightGreen),
            TableStatus.Seat => new SolidColorBrush(Colors.LightBlue), // Changed from Orange to LightBlue for better contrast
            TableStatus.Dirty => new SolidColorBrush(Colors.LightGray),
            TableStatus.Booked => new SolidColorBrush(Colors.LightSkyBlue),
            TableStatus.Disable => new SolidColorBrush(Colors.LightCoral), // Softer red for better accessibility
            _ => new SolidColorBrush(Colors.LightGray)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

using Microsoft.UI.Xaml.Data;
using Magidesk.Application.DTOs;

namespace Magidesk.Presentation.Converters;

/// <summary>
/// Converts SessionAlertType to appropriate FontIcon glyph.
/// </summary>
public class AlertTypeToGlyphConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is SessionAlertType alertType)
        {
            return alertType switch
            {
                SessionAlertType.LongPause => "\uE7BA", // Warning
                SessionAlertType.LongRunning => "\uE7C4", // Clock
                SessionAlertType.EquipmentIssue => "\uE90F", // Repair
                SessionAlertType.CapacityWarning => "\uE7C5", // People
                SessionAlertType.MaintenanceRequired => "\uE90F", // Repair
                SessionAlertType.SystemIssue => "\uE783", // Error
                _ => "\uE7BA" // Default warning
            };
        }

        return "\uE7BA"; // Default warning
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
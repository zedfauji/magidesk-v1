using System;
using System.Text;
using Magidesk.Application.DTOs;
using Magidesk.Domain.Enumerations;
using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

/// <summary>
/// Converts a TableDto to a detailed tooltip string showing status information.
/// </summary>
public class TableStatusToTooltipConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not TableDto table)
        {
            return string.Empty;
        }

        var tooltip = new StringBuilder();
        tooltip.AppendLine($"Table {table.TableNumber}");
        tooltip.AppendLine($"Capacity: {table.Capacity}");
        tooltip.AppendLine($"Status: {GetStatusDisplayName(table.Status)}");

        // Add session information if available
        if (table.SessionId.HasValue)
        {
            tooltip.AppendLine();
            tooltip.AppendLine("Session Information:");
            tooltip.AppendLine($"Status: {GetSessionStatusDisplayName(table.SessionStatus)}");
            
            if (table.SessionElapsedTimeDisplay != null)
            {
                tooltip.AppendLine($"Elapsed Time: {table.SessionElapsedTimeDisplay}");
            }
            
            if (table.SessionRunningChargeDisplay != null)
            {
                tooltip.AppendLine($"Running Charge: {table.SessionRunningChargeDisplay}");
            }
            
            if (table.SessionHourlyRate.HasValue)
            {
                tooltip.AppendLine($"Hourly Rate: ${table.SessionHourlyRate.Value:F2}");
            }
        }

        return tooltip.ToString().TrimEnd();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }

    private static string GetStatusDisplayName(TableStatus status)
    {
        return status switch
        {
            TableStatus.Available => "Available",
            TableStatus.Seat => "In Use",
            TableStatus.Dirty => "Needs Cleaning",
            TableStatus.Booked => "Reserved",
            TableStatus.Disable => "Disabled",
            _ => status.ToString()
        };
    }

    private static string GetSessionStatusDisplayName(TableSessionStatus? status)
    {
        if (!status.HasValue)
            return "None";

        return status.Value switch
        {
            TableSessionStatus.Active => "Active",
            TableSessionStatus.Paused => "Paused",
            TableSessionStatus.Ended => "Ended",
            _ => status.Value.ToString()
        };
    }
}

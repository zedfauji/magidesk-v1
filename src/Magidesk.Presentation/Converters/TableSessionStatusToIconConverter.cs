using System;
using Magidesk.Domain.Enumerations;
using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

/// <summary>
/// Converts TableSessionStatus to a Segoe Fluent Icons glyph.
/// </summary>
public class TableSessionStatusToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not TableSessionStatus sessionStatus)
        {
            return string.Empty; // No icon if no session
        }

        return sessionStatus switch
        {
            TableSessionStatus.Active => "\uE121", // Clock icon
            TableSessionStatus.Paused => "\uE769", // Pause icon
            TableSessionStatus.Ended => string.Empty, // No icon for ended sessions
            _ => string.Empty
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;

namespace Magidesk.Presentation.Converters;

public class TableToSelectionVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        // This converter is used to show/hide selection indicators
        // The value should be the selected table, and the parameter should be the current table
        if (value != null && parameter != null)
        {
            return value.Equals(parameter) ? Visibility.Visible : Visibility.Collapsed;
        }
        
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}

using System;
using System.Collections;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;

namespace Magidesk.Presentation.Converters;

public class CollectionEmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is IEnumerable collection)
        {
            var enumerator = collection.GetEnumerator();
            bool hasItems = enumerator.MoveNext();
            
            // Default: Empty -> Visible, Items -> Collapsed
            // If parameter is "Inverse": Empty -> Collapsed, Items -> Visible
            bool isInverse = parameter?.ToString() == "Inverse";
            
            if (isInverse)
                return hasItems ? Visibility.Visible : Visibility.Collapsed;
            else
                return hasItems ? Visibility.Collapsed : Visibility.Visible;
        }
        
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}

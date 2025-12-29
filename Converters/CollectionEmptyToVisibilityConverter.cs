using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System.Collections;

namespace Magidesk.Presentation.Converters;

public class CollectionEmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int count)
        {
            return count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        if (value is ICollection collection)
        {
             return collection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed; // Default to hidden if not a collection/count
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

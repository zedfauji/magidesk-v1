using System;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Magidesk.Domain.Entities;

namespace Magidesk.Presentation.Converters;

public class MenuItemToLowStockBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is MenuItem item)
        {
            if (item.TrackStock)
            {
                if (item.StockQuantity <= 0)
                {
                    // Out of Stock - Red
                    return new SolidColorBrush(Colors.Red);
                }
                
                if (item.StockQuantity <= item.MinimumStockLevel)
                {
                    // Low Stock - Orange/Amber
                    return new SolidColorBrush(Colors.Orange);
                }
            }
        }
        
        // Default / Transparent
        return new SolidColorBrush(Colors.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

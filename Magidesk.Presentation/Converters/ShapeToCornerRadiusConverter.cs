using Magidesk.Application.DTOs;
using Magidesk.Domain.Enumerations;
using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

public class ShapeToCornerRadiusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TableShapeType shape)
        {
            return ConvertShape(shape);
        }

        if (value is int intValue)
        {
            return ConvertShape((TableShapeType)intValue);
        }

        return new Microsoft.UI.Xaml.CornerRadius(8);
    }

    private Microsoft.UI.Xaml.CornerRadius ConvertShape(TableShapeType shape)
    {
        return shape switch
        {
            TableShapeType.Rectangle => new Microsoft.UI.Xaml.CornerRadius(8),
            TableShapeType.Square => new Microsoft.UI.Xaml.CornerRadius(8),
            TableShapeType.Round => new Microsoft.UI.Xaml.CornerRadius(50),
            TableShapeType.Oval => new Microsoft.UI.Xaml.CornerRadius(50),
            _ => new Microsoft.UI.Xaml.CornerRadius(8)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

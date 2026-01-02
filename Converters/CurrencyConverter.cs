using System;
using Microsoft.UI.Xaml.Data;

namespace Magidesk.Presentation.Converters;

public class CurrencyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is decimal d)
        {
            return d.ToString(parameter as string ?? "C2");
        }
        if (value is double db)
        {
            return db.ToString(parameter as string ?? "C2");
        }
        if (value is float f)
        {
            return f.ToString(parameter as string ?? "C2");
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

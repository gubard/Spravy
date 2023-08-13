using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Spravy.Ui.Converters;

public class Int32ToIsVisibleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int i)
        {
            if (i > 0)
            {
                return true;
            }
        }

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Spravy.Ui.Converters;

public class Int32ToIsVisibleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int i)
        {
            return false;
        }

        return i > 0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
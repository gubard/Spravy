using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Spravy.Ui.Converters;

public class UInt32ZeroToBooleanConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not uint item)
        {
            return false;
        }

        return item > 0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Spravy.Ui.Converters;

public class NullToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
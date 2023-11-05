using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Spravy.Domain.Extensions;

namespace Spravy.Ui.Converters;

public class NullOrWhiteSpaceToBooleanValueConverter : IValueConverter
{
    public bool Reverse { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string str)
        {
            return false;
        }

        if (Reverse)
        {
            return !str.IsNullOrWhiteSpace();
        }

        return str.IsNullOrWhiteSpace();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
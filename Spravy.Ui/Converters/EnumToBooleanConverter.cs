using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Spravy.Ui.Converters;

public class EnumToBooleanConverter<TEnum> : IValueConverter where TEnum : struct, Enum
{
    public TEnum Value { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not TEnum item)
        {
            return false;
        }

        return Value.Equals(item);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
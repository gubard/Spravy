namespace Spravy.Ui.Converters;

public class EnumToBooleanConverter<TEnum> : IValueConverter where TEnum : struct, Enum
{
    public TEnum Value { get; set; }
    public bool Reverse { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not TEnum item)
        {
            return false;
        }

        if (Reverse)
        {
            return !Value.Equals(item);
        }

        return Value.Equals(item);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
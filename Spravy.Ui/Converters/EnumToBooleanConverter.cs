namespace Spravy.Ui.Converters;

public class EnumToBooleanConverter<TEnum> : IValueConverter
    where TEnum : struct, Enum
{
    public bool Reverse { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not TEnum item)
        {
            return false;
        }
        
        if (parameter is not TEnum parameterEnum)
        {
            return false;
        }

        if (Reverse)
        {
            return !parameterEnum.Equals(item);
        }

        return parameterEnum.Equals(item);
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotSupportedException();
    }
}

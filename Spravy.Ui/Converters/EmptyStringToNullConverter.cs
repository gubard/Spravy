namespace Spravy.Ui.Converters;

public class EmptyStringToNullConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string str)
        {
            return value;
        }

        return str.IsNullOrWhiteSpace() ? null : value;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is null)
        {
            return string.Empty;
        }

        return value;
    }
}

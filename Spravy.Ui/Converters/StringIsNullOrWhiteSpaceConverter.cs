namespace Spravy.Ui.Converters;

public class StringIsNullOrWhiteSpaceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var revers = false;

        if (parameter is bool r)
        {
            revers = r;
        }

        if (value is not string str)
        {
            return false;
        }

        if (revers)
        {
            return !str.IsNullOrWhiteSpace();
        }

        return str.IsNullOrWhiteSpace();
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

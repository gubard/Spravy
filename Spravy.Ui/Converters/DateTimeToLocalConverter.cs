namespace Spravy.Ui.Converters;

public class DateTimeToLocalConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime { Kind: DateTimeKind.Utc } dateTime)
        {
            return dateTime.ToLocalTime();
        }

        return value;
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        return value;
    }
}

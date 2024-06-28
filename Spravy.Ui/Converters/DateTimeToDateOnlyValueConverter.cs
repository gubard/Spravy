namespace Spravy.Ui.Converters;

public class DateTimeToDateOnlyValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateOnly dateOnly)
        {
            return value;
        }

        if (DateOnly.MinValue == dateOnly)
        {
            return DateTime.MinValue;
        }

        return new DateTime(dateOnly.Year, dateOnly.Month, dateOnly.Day, 0, 0, 0);
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not DateTime dateTimeOffset)
        {
            return value;
        }

        return new DateOnly(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day);
    }
}

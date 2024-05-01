namespace Spravy.Ui.Converters;

public class DateTimeOffsetToDateOnlyValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateOnly dateOnly)
        {
            return value;
        }

        if (DateOnly.MinValue == dateOnly)
        {
            return DateTimeOffset.MinValue;
        }

        return new DateTimeOffset(dateOnly.Year, dateOnly.Month, dateOnly.Day, 0, 0, 0, DateTimeOffset.Now.Offset);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTimeOffset dateTimeOffset)
        {
            return value;
        }

        return new DateOnly(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day);
    }
}
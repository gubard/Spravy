namespace Spravy.Domain.Extensions;

public static class DateTimeExtension
{
    public static DateOnly ToDateOnly(this DateTime dateTime)
    {
        return new(dateTime.Year, dateTime.Month, dateTime.Day);
    }
}
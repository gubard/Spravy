namespace Spravy.Domain.Extensions;

public static class DateTimeOffsetExtension
{
    public static DateTimeOffset ToCurrentDay(this DateTimeOffset date)
    {
        var result = new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, date.Offset);

        return result;
    }

    public static DateTimeOffset? ToCurrentDayOrNull(this DateTimeOffset? date)
    {
        if (date is null)
        {
            return null;
        }

        return date.Value.ToCurrentDay();
    }

    public static DateTimeOffset WithDay(this DateTimeOffset date, int day)
    {
        return new DateTimeOffset(
            date.Year,
            date.Month,
            day,
            date.Hour,
            date.Minute,
            date.Second,
            date.Millisecond,
            date.Microsecond,
            date.Offset
        );
    }

    public static DateTimeOffset WithMonth(this DateTimeOffset date, int month)
    {
        return new DateTimeOffset(
            date.Year,
            month,
            date.Day,
            date.Hour,
            date.Minute,
            date.Second,
            date.Millisecond,
            date.Microsecond,
            date.Offset
        );
    }
}
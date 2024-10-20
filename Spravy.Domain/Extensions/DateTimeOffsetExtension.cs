namespace Spravy.Domain.Extensions;

public static class DateTimeOffsetExtension
{
    public static DateTimeOffset GetCurrentDate(this DateTimeOffset date)
    {
        return new(date.Year, date.Month, date.Day, 0, 0, 0, date.Offset);
    }

    public static DateTime ToDateTimeWithOffset(this DateTimeOffset date)
    {
        return date.DateTime.Add(date.Offset);
    }

    public static bool IsToday(this DateTimeOffset date)
    {
        var nowDay = DateTimeOffset.Now.ToDayDateTimeWithOffset();
        var day = date.ToDayDateTimeWithOffset();

        return day == nowDay;
    }

    public static DateTime ToDayDateTimeWithOffset(this DateTimeOffset date)
    {
        return date.ToDateTimeWithOffset().Date;
    }

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
        return new(
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
        return new(
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

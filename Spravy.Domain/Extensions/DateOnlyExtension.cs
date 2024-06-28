namespace Spravy.Domain.Extensions;

public static class DateOnlyExtension
{
    public static DateOnly WithDay(this DateOnly date, int day)
    {
        return new(date.Year, date.Month, day);
    }

    public static DateOnly WithMonth(this DateOnly date, int month)
    {
        return new(date.Year, month, date.Day);
    }

    public static DateTime ToDateTime(this DateOnly date, DateTimeKind kind)
    {
        return new(date.Year, date.Month, date.Day, 0, 0, 0, 0, 0, kind);
    }

    public static DateTimeOffset ToDateTimeOffset(this DateOnly date)
    {
        return new(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeOffset.Now.Offset);
    }
}

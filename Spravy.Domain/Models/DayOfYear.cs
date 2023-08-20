namespace Spravy.Domain.Models;

public readonly struct DayOfYear : IComparable<DayOfYear>
{
    public DayOfYear(byte day, byte month)
    {
        Day = day;
        Month = month;
    }

    public byte Day { get; }
    public byte Month { get; }


    public int CompareTo(DayOfYear other)
    {
        var year = DateTime.Now.Year;
        var x = new DateOnly(year, Month, Math.Min(DateTime.DaysInMonth(year, Month), Day));
        var y = new DateOnly(year, other.Month, Math.Min(DateTime.DaysInMonth(year, other.Month), other.Day));

        return x.CompareTo(y);
    }
}
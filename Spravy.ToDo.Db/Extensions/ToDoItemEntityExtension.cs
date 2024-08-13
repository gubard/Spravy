using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Db.Models;

namespace Spravy.ToDo.Db.Extensions;

public static class ToDoItemEntityExtension
{
    public static ReadOnlyMemory<byte> GetDaysOfMonth(this ToDoItemEntity item)
    {
        if (item.DaysOfMonth.IsNullOrWhiteSpace())
        {
            return ReadOnlyMemory<byte>.Empty;
        }

        var values = item.DaysOfMonth.Split(";");
        var result = new Memory<byte>(new byte[values.Length]);

        for (var index = 0; index < values.Length; index++)
        {
            if (byte.TryParse(values[index], out var b))
            {
                result.Span[index] = b;
            }
            else
            {
                return ReadOnlyMemory<byte>.Empty;
            }
        }

        return result;
    }

    public static IEnumerable<DayOfWeek> GetDaysOfWeek(this ToDoItemEntity item)
    {
        if (item.DaysOfWeek.IsNullOrWhiteSpace())
        {
            return Enumerable.Empty<DayOfWeek>();
        }

        return item.DaysOfWeek.Split(";").Select(Enum.Parse<DayOfWeek>);
    }

    public static IEnumerable<DayOfYear> GetDaysOfYear(this ToDoItemEntity item)
    {
        if (item.DaysOfYear.IsNullOrWhiteSpace())
        {
            return Enumerable.Empty<DayOfYear>();
        }

        return item
            .DaysOfYear.Split(";")
            .Select(x =>
            {
                var value = x.Split(".");

                return new DayOfYear(byte.Parse(value[1]), byte.Parse(value[0]));
            });
    }

    public static void SetDaysOfYear(this ToDoItemEntity item, ReadOnlyMemory<DayOfYear> daysOfYear)
    {
        item.DaysOfYear = daysOfYear.Select(x => $"{x.Month}.{x.Day}").JoinString(";");
    }

    public static void SetDaysOfMonth(this ToDoItemEntity item, ReadOnlyMemory<byte> daysOfMonth)
    {
        item.DaysOfMonth = daysOfMonth.JoinString(";");
    }

    public static void SetDaysOfWeek(this ToDoItemEntity item, ReadOnlyMemory<DayOfWeek> daysOfWeek)
    {
        item.DaysOfWeek = daysOfWeek.JoinString(";");
    }
}

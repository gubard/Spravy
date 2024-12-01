using Spravy.Domain.Enums;
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

    public static ReadOnlyMemory<DayOfWeek> GetDaysOfWeek(this ToDoItemEntity item)
    {
        if (item.DaysOfWeek.IsNullOrWhiteSpace())
        {
            return ReadOnlyMemory<DayOfWeek>.Empty;
        }

        return item.DaysOfWeek.Split(";").Select(Enum.Parse<DayOfWeek>).ToArray();
    }

    public static ReadOnlyMemory<DayOfYear> GetDaysOfYear(this ToDoItemEntity item)
    {
        if (item.DaysOfYear.IsNullOrWhiteSpace())
        {
            return ReadOnlyMemory<DayOfYear>.Empty;
        }

        return item.DaysOfYear
           .Split(";")
           .Select(
                x =>
                {
                    var value = x.Split(".");

                    return new DayOfYear(byte.Parse(value[1]), (Month)byte.Parse(value[0]));
                }
            )
           .ToArray();
    }

    public static void SetAnnuallyDays(this ToDoItemEntity item, ReadOnlyMemory<DayOfYear> daysOfYear)
    {
        item.DaysOfYear = daysOfYear.Select(x => $"{(byte)x.Month}.{x.Day}").JoinString(";");
    }

    public static void SetMonthlyDays(this ToDoItemEntity item, ReadOnlyMemory<byte> daysOfMonth)
    {
        item.DaysOfMonth = daysOfMonth.JoinString(";");
    }

    public static void SetWeeklyDays(this ToDoItemEntity item, ReadOnlyMemory<DayOfWeek> daysOfWeek)
    {
        item.DaysOfWeek = daysOfWeek.JoinString(";");
    }
}
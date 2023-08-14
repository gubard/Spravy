using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.Common.Models;
using Spravy.Db.Models;

namespace Spravy.Db.Extension;

public static class ToDoItemEntityExtension
{
    public static IEnumerable<byte> GetDaysOfMonth(this ToDoItemEntity item)
    {
        if (item.DaysOfMonth.IsNullOrWhiteSpace())
        {
            return Enumerable.Empty<byte>();
        }

        return item.DaysOfMonth.Split(";").Select(byte.Parse);
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

        return item.DaysOfYear.Split(";")
            .Select(
                x =>
                {
                    var value = x.Split(".");

                    return new DayOfYear(byte.Parse(value[1]), byte.Parse(value[0]));
                }
            );
    }

    public static void SetDaysOfYear(this ToDoItemEntity item, IEnumerable<DayOfYear> daysOfYear)
    {
        item.DaysOfYear = daysOfYear.Select(x => $"{x.Month}.{x.Day}").JoinString(";");
    }

    public static void SetDaysOfMonth(this ToDoItemEntity item, IEnumerable<byte> daysOfMonth)
    {
        item.DaysOfMonth = daysOfMonth.JoinString(";");
    }

    public static void SetDaysOfWeek(this ToDoItemEntity item, IEnumerable<DayOfWeek> daysOfWeek)
    {
        item.DaysOfWeek = daysOfWeek.JoinString(";");
    }
}
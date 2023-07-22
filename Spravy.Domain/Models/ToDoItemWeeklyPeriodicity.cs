using ExtensionFramework.Core.Common.Models;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Models;

public readonly struct ToDoItemWeeklyPeriodicity : IToDoItemPeriodicity
{
    public ToDoItemWeeklyPeriodicity(IEnumerable<WeeklyDayTime> days)
    {
        var set = new HashSet<WeeklyDayTime>();

        foreach (var day in days)
        {
            set.Add(day);
        }

        Days = set;
    }

    public IReadOnlySet<WeeklyDayTime> Days { get; }
}
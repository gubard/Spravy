using Spravy.Core.Interfaces;

namespace Spravy.Core.Models;

public readonly struct ToDoItemDailyPeriodicity : IToDoItemPeriodicity
{
    public ToDoItemDailyPeriodicity(IEnumerable<TimeOnly> times)
    {
        var set = new HashSet<TimeOnly>();

        foreach (var time in times)
        {
            set.Add(time);
        }

        Times = set;
    }

    public IReadOnlySet<TimeOnly> Times { get; }
}
using ExtensionFramework.Core.Common.Models;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Models;

public readonly struct ToDoItemMonthlyPeriodicity : IToDoItemPeriodicity
{
    public ToDoItemMonthlyPeriodicity(IEnumerable<MonthDayTime> dayTimes)
    {
        var set = new HashSet<MonthDayTime>();

        foreach (var time in dayTimes)
        {
            set.Add(time);
        }

        DayTimes = set;
    }

    public IReadOnlySet<MonthDayTime> DayTimes { get; }
}
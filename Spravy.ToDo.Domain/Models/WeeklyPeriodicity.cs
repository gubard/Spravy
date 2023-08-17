using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Models;

public readonly struct WeeklyPeriodicity : IPeriodicity
{
    public WeeklyPeriodicity(IEnumerable<DayOfWeek> days)
    {
        Days = new SortedSet<DayOfWeek>(days);
    }

    public IReadOnlySet<DayOfWeek> Days { get; }
}
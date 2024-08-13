using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Models;

public readonly struct WeeklyPeriodicity : IPeriodicity
{
    public WeeklyPeriodicity(IEnumerable<DayOfWeek> days)
    {
        Days = days.ToArray();
    }

    public ReadOnlyMemory<DayOfWeek> Days { get; }
}

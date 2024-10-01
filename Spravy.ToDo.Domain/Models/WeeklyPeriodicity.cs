namespace Spravy.ToDo.Domain.Models;

public readonly struct WeeklyPeriodicity
{
    public WeeklyPeriodicity(ReadOnlyMemory<DayOfWeek> days)
    {
        Days = days.ToArray();
    }

    public ReadOnlyMemory<DayOfWeek> Days { get; }
}

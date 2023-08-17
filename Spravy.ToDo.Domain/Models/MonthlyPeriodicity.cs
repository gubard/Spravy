using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Models;

public readonly struct MonthlyPeriodicity : IPeriodicity
{
    public MonthlyPeriodicity(IEnumerable<byte> days)
    {
        Days = new SortedSet<byte>(days);
    }

    public IReadOnlySet<byte> Days { get; }
}
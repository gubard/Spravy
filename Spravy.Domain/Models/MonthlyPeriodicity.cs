using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Models;

public readonly struct MonthlyPeriodicity : IPeriodicity
{
    public MonthlyPeriodicity(IEnumerable<byte> days)
    {
        Days = new SortedSet<byte>(days);
    }

    public IReadOnlySet<byte> Days { get; }
}
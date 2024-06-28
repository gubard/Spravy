using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Models;

public readonly struct AnnuallyPeriodicity : IPeriodicity
{
    public AnnuallyPeriodicity(IEnumerable<DayOfYear> days)
    {
        Days = new SortedSet<DayOfYear>(days);
    }

    public IReadOnlySet<DayOfYear> Days { get; }
}

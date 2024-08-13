using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Models;

public readonly struct AnnuallyPeriodicity : IPeriodicity
{
    public AnnuallyPeriodicity(IEnumerable<DayOfYear> days)
    {
        Days = days.ToArray();
    }

    public ReadOnlyMemory<DayOfYear> Days { get; }
}

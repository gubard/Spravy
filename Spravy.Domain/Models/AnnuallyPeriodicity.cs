using ExtensionFramework.Core.Common.Models;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Models;

public readonly struct AnnuallyPeriodicity : IPeriodicity
{
    public AnnuallyPeriodicity(IEnumerable<DayOfYear> days)
    {
        Days = new SortedSet<DayOfYear>(days);
    }

    public IReadOnlySet<DayOfYear> Days { get; }
}
using Spravy.Domain.Models;

namespace Spravy.ToDo.Domain.Models;

public readonly struct AnnuallyPeriodicity
{
    public AnnuallyPeriodicity(ReadOnlyMemory<DayOfYear> days)
    {
        Days = days.ToArray();
    }

    public ReadOnlyMemory<DayOfYear> Days { get; }
}

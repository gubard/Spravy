using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Models;

public readonly struct MonthlyPeriodicity : IPeriodicity
{
    public MonthlyPeriodicity(ReadOnlyMemory<byte> days)
    {
        Days = days;
    }

    public ReadOnlyMemory<byte> Days { get; }
}

namespace Spravy.ToDo.Domain.Models;

public readonly struct MonthlyPeriodicity
{
    public MonthlyPeriodicity(ReadOnlyMemory<byte> days)
    {
        Days = days;
    }

    public ReadOnlyMemory<byte> Days { get; }
}

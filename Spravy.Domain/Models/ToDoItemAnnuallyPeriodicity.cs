using ExtensionFramework.Core.Common.Models;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Models;

public readonly struct ToDoItemAnnuallyPeriodicity : IToDoItemPeriodicity
{
    public ToDoItemAnnuallyPeriodicity(IEnumerable<YearDateTime> days)
    {
        var set = new HashSet<YearDateTime>();

        foreach (var time in days)
        {
            set.Add(time);
        }

        Days = set;
    }

    public IReadOnlySet<YearDateTime> Days { get; }
}
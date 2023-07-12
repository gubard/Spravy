using ExtensionFramework.Core.Common.Models;
using Spravy.Core.Interfaces;

namespace Spravy.Core.Models;

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
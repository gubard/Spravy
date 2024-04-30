using System.Linq;
using Avalonia.Collections;
using ReactiveUI;

namespace Spravy.Ui.Models;

public class DayOfYearSelectItem : NotifyBase
{
    private byte month;

    public DayOfYearSelectItem()
    {
        Days = new(Enumerable.Range(1, 31)
           .Select(x => new DayOfMonthSelectItem
            {
                Day = (byte)x,
            }));
    }

    public byte Month
    {
        get => month;
        set => this.RaiseAndSetIfChanged(ref month, value);
    }

    public AvaloniaList<DayOfMonthSelectItem> Days { get; }
}
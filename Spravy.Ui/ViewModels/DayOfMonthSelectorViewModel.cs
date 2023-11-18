using System.Linq;
using Avalonia.Collections;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class DayOfMonthSelectorViewModel : ViewModelBase
{
    public DayOfMonthSelectorViewModel()
    {
        Items = new(
            Enumerable.Range(1, 31)
                .Select(
                    x => new DayOfMonthSelectItem
                    {
                        Day = (byte)x
                    }
                )
        );
    }

    public AvaloniaList<DayOfMonthSelectItem> Items { get; }
}
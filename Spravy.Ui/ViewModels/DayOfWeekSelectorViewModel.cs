using System;
using System.Linq;
using Avalonia.Collections;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class DayOfWeekSelectorViewModel : ViewModelBase
{
    public DayOfWeekSelectorViewModel()
    {
        Items = new(
            Enum.GetValues<DayOfWeek>()
                .Select(
                    x => new DayOfWeekSelectItem
                    {
                        DayOfWeek = x,
                    }
                )
        );
    }

    public AvaloniaList<DayOfWeekSelectItem> Items { get; }
}
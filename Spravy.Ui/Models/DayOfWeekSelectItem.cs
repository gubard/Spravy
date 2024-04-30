using System;
using ReactiveUI;

namespace Spravy.Ui.Models;

public class DayOfWeekSelectItem : NotifyBase
{
    private DayOfWeek dayOfWeek;
    private bool isSelected;

    public bool IsSelected
    {
        get => isSelected;
        set => this.RaiseAndSetIfChanged(ref isSelected, value);
    }

    public DayOfWeek DayOfWeek
    {
        get => dayOfWeek;
        set => this.RaiseAndSetIfChanged(ref dayOfWeek, value);
    }
}
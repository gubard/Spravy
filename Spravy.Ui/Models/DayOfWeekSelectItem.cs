using System;
using ReactiveUI;

namespace Spravy.Ui.Models;

public class DayOfWeekSelectItem : NotifyBase
{
    private bool isSelected;
    private DayOfWeek dayOfWeek;

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
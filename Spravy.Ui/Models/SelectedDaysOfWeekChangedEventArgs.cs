using System;

namespace Spravy.Ui.Models;

public readonly struct SelectedDaysOfWeekChangedEventArgs
{
    public SelectedDaysOfWeekChangedEventArgs(DayOfWeek[] newSelectedDaysOfWeek)
    {
        NewSelectedDaysOfWeek = newSelectedDaysOfWeek;
    }

    public DayOfWeek[] NewSelectedDaysOfWeek { get; }
}
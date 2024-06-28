namespace Spravy.Ui.Models;

public readonly struct SelectedSelectedDaysOfYearEventArgs
{
    public SelectedSelectedDaysOfYearEventArgs(DayOfYear[] newSelectedDaysOfYear)
    {
        NewSelectedDaysOfYear = newSelectedDaysOfYear;
    }

    public DayOfYear[] NewSelectedDaysOfYear { get; }
}

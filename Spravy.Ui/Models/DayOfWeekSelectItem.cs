namespace Spravy.Ui.Models;

public partial class DayOfWeekSelectItem : NotifyBase
{
    [ObservableProperty]
    private DayOfWeek dayOfWeek;

    [ObservableProperty]
    private bool isSelected;
}

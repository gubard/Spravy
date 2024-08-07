namespace Spravy.Ui.Models;

public partial class DayOfMonthSelectItem : NotifyBase
{
    [ObservableProperty]
    private byte day;

    [ObservableProperty]
    private bool isSelected;
}

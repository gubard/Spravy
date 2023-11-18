using ReactiveUI;

namespace Spravy.Ui.Models;

public class DayOfMonthSelectItem : NotifyBase
{
    private bool isSelected;
    private byte day;

    public bool IsSelected
    {
        get => isSelected;
        set => this.RaiseAndSetIfChanged(ref isSelected, value);
    }

    public byte Day
    {
        get => day;
        set => this.RaiseAndSetIfChanged(ref day, value);
    }
}
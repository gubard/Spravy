using ReactiveUI;

namespace Spravy.Ui.Models;

public class DayOfMonthSelectItem : NotifyBase
{
    private byte day;
    private bool isSelected;

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
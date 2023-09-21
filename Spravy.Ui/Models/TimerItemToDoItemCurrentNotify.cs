using ReactiveUI;

namespace Spravy.Ui.Models;

public class TimerItemToDoItemCurrentNotify : TimerItemNotify
{
    private bool isCurrent;

    public bool IsCurrent
    {
        get => isCurrent;
        set => this.RaiseAndSetIfChanged(ref isCurrent, value);
    }
}
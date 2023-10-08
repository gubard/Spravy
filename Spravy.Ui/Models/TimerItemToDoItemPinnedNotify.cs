using ReactiveUI;

namespace Spravy.Ui.Models;

public class TimerItemToDoItemPinnedNotify : TimerItemNotify
{
    private bool isPinned;

    public bool IsPinned
    {
        get => isPinned;
        set => this.RaiseAndSetIfChanged(ref isPinned, value);
    }
}
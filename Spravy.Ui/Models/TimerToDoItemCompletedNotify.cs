using ReactiveUI;

namespace Spravy.Ui.Models;

public class TimerToDoItemCompletedNotify : TimerNotify
{
    private bool isCompleted;

    public bool IsCompleted
    {
        get => isCompleted;
        set => this.RaiseAndSetIfChanged(ref isCompleted, value);
    }
}
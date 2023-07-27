using ReactiveUI;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Models;

public class ToDoSubItemValueNotify : ToDoSubItemNotify, IIsCompletedToDoItem
{
    private bool isCompleted;
    private uint completedCount;
    private uint skippedCount;
    private uint failedCount;

    public bool IsCompleted
    {
        get => isCompleted;
        set => this.RaiseAndSetIfChanged(ref isCompleted, value);
    }

    public uint CompletedCount
    {
        get => completedCount;
        set => this.RaiseAndSetIfChanged(ref completedCount, value);
    }

    public uint SkippedCount
    {
        get => skippedCount;
        set => this.RaiseAndSetIfChanged(ref skippedCount, value);
    }

    public uint FailedCount
    {
        get => failedCount;
        set => this.RaiseAndSetIfChanged(ref failedCount, value);
    }
}
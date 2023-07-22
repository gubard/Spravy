using System;
using ReactiveUI;

namespace Spravy.Ui.Models;

public class ToDoSubItemValueNotify : ToDoSubItemNotify
{
    private DateTimeOffset? dueDate;
    private bool isComplete;
    private uint completedCount;
    private uint skippedCount;
    private uint failedCount;

    public DateTimeOffset? DueDate
    {
        get => dueDate;
        set => this.RaiseAndSetIfChanged(ref dueDate, value);
    }

    public bool IsComplete
    {
        get => isComplete;
        set => this.RaiseAndSetIfChanged(ref isComplete, value);
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
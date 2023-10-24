using System;
using ReactiveUI;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Models;

public class ToDoSubItemPlannedNotify : ToDoSubItemNotify, IIsCompletedToDoItem
{
    private DateTimeOffset dueDate;
    private uint completedCount;
    private uint skippedCount;
    private uint failedCount;
    private bool isCompleted;
    private DateTimeOffset? lastCompleted;

    public DateTimeOffset? LastCompleted
    {
        get => lastCompleted;
        set => this.RaiseAndSetIfChanged(ref lastCompleted, value);
    }

    public DateTimeOffset DueDate
    {
        get => dueDate;
        set => this.RaiseAndSetIfChanged(ref dueDate, value);
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

    public bool IsCompleted
    {
        get => isCompleted;
        set => this.RaiseAndSetIfChanged(ref isCompleted, value);
    }
}
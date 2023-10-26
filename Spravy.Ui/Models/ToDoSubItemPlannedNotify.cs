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
    private double completedPercentage;
    private double skippedPercentage;
    private double failedPercentage;

    public ToDoSubItemPlannedNotify()
    {
        this.WhenAnyValue(x => x.FailedCount, x => x.SkippedCount, x => x.FailedCount)
            .Subscribe(
                _ =>
                {
                    var count = FailedCount + SkippedCount + CompletedCount + 0.0;
                    CompletedPercentage = CompletedCount * 200.0 / count;
                    SkippedPercentage = SkippedCount * 200.0 / count;
                    FailedPercentage = FailedCount * 200.0 / count;
                });
    }

    public double CompletedPercentage
    {
        get => completedPercentage;
        set => this.RaiseAndSetIfChanged(ref completedPercentage, value);
    }

    public double SkippedPercentage
    {
        get => skippedPercentage;
        set => this.RaiseAndSetIfChanged(ref skippedPercentage, value);
    }

    public double FailedPercentage
    {
        get => failedPercentage;
        set => this.RaiseAndSetIfChanged(ref failedPercentage, value);
    }

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
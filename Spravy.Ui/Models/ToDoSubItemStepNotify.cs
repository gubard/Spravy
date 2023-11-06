using System;
using ReactiveUI;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Models;

public class ToDoSubItemStepNotify : ToDoSubItemNotify, IIsCompletedToDoItem
{
    private bool isCompleted;
    private uint completedCount;
    private uint skippedCount;
    private uint failedCount;
    private DateTimeOffset? lastCompleted;
    private double completedPercentage;
    private double skippedPercentage;
    private double failedPercentage;

    public ToDoSubItemStepNotify()
    {
        this.WhenAnyValue(x => x.FailedCount, x => x.SkippedCount, x => x.FailedCount)
            .Subscribe(
                _ =>
                {
                    var count = FailedCount + SkippedCount + CompletedCount + 0.0;

                    if (count == 0)
                    {
                        CompletedPercentage = 0;
                        SkippedPercentage = 0;
                        FailedPercentage = 0;
                    }
                    else
                    {
                        CompletedPercentage = CompletedCount * 200.0 / count;
                        SkippedPercentage = SkippedCount * 200.0 / count;
                        FailedPercentage = FailedCount * 200.0 / count;
                    }
                }
            );
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
using System;
using ReactiveUI;

namespace Spravy.Ui.Models;

public class ToDoSubItemPeriodicityOffsetNotify : ToDoSubItemNotify
{
    private DateTimeOffset dueDate;
    private uint completedCount;
    private uint skippedCount;
    private uint failedCount;
    private ushort daysOffset;
    private ushort monthsOffset;
    private ushort weeksOffset;
    private ushort yearsOffset;
    private DateTimeOffset? lastCompleted;

    public DateTimeOffset? LastCompleted
    {
        get => lastCompleted;
        set => this.RaiseAndSetIfChanged(ref lastCompleted, value);
    }

    public ushort DaysOffset
    {
        get => daysOffset;
        set => this.RaiseAndSetIfChanged(ref daysOffset, value);
    }

    public ushort MonthsOffset
    {
        get => monthsOffset;
        set => this.RaiseAndSetIfChanged(ref monthsOffset, value);
    }

    public ushort WeeksOffset
    {
        get => weeksOffset;
        set => this.RaiseAndSetIfChanged(ref weeksOffset, value);
    }

    public ushort YearsOffset
    {
        get => yearsOffset;
        set => this.RaiseAndSetIfChanged(ref yearsOffset, value);
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
}
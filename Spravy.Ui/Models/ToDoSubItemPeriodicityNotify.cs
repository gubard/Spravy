using System;
using ReactiveUI;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.Ui.Models;

public class ToDoSubItemPeriodicityNotify : ToDoSubItemNotify
{
    private DateTimeOffset dueDate;
    private uint completedCount;
    private uint skippedCount;
    private uint failedCount;
    private TypeOfPeriodicity typeOfPeriodicity;
    private DateTimeOffset? lastCompleted;

    public DateTimeOffset? LastCompleted
    {
        get => lastCompleted;
        set => this.RaiseAndSetIfChanged(ref lastCompleted, value);
    }

    public TypeOfPeriodicity TypeOfPeriodicity
    {
        get => typeOfPeriodicity;
        set => this.RaiseAndSetIfChanged(ref typeOfPeriodicity, value);
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
using System;
using ReactiveUI;

namespace Spravy.Ui.Models;

public abstract class TimerNotify : NotifyBase
{
    private Guid id;
    private DateTimeOffset dueDateTime;

    public Guid Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    public DateTimeOffset DueDateTime
    {
        get => dueDateTime;
        set => this.RaiseAndSetIfChanged(ref dueDateTime, value);
    }
}
using System;
using ReactiveUI;

namespace Spravy.Ui.Models;

public abstract class TimerItemNotify : NotifyBase
{
    private DateTimeOffset dueDateTime;
    private Guid id;

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
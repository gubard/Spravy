using System;
using ReactiveUI;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class AddTimerViewModel : ViewModelBase
{
    private DateTimeOffset dueDateTime;
    private bool isCompleted;
    
    public Guid EventId { get; set; }

    public DateTimeOffset DueDateTime
    {
        get => dueDateTime;
        set => this.RaiseAndSetIfChanged(ref dueDateTime, value);
    }

    public bool IsCompleted
    {
        get => isCompleted;
        set => this.RaiseAndSetIfChanged(ref isCompleted, value);
    }
}
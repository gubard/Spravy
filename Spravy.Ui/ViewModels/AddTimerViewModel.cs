using System;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class AddTimerViewModel : ViewModelBase, IToDoShortItemProperty, IDueDateTimeProperty
{
    private DateTimeOffset dueDateTime = DateTimeOffset.Now.ToCurrentDay();
    private bool isFavorite;
    private ToDoShortItemNotify? item;

    public Guid EventId { get; set; }

    public ToDoShortItemNotify? ShortItem
    {
        get => item;
        set => this.RaiseAndSetIfChanged(ref item, value);
    }

    public DateTimeOffset DueDateTime
    {
        get => dueDateTime;
        set => this.RaiseAndSetIfChanged(ref dueDateTime, value);
    }

    public bool IsFavorite
    {
        get => isFavorite;
        set => this.RaiseAndSetIfChanged(ref isFavorite, value);
    }
}
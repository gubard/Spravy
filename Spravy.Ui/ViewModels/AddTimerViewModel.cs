using System;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class AddTimerViewModel : ViewModelBase, IToDoShortItemProperty, IDueDateTimeProperty
{
    [Reactive]
    public Guid EventId { get; set; }

    [Reactive]
    public bool IsFavorite { get; set; }

    [Reactive]
    public DateTimeOffset DueDateTime { get; set; } = DateTimeOffset.Now.ToCurrentDay();

    [Reactive]
    public ToDoShortItemNotify? ShortItem { get; set; }
}
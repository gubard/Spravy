namespace Spravy.Ui.Features.Timer.ViewModels;

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
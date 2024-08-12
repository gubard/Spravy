namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsGroupByStatusViewModel : ViewModelBase
{
    public ToDoItemsGroupByStatusViewModel(
        ToDoItemsViewModel missed,
        ToDoItemsViewModel readyForCompleted,
        ToDoItemsViewModel planned,
        ToDoItemsViewModel completed
    )
    {
        missed.Header = new("ToDoItemsGroupByStatusView.Missed");
        Missed = missed;
        readyForCompleted.Header = new("ToDoItemsGroupByStatusView.ReadyForCompleted");
        ReadyForCompleted = readyForCompleted;
        planned.Header = new("ToDoItemsGroupByStatusView.Planned");
        Planned = planned;
        completed.Header = new("ToDoItemsGroupByStatusView.Completed");
        Completed = completed;
    }

    public ToDoItemsViewModel Missed { get; }
    public ToDoItemsViewModel ReadyForCompleted { get; }
    public ToDoItemsViewModel Planned { get; }
    public ToDoItemsViewModel Completed { get; }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return Missed
            .ClearExceptUi(items.Where(x => x.Status == ToDoItemStatus.Miss))
            .IfSuccess(
                () =>
                    ReadyForCompleted.ClearExceptUi(
                        items.Where(x => x.Status == ToDoItemStatus.ReadyForComplete)
                    )
            )
            .IfSuccess(
                () => Planned.ClearExceptUi(items.Where(x => x.Status == ToDoItemStatus.Planned))
            )
            .IfSuccess(
                () =>
                    Completed.ClearExceptUi(items.Where(x => x.Status == ToDoItemStatus.Completed))
            );
    }

    public Result AddOrUpdateUi(ToDoItemEntityNotify item)
    {
        var result = item.Status switch
        {
            ToDoItemStatus.Miss
                => Missed
                    .AddOrUpdateUi(item)
                    .IfSuccess(() => ReadyForCompleted.RemoveItemUi(item))
                    .IfSuccess(() => Planned.RemoveItemUi(item))
                    .IfSuccess(() => Completed.RemoveItemUi(item)),
            ToDoItemStatus.ReadyForComplete
                => Missed
                    .RemoveItemUi(item)
                    .IfSuccess(() => ReadyForCompleted.AddOrUpdateUi(item))
                    .IfSuccess(() => Planned.RemoveItemUi(item))
                    .IfSuccess(() => Completed.RemoveItemUi(item)),
            ToDoItemStatus.Planned
                => Missed
                    .RemoveItemUi(item)
                    .IfSuccess(() => ReadyForCompleted.RemoveItemUi(item))
                    .IfSuccess(() => Planned.AddOrUpdateUi(item))
                    .IfSuccess(() => Completed.RemoveItemUi(item)),
            ToDoItemStatus.Completed
                => Missed
                    .RemoveItemUi(item)
                    .IfSuccess(() => ReadyForCompleted.RemoveItemUi(item))
                    .IfSuccess(() => Planned.RemoveItemUi(item))
                    .IfSuccess(() => Completed.AddOrUpdateUi(item)),
            _ => new(new ToDoItemStatusOutOfRangeError(item.Status))
        };

        return result;
    }
}

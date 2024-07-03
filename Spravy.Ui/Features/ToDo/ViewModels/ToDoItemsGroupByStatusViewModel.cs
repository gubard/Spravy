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

        this.WhenAnyValue(x => x.IsMulti)
            .Subscribe(x =>
            {
                Missed.IsMulti = x;
                ReadyForCompleted.IsMulti = x;
                Planned.IsMulti = x;
                Completed.IsMulti = x;
            });
    }

    public ToDoItemsViewModel Missed { get; }
    public ToDoItemsViewModel ReadyForCompleted { get; }
    public ToDoItemsViewModel Planned { get; }
    public ToDoItemsViewModel Completed { get; }

    [Reactive]
    public bool IsMulti { get; set; }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return Missed
            .ClearExceptUi(items.ToArray().Where(x => x.Status == ToDoItemStatus.Miss).ToArray())
            .IfSuccess(
                () =>
                    ReadyForCompleted.ClearExceptUi(
                        items
                            .ToArray()
                            .Where(x => x.Status == ToDoItemStatus.ReadyForComplete)
                            .ToArray()
                    )
            )
            .IfSuccess(
                () =>
                    Planned.ClearExceptUi(
                        items.ToArray().Where(x => x.Status == ToDoItemStatus.Planned).ToArray()
                    )
            )
            .IfSuccess(
                () =>
                    Completed.ClearExceptUi(
                        items.ToArray().Where(x => x.Status == ToDoItemStatus.Completed).ToArray()
                    )
            );
    }

    public Result UpdateItemUi(ToDoItemEntityNotify item)
    {
        var result = item.Status switch
        {
            ToDoItemStatus.Miss
                => Missed
                    .UpdateItemUi(item)
                    .IfSuccess(() => ReadyForCompleted.RemoveItemUi(item))
                    .IfSuccess(() => Planned.RemoveItemUi(item))
                    .IfSuccess(() => Completed.RemoveItemUi(item)),
            ToDoItemStatus.ReadyForComplete
                => Missed
                    .RemoveItemUi(item)
                    .IfSuccess(() => ReadyForCompleted.UpdateItemUi(item))
                    .IfSuccess(() => Planned.RemoveItemUi(item))
                    .IfSuccess(() => Completed.RemoveItemUi(item)),
            ToDoItemStatus.Planned
                => Missed
                    .RemoveItemUi(item)
                    .IfSuccess(() => ReadyForCompleted.RemoveItemUi(item))
                    .IfSuccess(() => Planned.UpdateItemUi(item))
                    .IfSuccess(() => Completed.RemoveItemUi(item)),
            ToDoItemStatus.Completed
                => Missed
                    .RemoveItemUi(item)
                    .IfSuccess(() => ReadyForCompleted.RemoveItemUi(item))
                    .IfSuccess(() => Planned.RemoveItemUi(item))
                    .IfSuccess(() => Completed.UpdateItemUi(item)),
            _ => new(new ToDoItemStatusOutOfRangeError(item.Status))
        };

        return result;
    }
}

namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class MultiToDoItemsViewModel : ViewModelBase
{
    [ObservableProperty]
    private GroupBy groupBy;

    [ObservableProperty]
    private bool isMulti;

    public MultiToDoItemsViewModel(
        ToDoItemsViewModel favorite,
        ToDoItemsViewModel items,
        ToDoItemsViewModel missed,
        ToDoItemsViewModel readyForCompleted,
        ToDoItemsViewModel planned,
        ToDoItemsViewModel completed,
        ToDoItemsViewModel values,
        ToDoItemsViewModel groups,
        ToDoItemsViewModel planneds,
        ToDoItemsViewModel periodicitys,
        ToDoItemsViewModel periodicityOffsets,
        ToDoItemsViewModel circles,
        ToDoItemsViewModel steps,
        ToDoItemsViewModel references
    )
    {
        GroupBy = GroupBy.ByStatus;
        Favorite = favorite;
        Items = items;
        Missed = missed;
        ReadyForCompleted = readyForCompleted;
        Planned = planned;
        Completed = completed;
        Values = values;
        Groups = groups;
        Planneds = planneds;
        Periodicitys = periodicitys;
        PeriodicityOffsets = periodicityOffsets;
        Circles = circles;
        Steps = steps;
        References = references;
    }

    public ToDoItemsViewModel Favorite { get; }

    public ToDoItemsViewModel Items { get; }

    public ToDoItemsViewModel Missed { get; }
    public ToDoItemsViewModel ReadyForCompleted { get; }
    public ToDoItemsViewModel Planned { get; }
    public ToDoItemsViewModel Completed { get; }

    public ToDoItemsViewModel Values { get; }
    public ToDoItemsViewModel Groups { get; }
    public ToDoItemsViewModel Planneds { get; }
    public ToDoItemsViewModel Periodicitys { get; }
    public ToDoItemsViewModel PeriodicityOffsets { get; }
    public ToDoItemsViewModel Circles { get; }
    public ToDoItemsViewModel Steps { get; }
    public ToDoItemsViewModel References { get; }

    public Result RemoveUi(ToDoItemEntityNotify item)
    {
        return Items
            .RemoveUi(item)
            .IfSuccess(() => Missed.RemoveUi(item))
            .IfSuccess(() => ReadyForCompleted.RemoveUi(item))
            .IfSuccess(() => Planned.RemoveUi(item))
            .IfSuccess(() => Completed.RemoveUi(item))
            .IfSuccess(() => Values.RemoveUi(item))
            .IfSuccess(() => Groups.RemoveUi(item))
            .IfSuccess(() => Planneds.RemoveUi(item))
            .IfSuccess(() => Periodicitys.RemoveUi(item))
            .IfSuccess(() => PeriodicityOffsets.RemoveUi(item))
            .IfSuccess(() => Circles.RemoveUi(item))
            .IfSuccess(() => Steps.RemoveUi(item))
            .IfSuccess(() => References.RemoveUi(item));
    }

    public Result ClearFavoriteExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        return Favorite.ClearExceptUi(ids);
    }

    public Result UpdateFavoriteItemUi(ToDoItemEntityNotify item)
    {
        return Favorite.AddOrUpdateUi(item);
    }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        return Items
            .ClearExceptUi(ids)
            .IfSuccess(() => Missed.ClearExceptUi(ids.Where(x => x.Status == ToDoItemStatus.Miss)))
            .IfSuccess(
                () =>
                    ReadyForCompleted.ClearExceptUi(
                        ids.Where(x => x.Status == ToDoItemStatus.ReadyForComplete)
                    )
            )
            .IfSuccess(
                () => Planned.ClearExceptUi(ids.Where(x => x.Status == ToDoItemStatus.Planned))
            )
            .IfSuccess(
                () => Completed.ClearExceptUi(ids.Where(x => x.Status == ToDoItemStatus.Completed))
            )
            .IfSuccess(() => Values.ClearExceptUi(ids.Where(x => x.Type == ToDoItemType.Value)))
            .IfSuccess(() => Groups.ClearExceptUi(ids.Where(x => x.Type == ToDoItemType.Group)))
            .IfSuccess(() => Planneds.ClearExceptUi(ids.Where(x => x.Type == ToDoItemType.Planned)))
            .IfSuccess(
                () => Periodicitys.ClearExceptUi(ids.Where(x => x.Type == ToDoItemType.Periodicity))
            )
            .IfSuccess(
                () =>
                    PeriodicityOffsets.ClearExceptUi(
                        ids.Where(x => x.Type == ToDoItemType.PeriodicityOffset)
                    )
            )
            .IfSuccess(() => Circles.ClearExceptUi(ids.Where(x => x.Type == ToDoItemType.Circle)))
            .IfSuccess(() => Steps.ClearExceptUi(ids.Where(x => x.Type == ToDoItemType.Step)))
            .IfSuccess(
                () => References.ClearExceptUi(ids.Where(x => x.Type == ToDoItemType.Reference))
            );
    }

    public Result AddOrUpdateUi(ToDoItemEntityNotify item)
    {
        return Items
            .AddOrUpdateUi(item)
            .IfSuccess(
                () =>
                    item.Status switch
                    {
                        ToDoItemStatus.Miss
                            => Missed
                                .AddOrUpdateUi(item)
                                .IfSuccess(() => ReadyForCompleted.RemoveUi(item))
                                .IfSuccess(() => Planned.RemoveUi(item))
                                .IfSuccess(() => Completed.RemoveUi(item)),
                        ToDoItemStatus.ReadyForComplete
                            => Missed
                                .RemoveUi(item)
                                .IfSuccess(() => ReadyForCompleted.AddOrUpdateUi(item))
                                .IfSuccess(() => Planned.RemoveUi(item))
                                .IfSuccess(() => Completed.RemoveUi(item)),
                        ToDoItemStatus.Planned
                            => Missed
                                .RemoveUi(item)
                                .IfSuccess(() => ReadyForCompleted.RemoveUi(item))
                                .IfSuccess(() => Planned.AddOrUpdateUi(item))
                                .IfSuccess(() => Completed.RemoveUi(item)),
                        ToDoItemStatus.Completed
                            => Missed
                                .RemoveUi(item)
                                .IfSuccess(() => ReadyForCompleted.RemoveUi(item))
                                .IfSuccess(() => Planned.RemoveUi(item))
                                .IfSuccess(() => Completed.AddOrUpdateUi(item)),
                        _ => new(new ToDoItemStatusOutOfRangeError(item.Status)),
                    }
            )
            .IfSuccess(
                () =>
                    item.Type switch
                    {
                        ToDoItemType.Value
                            => Values
                                .AddOrUpdateUi(item)
                                .IfSuccess(() => Groups.RemoveUi(item))
                                .IfSuccess(() => Planneds.RemoveUi(item))
                                .IfSuccess(() => Periodicitys.RemoveUi(item))
                                .IfSuccess(() => PeriodicityOffsets.RemoveUi(item))
                                .IfSuccess(() => Circles.RemoveUi(item))
                                .IfSuccess(() => Steps.RemoveUi(item))
                                .IfSuccess(() => References.RemoveUi(item)),
                        ToDoItemType.Group
                            => Values
                                .RemoveUi(item)
                                .IfSuccess(() => Groups.AddOrUpdateUi(item))
                                .IfSuccess(() => Planneds.RemoveUi(item))
                                .IfSuccess(() => Periodicitys.RemoveUi(item))
                                .IfSuccess(() => PeriodicityOffsets.RemoveUi(item))
                                .IfSuccess(() => Circles.RemoveUi(item))
                                .IfSuccess(() => Steps.RemoveUi(item))
                                .IfSuccess(() => References.RemoveUi(item)),
                        ToDoItemType.Planned
                            => Values
                                .RemoveUi(item)
                                .IfSuccess(() => Groups.RemoveUi(item))
                                .IfSuccess(() => Planneds.AddOrUpdateUi(item))
                                .IfSuccess(() => Periodicitys.RemoveUi(item))
                                .IfSuccess(() => PeriodicityOffsets.RemoveUi(item))
                                .IfSuccess(() => Circles.RemoveUi(item))
                                .IfSuccess(() => Steps.RemoveUi(item))
                                .IfSuccess(() => References.RemoveUi(item)),
                        ToDoItemType.Periodicity
                            => Values
                                .RemoveUi(item)
                                .IfSuccess(() => Groups.RemoveUi(item))
                                .IfSuccess(() => Planneds.RemoveUi(item))
                                .IfSuccess(() => Periodicitys.AddOrUpdateUi(item))
                                .IfSuccess(() => PeriodicityOffsets.RemoveUi(item))
                                .IfSuccess(() => Circles.RemoveUi(item))
                                .IfSuccess(() => Steps.RemoveUi(item))
                                .IfSuccess(() => References.RemoveUi(item)),
                        ToDoItemType.PeriodicityOffset
                            => Values
                                .RemoveUi(item)
                                .IfSuccess(() => Groups.RemoveUi(item))
                                .IfSuccess(() => Planneds.RemoveUi(item))
                                .IfSuccess(() => Periodicitys.RemoveUi(item))
                                .IfSuccess(() => PeriodicityOffsets.AddOrUpdateUi(item))
                                .IfSuccess(() => Circles.RemoveUi(item))
                                .IfSuccess(() => Steps.RemoveUi(item))
                                .IfSuccess(() => References.RemoveUi(item)),
                        ToDoItemType.Circle
                            => Values
                                .RemoveUi(item)
                                .IfSuccess(() => Groups.RemoveUi(item))
                                .IfSuccess(() => Planneds.RemoveUi(item))
                                .IfSuccess(() => Periodicitys.RemoveUi(item))
                                .IfSuccess(() => PeriodicityOffsets.RemoveUi(item))
                                .IfSuccess(() => Circles.AddOrUpdateUi(item))
                                .IfSuccess(() => Steps.RemoveUi(item))
                                .IfSuccess(() => References.RemoveUi(item)),
                        ToDoItemType.Step
                            => Values
                                .RemoveUi(item)
                                .IfSuccess(() => Groups.RemoveUi(item))
                                .IfSuccess(() => Planneds.RemoveUi(item))
                                .IfSuccess(() => Periodicitys.RemoveUi(item))
                                .IfSuccess(() => PeriodicityOffsets.RemoveUi(item))
                                .IfSuccess(() => Circles.RemoveUi(item))
                                .IfSuccess(() => Steps.AddOrUpdateUi(item))
                                .IfSuccess(() => References.RemoveUi(item)),
                        ToDoItemType.Reference
                            => Values
                                .RemoveUi(item)
                                .IfSuccess(() => Groups.RemoveUi(item))
                                .IfSuccess(() => Planneds.RemoveUi(item))
                                .IfSuccess(() => Periodicitys.RemoveUi(item))
                                .IfSuccess(() => PeriodicityOffsets.RemoveUi(item))
                                .IfSuccess(() => Circles.RemoveUi(item))
                                .IfSuccess(() => Steps.RemoveUi(item))
                                .IfSuccess(() => References.AddOrUpdateUi(item)),
                        _ => new(new ToDoItemTypeOutOfRangeError(item.Type)),
                    }
            );
    }
}

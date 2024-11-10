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
        ToDoItemsViewModel references,
        ToDoItemsViewModel comingSoon
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
        ComingSoon = comingSoon;
    }

    public ToDoItemsViewModel Favorite { get; }

    public ToDoItemsViewModel Items { get; }

    public ToDoItemsViewModel Missed { get; }
    public ToDoItemsViewModel ReadyForCompleted { get; }
    public ToDoItemsViewModel ComingSoon { get; }
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

    public Result RemoveUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return Items
            .RemoveUi(items)
            .IfSuccess(() => Missed.RemoveUi(items))
            .IfSuccess(() => ReadyForCompleted.RemoveUi(items))
            .IfSuccess(() => Planned.RemoveUi(items))
            .IfSuccess(() => Completed.RemoveUi(items))
            .IfSuccess(() => Values.RemoveUi(items))
            .IfSuccess(() => Groups.RemoveUi(items))
            .IfSuccess(() => Planneds.RemoveUi(items))
            .IfSuccess(() => Periodicitys.RemoveUi(items))
            .IfSuccess(() => PeriodicityOffsets.RemoveUi(items))
            .IfSuccess(() => Circles.RemoveUi(items))
            .IfSuccess(() => Steps.RemoveUi(items))
            .IfSuccess(() => References.RemoveUi(items));
    }

    public Result ClearFavoriteExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return Favorite.ClearExceptUi(items);
    }

    public Result UpdateFavoriteItemUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return Favorite.AddOrUpdateUi(items);
    }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return Items
            .ClearExceptUi(items)
            .IfSuccess(
                () => Missed.ClearExceptUi(items.Where(x => x.Status == ToDoItemStatus.Miss))
            )
            .IfSuccess(
                () =>
                    ReadyForCompleted.ClearExceptUi(
                        items.Where(x => x.Status == ToDoItemStatus.ReadyForComplete)
                    )
            )
            .IfSuccess(
                () =>
                    ComingSoon.ClearExceptUi(
                        items.Where(x => x.Status == ToDoItemStatus.ComingSoon)
                    )
            )
            .IfSuccess(
                () => Planned.ClearExceptUi(items.Where(x => x.Status == ToDoItemStatus.Planned))
            )
            .IfSuccess(
                () =>
                    Completed.ClearExceptUi(items.Where(x => x.Status == ToDoItemStatus.Completed))
            )
            .IfSuccess(() => Values.ClearExceptUi(items.Where(x => x.Type == ToDoItemType.Value)))
            .IfSuccess(() => Groups.ClearExceptUi(items.Where(x => x.Type == ToDoItemType.Group)))
            .IfSuccess(
                () => Planneds.ClearExceptUi(items.Where(x => x.Type == ToDoItemType.Planned))
            )
            .IfSuccess(
                () =>
                    Periodicitys.ClearExceptUi(items.Where(x => x.Type == ToDoItemType.Periodicity))
            )
            .IfSuccess(
                () =>
                    PeriodicityOffsets.ClearExceptUi(
                        items.Where(x => x.Type == ToDoItemType.PeriodicityOffset)
                    )
            )
            .IfSuccess(() => Circles.ClearExceptUi(items.Where(x => x.Type == ToDoItemType.Circle)))
            .IfSuccess(() => Steps.ClearExceptUi(items.Where(x => x.Type == ToDoItemType.Step)))
            .IfSuccess(
                () => References.ClearExceptUi(items.Where(x => x.Type == ToDoItemType.Reference))
            );
    }

    public Result AddOrUpdateUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return Items
            .AddOrUpdateUi(items)
            .IfSuccess(
                () =>
                    Missed
                        .AddOrUpdateUi(items.Where(x => x.Status == ToDoItemStatus.Miss))
                        .IfSuccess(
                            () => Missed.RemoveUi(items.Where(x => x.Status != ToDoItemStatus.Miss))
                        )
            )
            .IfSuccess(
                () =>
                    ReadyForCompleted
                        .AddOrUpdateUi(
                            items.Where(x => x.Status == ToDoItemStatus.ReadyForComplete)
                        )
                        .IfSuccess(
                            () =>
                                ReadyForCompleted.RemoveUi(
                                    items.Where(x => x.Status != ToDoItemStatus.ReadyForComplete)
                                )
                        )
            )
            .IfSuccess(
                () =>
                    ComingSoon
                        .AddOrUpdateUi(items.Where(x => x.Status == ToDoItemStatus.ComingSoon))
                        .IfSuccess(
                            () =>
                                ComingSoon.RemoveUi(
                                    items.Where(x => x.Status != ToDoItemStatus.ComingSoon)
                                )
                        )
            )
            .IfSuccess(
                () =>
                    Planned
                        .AddOrUpdateUi(items.Where(x => x.Status == ToDoItemStatus.Planned))
                        .IfSuccess(
                            () =>
                                Planned.RemoveUi(
                                    items.Where(x => x.Status != ToDoItemStatus.Planned)
                                )
                        )
            )
            .IfSuccess(
                () =>
                    Completed
                        .AddOrUpdateUi(items.Where(x => x.Status == ToDoItemStatus.Completed))
                        .IfSuccess(
                            () =>
                                Completed.RemoveUi(
                                    items.Where(x => x.Status != ToDoItemStatus.Completed)
                                )
                        )
            )
            .IfSuccess(
                () =>
                    Values
                        .AddOrUpdateUi(items.Where(x => x.Type == ToDoItemType.Value))
                        .IfSuccess(
                            () => Values.RemoveUi(items.Where(x => x.Type != ToDoItemType.Value))
                        )
            )
            .IfSuccess(
                () =>
                    Groups
                        .AddOrUpdateUi(items.Where(x => x.Type == ToDoItemType.Group))
                        .IfSuccess(
                            () => Groups.RemoveUi(items.Where(x => x.Type != ToDoItemType.Group))
                        )
            )
            .IfSuccess(
                () =>
                    Planneds
                        .AddOrUpdateUi(items.Where(x => x.Type == ToDoItemType.Planned))
                        .IfSuccess(
                            () =>
                                Planneds.RemoveUi(items.Where(x => x.Type != ToDoItemType.Planned))
                        )
            )
            .IfSuccess(
                () =>
                    Periodicitys
                        .AddOrUpdateUi(items.Where(x => x.Type == ToDoItemType.Periodicity))
                        .IfSuccess(
                            () =>
                                Periodicitys.RemoveUi(
                                    items.Where(x => x.Type != ToDoItemType.Periodicity)
                                )
                        )
            )
            .IfSuccess(
                () =>
                    PeriodicityOffsets
                        .AddOrUpdateUi(items.Where(x => x.Type == ToDoItemType.PeriodicityOffset))
                        .IfSuccess(
                            () =>
                                PeriodicityOffsets.RemoveUi(
                                    items.Where(x => x.Type != ToDoItemType.PeriodicityOffset)
                                )
                        )
            )
            .IfSuccess(
                () =>
                    Circles
                        .AddOrUpdateUi(items.Where(x => x.Type == ToDoItemType.Circle))
                        .IfSuccess(
                            () => Circles.RemoveUi(items.Where(x => x.Type != ToDoItemType.Circle))
                        )
            )
            .IfSuccess(
                () =>
                    Steps
                        .AddOrUpdateUi(items.Where(x => x.Type == ToDoItemType.Step))
                        .IfSuccess(
                            () => Steps.RemoveUi(items.Where(x => x.Type != ToDoItemType.Step))
                        )
            )
            .IfSuccess(
                () =>
                    References
                        .AddOrUpdateUi(items.Where(x => x.Type == ToDoItemType.Reference))
                        .IfSuccess(
                            () =>
                                References.RemoveUi(
                                    items.Where(x => x.Type != ToDoItemType.Reference)
                                )
                        )
            );
    }
}

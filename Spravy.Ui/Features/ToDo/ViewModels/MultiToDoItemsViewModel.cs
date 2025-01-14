namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class MultiToDoItemsViewModel : ViewModelBase, IRefresh
{
    [ObservableProperty]
    private GroupBy groupBy;

    [ObservableProperty]
    private bool isMulti;

    [ObservableProperty]
    private SortByToDoItem sortBy;

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

    public Result SetFavoriteItemsUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return Favorite.SetItemsUi(items);
    }

    public Result SetItemsUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return Items.SetItemsUi(items)
           .IfSuccess(() => Missed.SetItemsUi(items.Where(x => x.Status == ToDoItemStatus.Miss)))
           .IfSuccess(() => ReadyForCompleted.SetItemsUi(items.Where(x => x.Status == ToDoItemStatus.ReadyForComplete)))
           .IfSuccess(() => ComingSoon.SetItemsUi(items.Where(x => x.Status == ToDoItemStatus.ComingSoon)))
           .IfSuccess(() => Planned.SetItemsUi(items.Where(x => x.Status == ToDoItemStatus.Planned)))
           .IfSuccess(() => Completed.SetItemsUi(items.Where(x => x.Status == ToDoItemStatus.Completed)))
           .IfSuccess(() => Values.SetItemsUi(items.Where(x => x.Type == ToDoItemType.Value)))
           .IfSuccess(() => Groups.SetItemsUi(items.Where(x => x.Type == ToDoItemType.Group)))
           .IfSuccess(() => Planneds.SetItemsUi(items.Where(x => x.Type == ToDoItemType.Planned)))
           .IfSuccess(() => Periodicitys.SetItemsUi(items.Where(x => x.Type == ToDoItemType.Periodicity)))
           .IfSuccess(() => PeriodicityOffsets.SetItemsUi(items.Where(x => x.Type == ToDoItemType.PeriodicityOffset)))
           .IfSuccess(() => Circles.SetItemsUi(items.Where(x => x.Type == ToDoItemType.Circle)))
           .IfSuccess(() => Steps.SetItemsUi(items.Where(x => x.Type == ToDoItemType.Step)))
           .IfSuccess(() => References.SetItemsUi(items.Where(x => x.Type == ToDoItemType.Reference)));
    }

    public Result AddOrUpdateUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return Items.AddOrUpdateUi(items)
           .IfSuccess(
                () =>
                    Missed.AddOrUpdateUi(items.Where(x => x.Status == ToDoItemStatus.Miss))
                       .IfSuccess(() => Missed.RemoveUi(items.Where(x => x.Status != ToDoItemStatus.Miss)))
            )
           .IfSuccess(
                () =>
                    ReadyForCompleted.AddOrUpdateUi(items.Where(x => x.Status == ToDoItemStatus.ReadyForComplete))
                       .IfSuccess(
                            () =>
                                ReadyForCompleted.RemoveUi(
                                    items.Where(x => x.Status != ToDoItemStatus.ReadyForComplete)
                                )
                        )
            )
           .IfSuccess(
                () =>
                    ComingSoon.AddOrUpdateUi(items.Where(x => x.Status == ToDoItemStatus.ComingSoon))
                       .IfSuccess(() => ComingSoon.RemoveUi(items.Where(x => x.Status != ToDoItemStatus.ComingSoon)))
            )
           .IfSuccess(
                () =>
                    Planned.AddOrUpdateUi(items.Where(x => x.Status == ToDoItemStatus.Planned))
                       .IfSuccess(() => Planned.RemoveUi(items.Where(x => x.Status != ToDoItemStatus.Planned)))
            )
           .IfSuccess(
                () =>
                    Completed.AddOrUpdateUi(items.Where(x => x.Status == ToDoItemStatus.Completed))
                       .IfSuccess(() => Completed.RemoveUi(items.Where(x => x.Status != ToDoItemStatus.Completed)))
            )
           .IfSuccess(
                () =>
                    Values.AddOrUpdateUi(items.Where(x => x.Type == ToDoItemType.Value))
                       .IfSuccess(() => Values.RemoveUi(items.Where(x => x.Type != ToDoItemType.Value)))
            )
           .IfSuccess(
                () =>
                    Groups.AddOrUpdateUi(items.Where(x => x.Type == ToDoItemType.Group))
                       .IfSuccess(() => Groups.RemoveUi(items.Where(x => x.Type != ToDoItemType.Group)))
            )
           .IfSuccess(
                () =>
                    Planneds.AddOrUpdateUi(items.Where(x => x.Type == ToDoItemType.Planned))
                       .IfSuccess(() => Planneds.RemoveUi(items.Where(x => x.Type != ToDoItemType.Planned)))
            )
           .IfSuccess(
                () =>
                    Periodicitys.AddOrUpdateUi(items.Where(x => x.Type == ToDoItemType.Periodicity))
                       .IfSuccess(() => Periodicitys.RemoveUi(items.Where(x => x.Type != ToDoItemType.Periodicity)))
            )
           .IfSuccess(
                () =>
                    PeriodicityOffsets.AddOrUpdateUi(items.Where(x => x.Type == ToDoItemType.PeriodicityOffset))
                       .IfSuccess(
                            () =>
                                PeriodicityOffsets.RemoveUi(items.Where(x => x.Type != ToDoItemType.PeriodicityOffset))
                        )
            )
           .IfSuccess(
                () =>
                    Circles.AddOrUpdateUi(items.Where(x => x.Type == ToDoItemType.Circle))
                       .IfSuccess(() => Circles.RemoveUi(items.Where(x => x.Type != ToDoItemType.Circle)))
            )
           .IfSuccess(
                () => Steps.AddOrUpdateUi(items.Where(x => x.Type == ToDoItemType.Step))
                   .IfSuccess(() => Steps.RemoveUi(items.Where(x => x.Type != ToDoItemType.Step)))
            )
           .IfSuccess(
                () => References.AddOrUpdateUi(items.Where(x => x.Type == ToDoItemType.Reference))
                   .IfSuccess(() => References.RemoveUi(items.Where(x => x.Type != ToDoItemType.Reference)))
            );
    }

    public Cvtar RefreshAsync(CancellationToken ct)
    {
        return Items.RefreshAsync(ct)
           .IfSuccessAsync(() => Missed.RefreshAsync(ct), ct)
           .IfSuccessAsync(() => ReadyForCompleted.RefreshAsync(ct), ct)
           .IfSuccessAsync(() => Planned.RefreshAsync(ct), ct)
           .IfSuccessAsync(() => Completed.RefreshAsync(ct), ct)
           .IfSuccessAsync(() => Values.RefreshAsync(ct), ct)
           .IfSuccessAsync(() => Groups.RefreshAsync(ct), ct)
           .IfSuccessAsync(() => Planneds.RefreshAsync(ct), ct)
           .IfSuccessAsync(() => Periodicitys.RefreshAsync(ct), ct)
           .IfSuccessAsync(() => PeriodicityOffsets.RefreshAsync(ct), ct)
           .IfSuccessAsync(() => Circles.RefreshAsync(ct), ct)
           .IfSuccessAsync(() => Steps.RefreshAsync(ct), ct)
           .IfSuccessAsync(() => References.RefreshAsync(ct), ct);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(SortBy))
        {
            Favorite.SortBy = SortBy;
            Items.SortBy = SortBy;
            Missed.SortBy = SortBy;
            ReadyForCompleted.SortBy = SortBy;
            ComingSoon.SortBy = SortBy;
            Completed.SortBy = SortBy;
            Values.SortBy = SortBy;
            Groups.SortBy = SortBy;
            Planneds.SortBy = SortBy;
            Periodicitys.SortBy = SortBy;
            PeriodicityOffsets.SortBy = SortBy;
            Circles.SortBy = SortBy;
            Steps.SortBy = SortBy;
            References.SortBy = SortBy;
        }
    }
}
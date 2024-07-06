namespace Spravy.Ui.Features.ToDo.ViewModels;

public class LeafToDoItemsViewModel
    : NavigatableViewModelBase,
        IRefresh,
        IToDoItemUpdater,
        IToDoSubItemsViewModelProperty
{
    private readonly TaskWork refreshWork;
    private readonly IObjectStorage objectStorage;
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;

    public LeafToDoItemsViewModel(
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        IObjectStorage objectStorage,
        IToDoCache toDoCache,
        ITaskProgressService taskProgressService,
        SpravyCommandNotifyService spravyCommandNotifyService
    )
        : base(true)
    {
        Commands = new();
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.toDoService = toDoService;
        this.objectStorage = objectStorage;
        this.toDoCache = toDoCache;
        refreshWork = TaskWork.Create(errorHandler, RefreshCoreAsync);
        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );

        ToDoSubItemsViewModel
            .List.WhenAnyValue(x => x.IsMulti)
            .Subscribe(x =>
            {
                if (x)
                {
                    Commands.Update(spravyCommandNotifyService.LeafToDoItemsMulti);
                }
                else
                {
                    Commands.Clear();
                }
            });
    }

    public SpravyCommand InitializedCommand { get; }
    public ReadOnlyMemory<Guid> LeafIds { get; set; } = ReadOnlyMemory<Guid>.Empty;
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    public AvaloniaList<SpravyCommandNotify> Commands { get; }

    public override string ViewId
    {
        get => $"{TypeCache<LeafToDoItemsViewModel>.Type.Name}:{Item?.Name}";
    }

    [Reactive]
    public ToDoItemEntityNotify? Item { get; set; }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return RefreshCore(ct).ConfigureAwait(false);
    }

    private async ValueTask<Result> RefreshCore(CancellationToken ct)
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshCoreAsync(CancellationToken ct)
    {
        if (LeafIds.IsEmpty)
        {
            return Item.IfNotNull(nameof(Item))
                .IfSuccessAsync(
                    item =>
                        toDoService
                            .GetLeafToDoItemIdsAsync(item.Id, ct)
                            .IfSuccessForEachAsync(id => toDoCache.GetToDoItem(id), ct)
                            .IfSuccessAsync(
                                items => ToDoSubItemsViewModel.UpdateItemsAsync(items, true, ct),
                                ct
                            ),
                    ct
                );
        }

        return LeafIds
            .ToResult()
            .IfSuccessForEachAsync(id => toDoService.GetLeafToDoItemIdsAsync(id, ct), ct)
            .IfSuccessAsync(items => items.SelectMany().ToReadOnlyMemory().ToResult(), ct)
            .IfSuccessForEachAsync(id => toDoCache.GetToDoItem(id), ct)
            .IfSuccessAsync(items => ToDoSubItemsViewModel.UpdateItemsAsync(items, true, ct), ct);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<LeafToDoItemsViewModelSetting>(ViewId, ct)
            .IfSuccessAsync(obj => SetStateAsync(obj, ct), ct)
            .IfSuccessAsync(() => RefreshAsync(ct), ct);
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new LeafToDoItemsViewModelSetting(this), ct);
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return setting
            .CastObject<LeafToDoItemsViewModelSetting>()
            .IfSuccess(s =>
                this.PostUiBackground(() =>
                {
                    ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                    ToDoSubItemsViewModel.List.IsMulti = s.IsMulti;

                    return Result.Success;
                })
            )
            .ToValueTaskResult()
            .ConfigureAwait(false);
    }

    public Result UpdateInListToDoItemUi(ToDoItemEntityNotify item)
    {
        if (ToDoSubItemsViewModel.List.ToDoItems.GroupByNone.Items.Items.Contains(item))
        {
            return ToDoSubItemsViewModel.List.UpdateItemUi(item);
        }

        return Result.Success;
    }

    [ProtoContract]
    private class LeafToDoItemsViewModelSetting : IViewModelSetting<LeafToDoItemsViewModelSetting>
    {
        public LeafToDoItemsViewModelSetting(LeafToDoItemsViewModel viewModel)
        {
            GroupBy = viewModel.ToDoSubItemsViewModel.List.GroupBy;
            IsMulti = viewModel.ToDoSubItemsViewModel.List.IsMulti;
        }

        public LeafToDoItemsViewModelSetting() { }

        static LeafToDoItemsViewModelSetting()
        {
            Default = new() { GroupBy = GroupBy.ByStatus, };
        }

        [ProtoMember(1)]
        public GroupBy GroupBy { get; set; }

        [ProtoMember(2)]
        public bool IsMulti { get; set; }

        public static LeafToDoItemsViewModelSetting Default { get; }
    }
}

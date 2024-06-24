namespace Spravy.Ui.Features.ToDo.ViewModels;

public class RootToDoItemsViewModel : NavigatableViewModelBase,
    IToDoItemUpdater,
    IToDoSubItemsViewModelProperty,
    IRefresh
{
    private readonly TaskWork refreshWork;
    private readonly IObjectStorage objectStorage;
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;

    public RootToDoItemsViewModel(
        FastAddToDoItemViewModel fastAddToDoItemViewModel,
        SpravyCommandNotifyService spravyCommandNotifyService,
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IToDoCache toDoCache,
        IObjectStorage objectStorage,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    ) : base(true)
    {
        Commands = new();
        FastAddToDoItemViewModel = fastAddToDoItemViewModel;
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.toDoCache = toDoCache;
        this.objectStorage = objectStorage;
        this.toDoService = toDoService;
        refreshWork = TaskWork.Create(errorHandler, RefreshCoreAsync);
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler, taskProgressService);

        toDoSubItemsViewModel.List
           .WhenAnyValue(x => x.IsMulti)
           .Subscribe(x =>
            {
                Commands.Clear();

                if (x)
                {
                    Commands.AddRange(spravyCommandNotifyService.RootToDoItemsMulti.ToArray());
                }
            });
    }

    public AvaloniaList<SpravyCommandNotify> Commands { get; }
    public SpravyCommand InitializedCommand { get; }
    public FastAddToDoItemViewModel FastAddToDoItemViewModel { get; }
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }

    public override string ViewId
    {
        get => TypeCache<RootToDoItemsViewModel>.Type.Name;
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return RefreshCore().ConfigureAwait(false);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return objectStorage.GetObjectOrDefaultAsync<RootToDoItemsViewModelSetting>(ViewId, ct)
           .IfSuccessAsync(obj => SetStateAsync(obj, ct), ct)
           .IfSuccessAsync(() => refreshWork.RunAsync().ToValueTaskResultOnly().ConfigureAwait(false),
                ct);
    }

    public async ValueTask<Result> RefreshCore()
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshCoreAsync(CancellationToken ct)
    {
        return toDoCache.GetRootItems()
           .IfSuccessAsync(items => this.InvokeUiBackgroundAsync(() => ToDoSubItemsViewModel.ClearExceptUi(items)),
                ct)
           .IfSuccessAsync(() => toDoService.GetRootToDoItemIdsAsync(ct), ct)
           .IfSuccessAsync(items => toDoCache.UpdateRootItems(items), ct)
           .IfSuccessAsync(items => ToDoSubItemsViewModel.UpdateItemsAsync(items.ToArray(), false, ct),
                ct);
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new RootToDoItemsViewModelSetting(this), ct);
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return setting.CastObject<RootToDoItemsViewModelSetting>()
           .IfSuccessAsync(s => this.InvokeUiBackgroundAsync(() =>
            {
                ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                ToDoSubItemsViewModel.List.IsMulti = s.IsMulti;

                return Result.Success;
            }), ct);
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
    private class RootToDoItemsViewModelSetting : IViewModelSetting<RootToDoItemsViewModelSetting>
    {
        static RootToDoItemsViewModelSetting()
        {
            Default = new()
            {
                GroupBy = GroupBy.ByStatus,
            };
        }

        public RootToDoItemsViewModelSetting(RootToDoItemsViewModel viewModel)
        {
            GroupBy = viewModel.ToDoSubItemsViewModel.List.GroupBy;
            IsMulti = viewModel.ToDoSubItemsViewModel.List.IsMulti;
        }

        public RootToDoItemsViewModelSetting()
        {
        }

        [ProtoMember(1)]
        public GroupBy GroupBy { get; set; }

        [ProtoMember(2)]
        public bool IsMulti { get; set; }

        public static RootToDoItemsViewModelSetting Default { get; }
    }
}
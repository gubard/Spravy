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
        RootToDoItemsCommands commands,
        PageHeaderViewModel pageHeaderViewModel,
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IToDoCache toDoCache,
        IObjectStorage objectStorage,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    ) : base(true)
    {
        FastAddToDoItemViewModel = fastAddToDoItemViewModel;
        Commands = commands;
        PageHeaderViewModel = pageHeaderViewModel;
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.toDoCache = toDoCache;
        this.objectStorage = objectStorage;
        this.toDoService = toDoService;
        refreshWork = TaskWork.Create(errorHandler, RefreshCoreAsync);
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler, taskProgressService);
        pageHeaderViewModel.Header = "Spravy";
        pageHeaderViewModel.LeftCommand = CommandStorage.NavigateToCurrentToDoItemItem;

        toDoSubItemsViewModel.List
           .WhenAnyValue(x => x.IsMulti)
           .Subscribe(x =>
            {
                PageHeaderViewModel.Commands.Clear();

                if (x)
                {
                    PageHeaderViewModel.Commands.AddRange(Commands.Commands.ToArray());
                }
            });
    }

    public SpravyCommand InitializedCommand { get; }
    public FastAddToDoItemViewModel FastAddToDoItemViewModel { get; }
    public RootToDoItemsCommands Commands { get; }
    public PageHeaderViewModel PageHeaderViewModel { get; }
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }

    public override string ViewId
    {
        get => TypeCache<RootToDoItemsViewModel>.Type.Name;
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return RefreshCore().ConfigureAwait(false);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return objectStorage.GetObjectOrDefaultAsync<RootToDoItemsViewModelSetting>(ViewId, cancellationToken)
           .IfSuccessAsync(obj => SetStateAsync(obj, cancellationToken), cancellationToken)
           .IfSuccessAsync(() => refreshWork.RunAsync().ToValueTaskResultOnly().ConfigureAwait(false),
                cancellationToken);
    }

    public async ValueTask<Result> RefreshCore()
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshCoreAsync(CancellationToken cancellationToken)
    {
        return toDoCache.GetRootItems()
           .IfSuccessAsync(items => this.InvokeUiBackgroundAsync(() => ToDoSubItemsViewModel.ClearExceptUi(items)),
                cancellationToken)
           .IfSuccessAsync(() => toDoService.GetRootToDoItemIdsAsync(cancellationToken), cancellationToken)
           .IfSuccessAsync(items => toDoCache.UpdateRootItems(items), cancellationToken)
           .IfSuccessAsync(items => ToDoSubItemsViewModel.UpdateItemsAsync(items.ToArray(), false, cancellationToken),
                cancellationToken);
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return objectStorage.SaveObjectAsync(ViewId, new RootToDoItemsViewModelSetting(this));
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return setting.CastObject<RootToDoItemsViewModelSetting>()
           .IfSuccessAsync(s => this.InvokeUiBackgroundAsync(() =>
            {
                ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                ToDoSubItemsViewModel.List.IsMulti = s.IsMulti;

                return Result.Success;
            }), cancellationToken);
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
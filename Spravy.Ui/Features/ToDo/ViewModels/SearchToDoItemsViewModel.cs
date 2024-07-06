namespace Spravy.Ui.Features.ToDo.ViewModels;

public class SearchToDoItemsViewModel
    : NavigatableViewModelBase,
        IToDoItemUpdater,
        IToDoSubItemsViewModelProperty,
        IRefresh
{
    private readonly TaskWork refreshWork;
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;
    private readonly IObjectStorage objectStorage;

    public SearchToDoItemsViewModel(
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IToDoService toDoService,
        SpravyCommandNotifyService spravyCommandNotifyService,
        IErrorHandler errorHandler,
        IObjectStorage objectStorage,
        IToDoCache toDoCache,
        ITaskProgressService taskProgressService
    )
        : base(true)
    {
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.toDoService = toDoService;
        this.objectStorage = objectStorage;
        this.toDoCache = toDoCache;
        Commands = new();

        ToDoSubItemsViewModel
            .List.WhenAnyValue(x => x.IsMulti)
            .Subscribe(x =>
            {
                Commands.Clear();

                if (x)
                {
                    Commands.Update(spravyCommandNotifyService.SearchToDoItemsMulti);
                }
                else
                {
                    Commands.Clear();
                }
            });

        refreshWork = TaskWork.Create(errorHandler, RefreshCoreAsync);
        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    public AvaloniaList<SpravyCommandNotify> Commands { get; }

    public override string ViewId
    {
        get => TypeCache<SearchToDoItemsViewModel>.Type.Name;
    }

    [Reactive]
    public string SearchText { get; set; } = string.Empty;

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return refreshWork.RunAsync().ToValueTaskResultOnly().ConfigureAwait(false);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<SearchViewModelSetting>(ViewId, ct)
            .IfSuccessAsync(obj => SetStateAsync(obj, ct), ct);
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshCoreAsync(CancellationToken ct)
    {
        return toDoService
            .SearchToDoItemIdsAsync(SearchText, ct)
            .IfSuccessForEachAsync(id => toDoCache.GetToDoItem(id), ct)
            .IfSuccessAsync(ids => ToDoSubItemsViewModel.UpdateItemsAsync(ids, false, ct), ct);
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new SearchViewModelSetting(this), ct);
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return setting
            .CastObject<SearchViewModelSetting>()
            .IfSuccess(s =>
                this.PostUiBackground(() =>
                {
                    SearchText = s.SearchText;

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
    private class SearchViewModelSetting : IViewModelSetting<SearchViewModelSetting>
    {
        public SearchViewModelSetting(SearchToDoItemsViewModel toDoItemsViewModel)
        {
            SearchText = toDoItemsViewModel.SearchText;
        }

        public SearchViewModelSetting() { }

        static SearchViewModelSetting()
        {
            Default = new();
        }

        [ProtoMember(1)]
        public string SearchText { get; set; } = string.Empty;

        public static SearchViewModelSetting Default { get; }
    }
}

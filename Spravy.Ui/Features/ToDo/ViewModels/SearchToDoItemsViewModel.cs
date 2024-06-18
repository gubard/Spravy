namespace Spravy.Ui.Features.ToDo.ViewModels;

public class SearchToDoItemsViewModel : NavigatableViewModelBase,
    IToDoItemSearchProperties,
    IToDoItemUpdater,
    IToDoSubItemsViewModelProperty
{
    private readonly TaskWork refreshWork;
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;
    private readonly IObjectStorage objectStorage;

    public SearchToDoItemsViewModel(
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IToDoService toDoService,
        INavigator navigator,
        IUiApplicationService uiApplicationService,
        IDialogViewer dialogViewer,
        IClipboardService clipboardService,
        IOpenerLink openerLink,
        IErrorHandler errorHandler,
        IObjectStorage objectStorage,
        IToDoCache toDoCache
    ) : base(true)
    {
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.toDoService = toDoService;
        this.objectStorage = objectStorage;
        this.toDoCache = toDoCache;
        Commands = new();

        ToDoSubItemsViewModel.List
           .WhenAnyValue(x => x.IsMulti)
           .Subscribe(x =>
            {
                Commands.Clear();

                if (x)
                {
                    Commands.AddRange([
                        SpravyCommandNotify.CreateMultiCloneItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiCompleteItem(uiApplicationService, toDoService, errorHandler),
                        SpravyCommandNotify.CreateMultiDeleteItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiResetItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiAddChildItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiCloneItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiChangeOrderItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiChangeParentItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiOpenLeafItem(uiApplicationService, navigator, errorHandler),
                        SpravyCommandNotify.CreateMultiOpenLinkItem(uiApplicationService, openerLink, errorHandler),
                        SpravyCommandNotify.CreateMultiShowSettingItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiAddToFavoriteItem(uiApplicationService, toDoService,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiCopyToClipboardItem(uiApplicationService, toDoService,
                            dialogViewer,
                            clipboardService, errorHandler),
                        SpravyCommandNotify.CreateMultiRemoveFromFavoriteItem(uiApplicationService, toDoService,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiRandomizeChildrenOrderItem(uiApplicationService, toDoService,
                            dialogViewer, errorHandler),
                        SpravyCommandNotify.CreateMultiMakeAsRootItem(uiApplicationService, toDoService, errorHandler),
                    ]);
                }
            });

        refreshWork = TaskWork.Create(RefreshCoreAsync);
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler);
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

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return refreshWork.RunAsync().ToValueTaskResultOnly().ConfigureAwait(false);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return objectStorage.GetObjectOrDefaultAsync<SearchViewModelSetting>(ViewId, cancellationToken)
           .IfSuccessAsync(obj => SetStateAsync(obj, cancellationToken), cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshCoreAsync(CancellationToken cancellationToken)
    {
        return toDoService.SearchToDoItemIdsAsync(SearchText, cancellationToken)
           .IfSuccessForEachAsync(id => toDoCache.GetToDoItem(id), cancellationToken)
           .IfSuccessAsync(ids => ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), false, cancellationToken),
                cancellationToken);
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return objectStorage.SaveObjectAsync(ViewId, new SearchViewModelSetting(this));
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return setting.CastObject<SearchViewModelSetting>()
           .IfSuccessAsync(s => this.InvokeUiBackgroundAsync(() =>
            {
                SearchText = s.SearchText;

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
    private class SearchViewModelSetting : IViewModelSetting<SearchViewModelSetting>
    {
        public SearchViewModelSetting(SearchToDoItemsViewModel toDoItemsViewModel)
        {
            SearchText = toDoItemsViewModel.SearchText;
        }

        public SearchViewModelSetting()
        {
        }

        static SearchViewModelSetting()
        {
            Default = new();
        }

        [ProtoMember(1)]
        public string SearchText { get; set; } = string.Empty;

        public static SearchViewModelSetting Default { get; }
    }
}
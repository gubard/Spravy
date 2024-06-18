namespace Spravy.Ui.Features.ToDo.ViewModels;

public class LeafToDoItemsViewModel : NavigatableViewModelBase,
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
        PageHeaderViewModel pageHeaderViewModel,
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
        PageHeaderViewModel = pageHeaderViewModel;
        this.toDoService = toDoService;
        this.objectStorage = objectStorage;
        this.toDoCache = toDoCache;
        PageHeaderViewModel.LeftCommand = CommandStorage.NavigateToCurrentToDoItemItem;

        ToDoSubItemsViewModel.List
           .WhenAnyValue(x => x.IsMulti)
           .Subscribe(x =>
            {
                PageHeaderViewModel.Commands.Clear();

                if (x)
                {
                    PageHeaderViewModel.Commands.AddRange([
                        SpravyCommandNotify.CreateMultiCloneItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiCompleteItem(uiApplicationService, toDoService, errorHandler),
                        SpravyCommandNotify.CreateMultiDeleteItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiResetItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiAddChildItem(uiApplicationService, toDoService, dialogViewer, errorHandler),
                        SpravyCommandNotify.CreateMultiCloneItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiChangeOrderItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiChangeParentItem(uiApplicationService, toDoService, dialogViewer,
                            errorHandler),
                        SpravyCommandNotify.CreateMultiOpenLeafItem(uiApplicationService, navigator, errorHandler),
                        SpravyCommandNotify.CreateMultiOpenLinkItem(uiApplicationService, openerLink, errorHandler),
                        SpravyCommandNotify.CreateMultiShowSettingItem(uiApplicationService, toDoService, dialogViewer, errorHandler),
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
    public ReadOnlyMemory<Guid> LeafIds { get; set; } = ReadOnlyMemory<Guid>.Empty;
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    public PageHeaderViewModel PageHeaderViewModel { get; }

    public override string ViewId
    {
        get => $"{TypeCache<LeafToDoItemsViewModel>.Type.Name}:{Item?.Name}";
    }

    [Reactive]
    public ToDoItemEntityNotify? Item { get; set; }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return RefreshCore(cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> RefreshCore(CancellationToken cancellationToken)
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshCoreAsync(CancellationToken cancellationToken)
    {
        if (LeafIds.IsEmpty)
        {
            return Item.IfNotNull(nameof(Item))
               .IfSuccessAsync(
                    item => toDoService.GetLeafToDoItemIdsAsync(item.Id, cancellationToken)
                       .IfSuccessForEachAsync(id => toDoCache.GetToDoItem(id), cancellationToken)
                       .IfSuccessAsync(items => ToDoSubItemsViewModel.UpdateItemsAsync(items, true, cancellationToken),
                            cancellationToken), cancellationToken);
        }

        return LeafIds.ToResult()
           .IfSuccessForEachAsync(id => toDoService.GetLeafToDoItemIdsAsync(id, cancellationToken), cancellationToken)
           .IfSuccessAsync(items => items.SelectMany().ToReadOnlyMemory().ToResult(), cancellationToken)
           .IfSuccessForEachAsync(id => toDoCache.GetToDoItem(id), cancellationToken)
           .IfSuccessAsync(items => ToDoSubItemsViewModel.UpdateItemsAsync(items, true, cancellationToken),
                cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return objectStorage.GetObjectOrDefaultAsync<LeafToDoItemsViewModelSetting>(ViewId, cancellationToken)
           .IfSuccessAsync(obj => SetStateAsync(obj, cancellationToken), cancellationToken)
           .IfSuccessAsync(() => RefreshAsync(cancellationToken), cancellationToken);
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return objectStorage.SaveObjectAsync(ViewId, new LeafToDoItemsViewModelSetting(this));
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return setting.CastObject<LeafToDoItemsViewModelSetting>()
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
    private class LeafToDoItemsViewModelSetting : IViewModelSetting<LeafToDoItemsViewModelSetting>
    {
        public LeafToDoItemsViewModelSetting(LeafToDoItemsViewModel viewModel)
        {
            GroupBy = viewModel.ToDoSubItemsViewModel.List.GroupBy;
            IsMulti = viewModel.ToDoSubItemsViewModel.List.IsMulti;
        }

        public LeafToDoItemsViewModelSetting()
        {
        }

        static LeafToDoItemsViewModelSetting()
        {
            Default = new()
            {
                GroupBy = GroupBy.ByStatus,
            };
        }

        [ProtoMember(1)]
        public GroupBy GroupBy { get; set; }

        [ProtoMember(2)]
        public bool IsMulti { get; set; }

        public static LeafToDoItemsViewModelSetting Default { get; }
    }
}
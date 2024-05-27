namespace Spravy.Ui.Features.ToDo.ViewModels;

public class RootToDoItemsViewModel : NavigatableViewModelBase,
    IToDoItemOrderChanger,
    ITaskProgressServiceProperty,
    IToDoItemUpdater
{
    private readonly PageHeaderViewModel pageHeaderViewModel;
    private readonly TaskWork refreshWork;
    private readonly ToDoSubItemsViewModel toDoSubItemsViewModel;
    
    public RootToDoItemsViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    public ICommand InitializedCommand { get; }
    
    public override string ViewId
    {
        get => TypeCache<RootToDoItemsViewModel>.Type.Name;
    }
    
    [Inject]
    public required FastAddToDoItemViewModel FastAddToDoItemViewModel { get; init; }
    
    [Inject]
    public required ITaskProgressService TaskProgressService { get; init; }
    
    [Inject]
    public required RootToDoItemsCommands Commands { get; init; }
    
    [Inject]
    public required PageHeaderViewModel PageHeaderViewModel
    {
        get => pageHeaderViewModel;
        [MemberNotNull(nameof(pageHeaderViewModel))]
        init
        {
            pageHeaderViewModel = value;
            pageHeaderViewModel.Header = "Spravy";
            pageHeaderViewModel.LeftCommand = CommandStorage.NavigateToCurrentToDoItemItem;
        }
    }
    
    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel
    {
        get => toDoSubItemsViewModel;
        [MemberNotNull(nameof(toDoSubItemsViewModel))]
        init
        {
            toDoSubItemsViewModel = value;
            
            toDoSubItemsViewModel.List
               .WhenAnyValue(x => x.IsMulti)
               .Subscribe(x =>
                {
                    if (pageHeaderViewModel is null)
                    {
                        return;
                    }
                    
                    pageHeaderViewModel.Commands.Clear();
                    
                    if (x)
                    {
                        pageHeaderViewModel.Commands.AddRange([
                            Commands.MultiAddChildItem,
                            Commands.MultiShowSettingItem,
                            Commands.MultiDeleteItem,
                            Commands.MultiOpenLeafItem,
                            Commands.MultiChangeParentItem,
                            Commands.MultiMakeAsRootItem,
                            Commands.MultiCopyToClipboardItem,
                            Commands.MultiRandomizeChildrenOrderItem,
                            Commands.MultiChangeOrderItem,
                            Commands.MultiResetItem,
                            Commands.MultiCloneItem,
                            Commands.MultiOpenLinkItem,
                            Commands.MultiAddToFavoriteItem,
                            Commands.MultiRemoveFromFavoriteItem,
                            Commands.MultiCompleteItem,
                        ]);
                    }
                });
        }
    }
    
    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
    [Inject]
    public required IToDoCache ToDoCache { get; init; }
    
    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return RefreshCore().ConfigureAwait(false);
    }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return TaskProgressService.RunProgressAsync(
            () => ObjectStorage.GetObjectOrDefaultAsync<RootToDoItemsViewModelSetting>(ViewId, cancellationToken)
               .IfSuccessAsync(obj => SetStateAsync(obj, cancellationToken), cancellationToken)
               .IfSuccessAsync(() => refreshWork.RunAsync().ToValueTaskResultOnly().ConfigureAwait(false),
                    cancellationToken), cancellationToken);
    }
    
    public async ValueTask<Result> RefreshCore()
    {
        await refreshWork.RunAsync();
        
        return Result.Success;
    }
    
    private ConfiguredValueTaskAwaitable<Result> RefreshCoreAsync(CancellationToken cancellationToken)
    {
        return ToDoCache.GetRootItems()
           .IfSuccessAsync(items => this.InvokeUiBackgroundAsync(() => ToDoSubItemsViewModel.ClearExceptUi(items)),
                cancellationToken)
           .IfSuccessAsync(() => ToDoService.GetRootToDoItemIdsAsync(cancellationToken), cancellationToken)
           .IfSuccessAsync(items => ToDoCache.UpdateRootItems(items), cancellationToken)
           .IfSuccessAsync(
                items => ToDoSubItemsViewModel.UpdateItemsAsync(items.ToArray(), this, false, cancellationToken),
                cancellationToken);
    }
    
    public override Result Stop()
    {
        refreshWork.Cancel();
        
        return Result.Success;
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new RootToDoItemsViewModelSetting(this));
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
    
    public Result UpdateToDoItemUi(ToDoItemEntityNotify item)
    {
        return ToDoSubItemsViewModel.List.UpdateItemUi(item);
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
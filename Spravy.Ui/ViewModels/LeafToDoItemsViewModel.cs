namespace Spravy.Ui.ViewModels;

public class LeafToDoItemsViewModel : NavigatableViewModelBase, IRefresh
{
    private readonly PageHeaderViewModel pageHeaderViewModel;
    private readonly TaskWork refreshWork;
    
    public LeafToDoItemsViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    public ICommand InitializedCommand { get; }
    public ReadOnlyMemory<Guid> LeafIds { get; set; } = ReadOnlyMemory<Guid>.Empty;
    
    public override string ViewId
    {
        get => $"{TypeCache<LeafToDoItemsViewModel>.Type.Name}:{Item?.Name}";
    }
    
    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
    [Inject]
    public required IToDoCache ToDoCache { get; init; }
    
    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel { get; init; }
    
    [Inject]
    public required PageHeaderViewModel PageHeaderViewModel
    {
        get => pageHeaderViewModel;
        [MemberNotNull(nameof(pageHeaderViewModel))]
        init
        {
            pageHeaderViewModel = value;
            pageHeaderViewModel.LeftCommand = CommandStorage.NavigateToCurrentToDoItemItem;
        }
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
                    item => ToDoService.GetLeafToDoItemIdsAsync(item.Id, cancellationToken)
                       .IfSuccessForEachAsync(id => ToDoCache.GetToDoItem(id), cancellationToken)
                       .IfSuccessAsync(
                            items => ToDoSubItemsViewModel.UpdateItemsAsync(items, this, true, cancellationToken),
                            cancellationToken), cancellationToken);
        }
        
        return LeafIds.ToResult()
           .IfSuccessForEachAsync(id => ToDoService.GetLeafToDoItemIdsAsync(id, cancellationToken), cancellationToken)
           .IfSuccessAsync(items => items.SelectMany().ToReadOnlyMemory().ToResult(), cancellationToken)
           .IfSuccessForEachAsync(id => ToDoCache.GetToDoItem(id), cancellationToken)
           .IfSuccessAsync(items => ToDoSubItemsViewModel.UpdateItemsAsync(items, this, true, cancellationToken),
                cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.GetObjectOrDefaultAsync<LeafToDoItemsViewModelSetting>(ViewId, cancellationToken)
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
        return ObjectStorage.SaveObjectAsync(ViewId, new LeafToDoItemsViewModelSetting(this));
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
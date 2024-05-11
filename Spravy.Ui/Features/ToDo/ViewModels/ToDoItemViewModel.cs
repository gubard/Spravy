using Spravy.Ui.Features.ToDo.Interfaces;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemViewModel : NavigatableViewModelBase, ITaskProgressServiceProperty, IRefresh
{
    private readonly TaskWork refreshWork;
    
    public ToDoItemViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    public ICommand InitializedCommand { get; }
    
    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel { get; init; }
    
    [Inject]
    public required FastAddToDoItemViewModel FastAddToDoItemViewModel { get; init; }
    
    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }
    
    public Guid Id { get; set; }
    
    [Reactive]
    public ToDoItemEntityNotify? Item { get; set; }
    
    public bool IsNavigateToParent
    {
        get => true;
    }
    
    public override string ViewId
    {
        get => $"{TypeCache<ToDoItemViewModel>.Type.Name}:{Id}";
    }
    
    [Inject]
    public required IToDoCache ToDoCache { get; set; }
    
    [Inject]
    public required IToDoService ToDoService { get; set; }
    
    [Inject]
    public required ITaskProgressService TaskProgressService { get; init; }
    
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
        FastAddToDoItemViewModel.ParentId = Id;
        
        return RefreshPathAsync(cancellationToken)
           .IfSuccessAllAsync(cancellationToken, () => RefreshToDoItemCore(cancellationToken),
                () => RefreshToDoItemChildrenAsync(cancellationToken));
    }
    
    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemCore(CancellationToken cancellationToken)
    {
        return ToDoCache.GetToDoItem(Id)
           .IfSuccessAsync(item => this.InvokeUIBackgroundAsync(() => Item = item), cancellationToken)
           .IfSuccessAsync(() => ToDoService.GetToDoItemAsync(Id, cancellationToken), cancellationToken)
           .IfSuccessAsync(item => ToDoCache.UpdateAsync(item, cancellationToken), cancellationToken)
           .ToResultOnlyAsync();
    }
    
    private ConfiguredValueTaskAwaitable<Result> RefreshPathAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetParentsAsync(Id, cancellationToken)
           .IfSuccessAsync(parents => ToDoCache.UpdateAsync(Id, parents, cancellationToken), cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemChildrenAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetChildrenToDoItemIdsAsync(Id, cancellationToken)
           .IfSuccessForEachAsync(id => ToDoCache.GetToDoItem(id), cancellationToken)
           .IfSuccessAsync(items => ToDoSubItemsViewModel.UpdateItemsAsync(items, this, false, cancellationToken),
                cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return this.RunProgressAsync(() =>
        {
            FastAddToDoItemViewModel.Refresh = this;
            
            return ObjectStorage.GetObjectOrDefaultAsync<ToDoItemViewModelSetting>(ViewId, cancellationToken)
               .IfSuccessAsync(obj => SetStateAsync(obj, cancellationToken), cancellationToken)
               .IfSuccessAllAsync(cancellationToken, () => RefreshAsync(cancellationToken));
        }, cancellationToken);
    }
    
    public override Result Stop()
    {
        refreshWork.Cancel();
        
        return Result.Success;
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new ToDoItemViewModelSetting(this));
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return setting.CastObject<ToDoItemViewModelSetting>()
           .IfSuccessAsync(s => this.InvokeUIBackgroundAsync(() =>
            {
                ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                ToDoSubItemsViewModel.List.IsMulti = s.IsMulti;
            }), cancellationToken);
    }
    
    [ProtoContract]
    private class ToDoItemViewModelSetting : IViewModelSetting<ToDoItemViewModelSetting>
    {
        public ToDoItemViewModelSetting(ToDoItemViewModel viewModel)
        {
            GroupBy = viewModel.ToDoSubItemsViewModel.List.GroupBy;
            IsMulti = viewModel.ToDoSubItemsViewModel.List.IsMulti;
        }
        
        public ToDoItemViewModelSetting()
        {
        }
        
        static ToDoItemViewModelSetting()
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
        
        public static ToDoItemViewModelSetting Default { get; }
    }
}
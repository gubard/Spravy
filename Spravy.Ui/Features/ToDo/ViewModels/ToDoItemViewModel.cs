using Spravy.Ui.Features.ToDo.Commands;
using Spravy.Ui.Features.ToDo.Interfaces;
using Spravy.Ui.Features.ToDo.Settings;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemViewModel : NavigatableViewModelBase, ITaskProgressServiceProperty, IRefresh
{
    private readonly TaskWork refreshWork;
    
    public ToDoItemViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
    }
    
    [Inject]
    public required ToDoItemCommands Commands { get; init; }
    
    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel { get; init; }
    
    [Inject]
    public required FastAddToDoItemViewModel FastAddToDoItemViewModel { get; init; }
    
    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }
    
    public Guid Id { get; set; }
    
    [Reactive]
    public ToDoItemEntityNotify? Item { get; set; }
    
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
        return RefreshCore().ConfigureAwait(false);
    }
    
    private async ValueTask<Result> RefreshCore()
    {
        await refreshWork.RunAsync();
        
        return Result.Success;
    }
    
    private ConfiguredValueTaskAwaitable<Result> RefreshCoreAsync(CancellationToken cancellationToken)
    {
        FastAddToDoItemViewModel.ParentId = Id;
        
        return ToDoCache.GetChildrenItems(Id)
           .IfSuccessAsync(items => ToDoSubItemsViewModel.ClearExceptAsync(items), cancellationToken)
           .IfSuccessAsync(() => RefreshPathAsync(cancellationToken), cancellationToken)
           .IfSuccessAsync(() => RefreshToDoItemCore(cancellationToken), cancellationToken)
           .IfSuccessAsync(() => RefreshToDoItemChildrenAsync(cancellationToken), cancellationToken);
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
        return ToDoCache.GetChildrenItems(Id)
           .IfSuccessAsync(items => ToDoSubItemsViewModel.ClearExceptAsync(items), cancellationToken)
           .IfSuccessAsync(() => ToDoService.GetChildrenToDoItemIdsAsync(Id, cancellationToken), cancellationToken)
           .IfSuccessAsync(ids => ToDoCache.UpdateChildrenItems(Id, ids), cancellationToken)
           .IfSuccessAsync(items => ToDoSubItemsViewModel.UpdateItemsAsync(items, this, false, cancellationToken),
                cancellationToken);
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
}
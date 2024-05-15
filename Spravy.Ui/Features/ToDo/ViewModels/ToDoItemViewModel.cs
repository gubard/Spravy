using Spravy.Ui.Features.ToDo.Commands;
using Spravy.Ui.Features.ToDo.Interfaces;
using Spravy.Ui.Features.ToDo.Settings;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemViewModel : NavigatableViewModelBase, ITaskProgressServiceProperty, IRefresh
{
    private readonly TaskWork refreshWork;
    private readonly ToDoSubItemsViewModel toDoSubItemsViewModel;
    
    public ToDoItemViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        CommandItems = new();
    }
    
    public Guid Id { get; set; }
    public AvaloniaList<SpravyCommandNotify> CommandItems { get; }
    
    [Inject]
    public required ToDoItemCommands Commands { get; init; }
    
    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel
    {
        get => toDoSubItemsViewModel;
        [MemberNotNull(nameof(toDoSubItemsViewModel))]
        init
        {
            toDoSubItemsViewModel = value;
            toDoSubItemsViewModel.List.WhenAnyValue(x => x.IsMulti).Subscribe(_ => UpdateCommandItems());
        }
    }
    
    [Inject]
    public required FastAddToDoItemViewModel FastAddToDoItemViewModel { get; init; }
    
    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }
    
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
        
        return Result.AwaitableFalse.IfSuccessAllAsync(cancellationToken,
            () => RefreshToDoItemChildrenAsync(cancellationToken), () => RefreshToDoItemCore(cancellationToken),
            () => RefreshPathAsync(cancellationToken));
    }
    
    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemCore(CancellationToken cancellationToken)
    {
        return ToDoCache.GetToDoItem(Id)
           .IfSuccessAsync(item => this.InvokeUiBackgroundAsync(() => Item = item), cancellationToken)
           .IfSuccessAsync(() => ToDoService.GetToDoItemAsync(Id, cancellationToken), cancellationToken)
           .IfSuccessAsync(item => ToDoCache.UpdateAsync(item, cancellationToken), cancellationToken)
           .IfSuccessAsync(_ => this.InvokeUiBackgroundAsync(UpdateCommandItems), cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> RefreshPathAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetParentsAsync(Id, cancellationToken)
           .IfSuccessAsync(parents => ToDoCache.UpdateParentsAsync(Id, parents, cancellationToken), cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemChildrenAsync(CancellationToken cancellationToken)
    {
        return ToDoSubItemsViewModel
           .ClearExceptAsync(Item?.Children.ToArray() ?? ReadOnlyMemory<ToDoItemEntityNotify>.Empty, cancellationToken)
           .IfSuccessAsync(() => ToDoService.GetChildrenToDoItemIdsAsync(Id, cancellationToken), cancellationToken)
           .IfSuccessAsync(ids => ToDoCache.UpdateChildrenItemsAsync(Id, ids, cancellationToken), cancellationToken)
           .IfSuccessAsync(items => ToDoSubItemsViewModel.UpdateItemsAsync(items, this, false, cancellationToken),
                cancellationToken);
    }
    
    private void UpdateCommandItems()
    {
        CommandItems.Clear();
        
        if (Item is null)
        {
            return;
        }
        
        CommandItems.AddRange(ToDoSubItemsViewModel.List.IsMulti ? Item.MultiCommands : Item.SingleCommands);
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
           .IfSuccessAsync(s => this.InvokeUiBackgroundAsync(() =>
            {
                ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                ToDoSubItemsViewModel.List.IsMulti = s.IsMulti;
            }), cancellationToken);
    }
}
namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemViewModel : NavigatableViewModelBase, IRefresh, IToDoItemUpdater
{
    private readonly TaskWork refreshWork;
    private readonly IObjectStorage objectStorage;
    private readonly IToDoCache toDoCache;
    private readonly IToDoService toDoService;
    
    private Guid id;
    
    public ToDoItemViewModel(
        IObjectStorage objectStorage,
        ToDoItemCommands commands,
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        FastAddToDoItemViewModel fastAddToDoItemViewModel,
        ITaskProgressService taskProgressService,
        IToDoService toDoService,
        IToDoCache toDoCache
    ) : base(true)
    {
        this.objectStorage = objectStorage;
        Commands = commands;
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        FastAddToDoItemViewModel = fastAddToDoItemViewModel;
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        CommandItems = new();
        ToDoSubItemsViewModel.List.WhenAnyValue(x => x.IsMulti).Subscribe(_ => UpdateCommandItemsUi());
    }
    
    public Guid Id
    {
        get => id;
        set
        {
            id = value;
            FastAddToDoItemViewModel.ParentId = id;
        }
    }
    
    public AvaloniaList<SpravyCommandNotify> CommandItems { get; }
    public ToDoItemCommands Commands { get; }
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    public FastAddToDoItemViewModel FastAddToDoItemViewModel { get; }
    
    [Reactive]
    public ToDoItemEntityNotify? Item { get; set; }
    
    public override string ViewId
    {
        get => $"{TypeCache<ToDoItemViewModel>.Type.Name}:{Id}";
    }
    
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
        return Result.AwaitableSuccess.IfSuccessAllAsync(cancellationToken,
            () => RefreshToDoItemChildrenAsync(cancellationToken), () => RefreshToDoItemCore(cancellationToken),
            () => RefreshPathAsync(cancellationToken));
    }
    
    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemCore(CancellationToken cancellationToken)
    {
        return toDoCache.GetToDoItem(Id)
           .IfSuccessAsync(item => this.InvokeUiBackgroundAsync(() =>
            {
                Item = item;
                
                return Result.Success;
            }), cancellationToken)
           .IfSuccessAsync(() => toDoService.GetToDoItemAsync(Id, cancellationToken), cancellationToken)
           .IfSuccessAsync(
                item => this.InvokeUiBackgroundAsync(() =>
                    toDoCache.UpdateUi(item).IfSuccess(_ => UpdateCommandItemsUi())),
                cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> RefreshPathAsync(CancellationToken cancellationToken)
    {
        return toDoService.GetParentsAsync(Id, cancellationToken)
           .IfSuccessAsync(parents => this.InvokeUiBackgroundAsync(() => toDoCache.UpdateParentsUi(Id, parents)),
                cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemChildrenAsync(CancellationToken cancellationToken)
    {
        return this
           .InvokeUiBackgroundAsync(() =>
                ToDoSubItemsViewModel.ClearExceptUi(Item?.Children.ToArray()
                 ?? ReadOnlyMemory<ToDoItemEntityNotify>.Empty))
           .IfSuccessAsync(() => toDoService.GetChildrenToDoItemIdsAsync(Id, cancellationToken), cancellationToken)
           .IfSuccessAsync(ids => this.InvokeUiBackgroundAsync(() => toDoCache.UpdateChildrenItemsUi(Id, ids)),
                cancellationToken)
           .IfSuccessAsync(items => ToDoSubItemsViewModel.UpdateItemsAsync(items, false, cancellationToken),
                cancellationToken);
    }
    
    private Result UpdateCommandItemsUi()
    {
        CommandItems.Clear();
        
        if (Item is null)
        {
            return Result.Success;
        }
        
        CommandItems.AddRange(ToDoSubItemsViewModel.List.IsMulti ? Item.MultiCommands : Item.SingleCommands);
        
        return Result.Success;
    }
    
    public override Result Stop()
    {
        refreshWork.Cancel();
        
        return Result.Success;
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return objectStorage.SaveObjectAsync(ViewId, new ToDoItemViewModelSetting(this));
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
}
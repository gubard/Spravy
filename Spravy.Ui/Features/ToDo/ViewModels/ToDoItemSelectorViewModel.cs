using Spravy.Ui.Features.ToDo.Interfaces;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemSelectorViewModel : ViewModelBase
{
    private readonly List<ToDoItemEntityNotify> itemsCache = new();
    
    public ToDoItemSelectorViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
    [Inject]
    public required IMapper Mapper { get; init; }
    
    public AvaloniaList<ToDoItemEntityNotify> Roots { get; } = new();
    public ICommand InitializedCommand { get; }
    public List<Guid> IgnoreIds { get; } = new();
    
    public Guid DefaultSelectedItemId { get; set; }
    
    [Inject]
    public required IToDoCache ToDoCache { get; init; }
    
    [Reactive]
    public ICommand SearchCommand { get; protected set; }
    
    [Reactive]
    public string SearchText { get; set; } = string.Empty;
    
    [Reactive]
    public ToDoItemEntityNotify? SelectedItem { get; set; }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        SearchCommand = CreateCommandFromTask(TaskWork.Create(SearchAsync).RunAsync);
        
        return Refresh(cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> Refresh(CancellationToken cancellationToken)
    {
        return ToDoCache.GetRootItems()
           .IfSuccessAsync(items => this.InvokeUiBackgroundAsync(() =>
            {
                Roots.Clear();
                Roots.AddRange(items.ToArray());
            }), cancellationToken)
           .IfSuccessAsync(() => SetupAsync(cancellationToken), cancellationToken)
           .IfSuccessAsync(() => ToDoService.GetToDoSelectorItemsAsync(IgnoreIds.ToArray(), cancellationToken),
                cancellationToken)
           .IfSuccessAsync(items => ToDoCache.UpdateAsync(items, cancellationToken), cancellationToken)
           .IfSuccessAsync(items => this.InvokeUiBackgroundAsync(() =>
            {
                Roots.Clear();
                Roots.AddRange(items.ToArray());
            }), cancellationToken)
           .IfSuccessAsync(() => SetupAsync(cancellationToken), cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> SetupAsync(
        ToDoItemEntityNotify item,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableFalse
           .IfSuccessAsync(() =>
            {
                if (IgnoreIds.Contains(item.Id))
                {
                    return this.InvokeUiBackgroundAsync(() => item.IsIgnore = true);
                }
                
                return Result.AwaitableFalse;
            }, cancellationToken)
           .IfSuccessAsync(() =>
            {
                if (DefaultSelectedItemId == item.Id)
                {
                    return this.InvokeUiBackgroundAsync(() => SelectedItem = item)
                       .IfSuccessAsync(() => ExpandParentsAsync(item, cancellationToken), cancellationToken);
                }
                
                return Result.AwaitableFalse;
            }, cancellationToken)
           .IfSuccessAsync(
                () => item.Children
                   .ToArray()
                   .ToReadOnlyMemory()
                   .ToResult()
                   .IfSuccessForEachAsync(i => SetupAsync(i, cancellationToken), cancellationToken), cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> ExpandParentsAsync(
        ToDoItemEntityNotify item,
        CancellationToken cancellationToken
    )
    {
        return this.InvokeUiBackgroundAsync(() => item.IsExpanded = true)
           .IfSuccessAsync(() =>
            {
                if (item.Parent == null)
                {
                    return Result.AwaitableFalse;
                }
                
                return ExpandParentsAsync(item.Parent, cancellationToken);
            }, cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> SetupAsync(CancellationToken cancellationToken)
    {
        return Roots.ToArray()
           .ToReadOnlyMemory()
           .ToResult()
           .IfSuccessForEachAsync(item => SetupAsync(item, cancellationToken), cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> SearchAsync(CancellationToken cancellationToken)
    {
        return this.InvokeUiBackgroundAsync(() => Roots.Clear())
           .IfSuccessAsync(() => ToDoCache.GetRootItems(), cancellationToken)
           .IfSuccessForEachAsync(item => SearchAsync(item, cancellationToken), cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> SearchAsync(
        ToDoItemEntityNotify item,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableFalse
           .IfSuccessAsync(() =>
            {
                if (item.Name.Contains(SearchText))
                {
                    return this.InvokeUiBackgroundAsync(() => Roots.Add(item));
                }
                
                return Result.AwaitableFalse;
            }, cancellationToken)
           .IfSuccessAsync(
                () => item.Children
                   .ToArray()
                   .ToReadOnlyMemory()
                   .ToResult()
                   .IfSuccessForEachAsync(i => SearchAsync(i, cancellationToken), cancellationToken),
                cancellationToken);
    }
}
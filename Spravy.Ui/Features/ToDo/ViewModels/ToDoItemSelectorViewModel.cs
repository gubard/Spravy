using Spravy.Ui.Features.ToDo.Interfaces;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemSelectorViewModel : ViewModelBase
{
    private readonly List<ToDoItemEntityNotify> itemsCache = new();
    
    public ToDoItemSelectorViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    public AvaloniaList<ToDoItemEntityNotify> Roots { get; } = new();
    public ICommand InitializedCommand { get; }
    public ReadOnlyMemory<Guid> IgnoreIds { get; set; } = ReadOnlyMemory<Guid>.Empty;
    public Guid DefaultSelectedItemId { get; set; }
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
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
                
                return SetupUi();
            }), cancellationToken)
           .IfSuccessAsync(() => ToDoService.GetToDoSelectorItemsAsync(IgnoreIds.ToArray(), cancellationToken),
                cancellationToken)
           .IfSuccessAsync(items => this.InvokeUiBackgroundAsync(() => ToDoCache.UpdateUi(items)), cancellationToken)
           .IfSuccessAsync(items => this.InvokeUiBackgroundAsync(() =>
            {
                Roots.Clear();
                Roots.AddRange(items.ToArray());
                
                return SetupUi();
            }), cancellationToken);
    }
    
    private Result SetupUi(ToDoItemEntityNotify item)
    {
        if (IgnoreIds.Span.Contains(item.Id))
        {
            item.IsIgnore = true;
        }
        
        if (DefaultSelectedItemId == item.Id)
        {
            SelectedItem = item;
            var result = ExpandParentsUi(item);
            
            if (result.IsHasError)
            {
                return result;
            }
        }
        
        return item.Children.ToArray().ToReadOnlyMemory().ToResult().IfSuccessForEach(SetupUi);
    }
    
    private Result ExpandParentsUi(ToDoItemEntityNotify item)
    {
        item.IsExpanded = true;
        
        if (item.Parent == null)
        {
            return Result.Success;
        }
        
        return ExpandParentsUi(item.Parent);
    }
    
    private Result SetupUi()
    {
        return Roots.ToArray().ToReadOnlyMemory().ToResult().IfSuccessForEach(item => SetupUi(item));
    }
    
    private ConfiguredValueTaskAwaitable<Result> SearchAsync(CancellationToken cancellationToken)
    {
        return this.InvokeUiBackgroundAsync(() =>
            {
                Roots.Clear();
                
                return Result.Success;
            })
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
                    return this.InvokeUiBackgroundAsync(() =>
                    {
                        Roots.Add(item);
                        
                        return Result.Success;
                    });
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
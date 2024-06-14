namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoSubItemsViewModel : ViewModelBase
{
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;
    private readonly ITaskProgressService taskProgressService;
    
    public ToDoSubItemsViewModel(
        IToDoService toDoService,
        IToDoCache toDoCache,
        MultiToDoItemsViewModel list,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        List = list;
        this.taskProgressService = taskProgressService;
    }
    
    public MultiToDoItemsViewModel List { get; }
    
    private ConfiguredValueTaskAwaitable<Result> RefreshFavoriteToDoItemsAsync(CancellationToken cancellationToken)
    {
        return toDoService.GetFavoriteToDoItemIdsAsync(cancellationToken)
           .IfSuccessAsync(
                items => taskProgressService.RunProgressAsync((ushort)items.Length,
                    item => items.ToResult()
                       .IfSuccessForEach(x => toDoCache.GetToDoItem(x))
                       .IfSuccessAsync(
                            itemsNotify => List.ClearFavoriteExceptUi(itemsNotify)
                               .IfSuccessAsync(
                                    () => RefreshFavoriteToDoItemsCore(itemsNotify, item, cancellationToken)
                                       .ConfigureAwait(false), cancellationToken), cancellationToken),
                    cancellationToken), cancellationToken);
    }
    
    private async ValueTask<Result> RefreshFavoriteToDoItemsCore(
        ReadOnlyMemory<ToDoItemEntityNotify> ids,
        TaskProgressItem progressItem,
        CancellationToken cancellationToken
    )
    {
        await foreach (var items in toDoService
           .GetToDoItemsAsync(ids.ToArray().Select(x => x.Id).ToArray(), 5, cancellationToken)
           .ConfigureAwait(false))
        {
            if (items.IsHasError)
            {
                return new(items.Errors);
            }
            
            foreach (var item in items.Value.ToArray())
            {
                var i = await this.InvokeUiBackgroundAsync(() => toDoCache.UpdateUi(item));
                
                if (i.IsHasError)
                {
                    return new(i.Errors);
                }
                
                if (item.Type == ToDoItemType.Reference)
                {
                    var reference = await toDoService.GetReferenceToDoItemSettingsAsync(item.Id, cancellationToken);
                    
                    if (reference.IsHasError)
                    {
                        return new(reference.Errors);
                    }
                    
                    i.Value.ReferenceId = reference.Value.ReferenceId;
                }
                else
                {
                    i.Value.ReferenceId = null;
                }
                
                var result = await List.UpdateFavoriteItemAsync(i.Value);
                
                if (result.IsHasError)
                {
                    return result;
                }
                
                await this.InvokeUiBackgroundAsync(() =>
                {
                    progressItem.Progress++;
                    
                    return Result.Success;
                });
            }
        }
        
        return Result.Success;
    }
    
    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemListsAsync(
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        bool autoOrder,
        TaskProgressItem progressItem,
        CancellationToken cancellationToken
    )
    {
        return this.InvokeUiBackgroundAsync(() => ClearExceptUi(items))
           .IfSuccessAllAsync(cancellationToken, () => RefreshFavoriteToDoItemsAsync(cancellationToken),
                () => RefreshToDoItemListsCore(items, autoOrder, progressItem, cancellationToken)
                   .ConfigureAwait(false));
    }
    
    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return List.ClearExceptUi(items);
    }
    
    private async ValueTask<Result> RefreshToDoItemListsCore(
        ReadOnlyMemory<ToDoItemEntityNotify> ids,
        bool autoOrder,
        TaskProgressItem progressItem,
        CancellationToken cancellationToken
    )
    {
        uint orderIndex = 1;
        
        await foreach (var items in toDoService
           .GetToDoItemsAsync(ids.ToArray().Select(x => x.Id).ToArray(), 5, cancellationToken)
           .ConfigureAwait(false))
        {
            if (items.IsHasError)
            {
                return new(items.Errors);
            }
            
            foreach (var item in items.Value.ToArray())
            {
                var i = await this.InvokeUiBackgroundAsync(() => toDoCache.UpdateUi(item));
                
                if (i.IsHasError)
                {
                    return new(i.Errors);
                }
                
                if (item.Type == ToDoItemType.Reference)
                {
                    var reference = await toDoService.GetReferenceToDoItemSettingsAsync(item.Id, cancellationToken);
                    
                    if (reference.IsHasError)
                    {
                        return new(reference.Errors);
                    }
                    
                    i.Value.ReferenceId = reference.Value.ReferenceId;
                }
                else
                {
                    i.Value.ReferenceId = null;
                }
                
                if (autoOrder)
                {
                    i.Value.OrderIndex = orderIndex;
                    var result = await this.InvokeUiBackgroundAsync(() => List.UpdateItemUi(i.Value));
                    
                    if (result.IsHasError)
                    {
                        return result;
                    }
                }
                else
                {
                    var result = await this.InvokeUiBackgroundAsync(() => List.UpdateItemUi(i.Value));
                    
                    if (result.IsHasError)
                    {
                        return result;
                    }
                }
                
                orderIndex++;
                
                await this.InvokeUiBackgroundAsync(() =>
                {
                    progressItem.Progress++;
                    
                    return Result.Success;
                });
            }
        }
        
        return Result.Success;
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateItemsAsync(
        ReadOnlyMemory<ToDoItemEntityNotify> ids,
        bool autoOrder,
        CancellationToken cancellationToken
    )
    {
        return taskProgressService.RunProgressAsync((ushort)ids.Length,
            item => RefreshToDoItemListsAsync(ids, autoOrder, item, cancellationToken), cancellationToken);
    }
}
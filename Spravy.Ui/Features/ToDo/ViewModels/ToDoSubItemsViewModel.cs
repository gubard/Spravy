using Spravy.Ui.Features.ToDo.Interfaces;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoSubItemsViewModel : ViewModelBase, IToDoItemOrderChanger, ITaskProgressServiceProperty
{
    private IRefresh? refreshToDoItem;
    
    [Inject]
    public required MultiToDoItemsViewModel List { get; init; }
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
    [Inject]
    public required IToDoCache ToDoCache { get; init; }
    
    [Inject]
    public required ITaskProgressService TaskProgressService { get; init; }
    
    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return refreshToDoItem.ThrowIfNull().RefreshAsync(cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> RefreshFavoriteToDoItemsAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetFavoriteToDoItemIdsAsync(cancellationToken)
           .IfSuccessAsync(
                ids => this.RunProgressAsync((ushort)ids.Length,
                    item => List.ClearFavoriteExceptAsync(ids.ToArray())
                       .IfSuccessAsync(() => ids.ToResult().IfSuccessForEach(x => ToDoCache.GetToDoItem(x)),
                            cancellationToken)
                       .IfSuccessAsync(
                            items => RefreshFavoriteToDoItemsCore(items, item, cancellationToken).ConfigureAwait(false),
                            cancellationToken), cancellationToken), cancellationToken);
    }
    
    private async ValueTask<Result> RefreshFavoriteToDoItemsCore(
        ReadOnlyMemory<ToDoItemEntityNotify> ids,
        TaskProgressItem progressItem,
        CancellationToken cancellationToken
    )
    {
        await foreach (var items in ToDoService
           .GetToDoItemsAsync(ids.ToArray().Select(x => x.Id).ToArray(), 5, cancellationToken)
           .ConfigureAwait(false))
        {
            if (items.IsHasError)
            {
                return new(items.Errors);
            }
            
            foreach (var item in items.Value.ToArray())
            {
                var i = await ToDoCache.UpdateAsync(item, cancellationToken);
                
                if (i.IsHasError)
                {
                    return new(i.Errors);
                }
                
                if (item.Type == ToDoItemType.Reference)
                {
                    var reference = await ToDoService.GetReferenceToDoItemSettingsAsync(item.Id, cancellationToken);
                    
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
                
                await this.InvokeUIBackgroundAsync(() => progressItem.Progress++);
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
        return List.ClearExceptAsync(items.ToArray().Select(x => x.Id).ToArray())
           .IfSuccessAsync(
                () => items.ToResult().IfSuccessForEachAsync(item => List.UpdateItemAsync(item), cancellationToken),
                cancellationToken)
           .IfSuccessAsync(() => RefreshFavoriteToDoItemsAsync(cancellationToken), cancellationToken)
           .IfSuccessAsync(
                () => RefreshToDoItemListsCore(items, autoOrder, progressItem, cancellationToken).ConfigureAwait(false),
                cancellationToken);
    }
    
    private async ValueTask<Result> RefreshToDoItemListsCore(
        ReadOnlyMemory<ToDoItemEntityNotify> ids,
        bool autoOrder,
        TaskProgressItem progressItem,
        CancellationToken cancellationToken
    )
    {
        uint orderIndex = 1;
        
        await foreach (var items in ToDoService
           .GetToDoItemsAsync(ids.ToArray().Select(x => x.Id).ToArray(), 5, cancellationToken)
           .ConfigureAwait(false))
        {
            if (items.IsHasError)
            {
                return new(items.Errors);
            }
            
            foreach (var item in items.Value.ToArray())
            {
                var i = await ToDoCache.UpdateAsync(item, cancellationToken);
                
                if (i.IsHasError)
                {
                    return new(i.Errors);
                }
                
                if (item.Type == ToDoItemType.Reference)
                {
                    var reference = await ToDoService.GetReferenceToDoItemSettingsAsync(item.Id, cancellationToken);
                    
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
                    var result = await List.UpdateItemAsync(i.Value);
                    
                    if (result.IsHasError)
                    {
                        return result;
                    }
                }
                else
                {
                    var result = await List.UpdateItemAsync(i.Value);
                    
                    if (result.IsHasError)
                    {
                        return result;
                    }
                }
                
                orderIndex++;
                await this.InvokeUIBackgroundAsync(() => progressItem.Progress++);
            }
        }
        
        return Result.Success;
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateItemsAsync(
        ReadOnlyMemory<ToDoItemEntityNotify> ids,
        IRefresh refresh,
        bool autoOrder,
        CancellationToken cancellationToken
    )
    {
        refreshToDoItem = refresh;
        
        return this.RunProgressAsync((ushort)ids.Length,
            item => RefreshToDoItemListsAsync(ids, autoOrder, item, cancellationToken), cancellationToken);
    }
}
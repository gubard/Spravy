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
                       .IfSuccessAsync(
                            () => RefreshFavoriteToDoItemsCore(ids, item, cancellationToken).ConfigureAwait(false),
                            cancellationToken), cancellationToken), cancellationToken);
    }
    
    private async ValueTask<Result> RefreshFavoriteToDoItemsCore(
        ReadOnlyMemory<Guid> ids,
        TaskProgressItem progressItem,
        CancellationToken cancellationToken
    )
    {
        await foreach (var items in ToDoService.GetToDoItemsAsync(ids.ToArray(), 5, cancellationToken)
           .ConfigureAwait(false))
        {
            if (items.IsHasError)
            {
                return new(items.Errors);
            }
            
            foreach (var item in items.Value.ToArray())
            {
                Guid? referenceId = null;
                
                if (item.Type == ToDoItemType.Reference)
                {
                    var reference = await ToDoService.GetReferenceToDoItemSettingsAsync(item.Id, cancellationToken);
                    
                    if (reference.IsHasError)
                    {
                        return new(reference.Errors);
                    }
                    
                    referenceId = reference.Value.ReferenceId;
                }
                
                var result = await List.UpdateFavoriteItemAsync(item, referenceId);
                
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
        ReadOnlyMemory<Guid> ids,
        bool autoOrder,
        TaskProgressItem progressItem,
        CancellationToken cancellationToken
    )
    {
        return List.ClearExceptAsync(ids)
           .IfSuccessAsync(() => RefreshFavoriteToDoItemsAsync(cancellationToken), cancellationToken)
           .IfSuccessAsync(
                () => RefreshToDoItemListsCore(ids, autoOrder, progressItem, cancellationToken).ConfigureAwait(false),
                cancellationToken);
    }
    
    private async ValueTask<Result> RefreshToDoItemListsCore(
        ReadOnlyMemory<Guid> ids,
        bool autoOrder,
        TaskProgressItem progressItem,
        CancellationToken cancellationToken
    )
    {
        uint orderIndex = 1;
        
        await foreach (var items in ToDoService.GetToDoItemsAsync(ids, 5, cancellationToken).ConfigureAwait(false))
        {
            if (items.IsHasError)
            {
                return new(items.Errors);
            }
            
            foreach (var item in items.Value.ToArray())
            {
                Guid? referenceId = null;
                
                if (item.Type == ToDoItemType.Reference)
                {
                    var reference = await ToDoService.GetReferenceToDoItemSettingsAsync(item.Id, cancellationToken);
                    
                    if (reference.IsHasError)
                    {
                        return new(reference.Errors);
                    }
                    
                    referenceId = reference.Value.ReferenceId;
                }
                
                if (autoOrder)
                {
                    var result = await List.UpdateItemAsync(item.WithOrderIndex(orderIndex), referenceId);
                    
                    if (result.IsHasError)
                    {
                        return result;
                    }
                }
                else
                {
                    var result = await List.UpdateItemAsync(item, referenceId);
                    
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
        ReadOnlyMemory<Guid> ids,
        IRefresh refresh,
        bool autoOrder,
        CancellationToken cancellationToken
    )
    {
        refreshToDoItem = refresh;
        
        return this.RunProgressAsync((ushort)ids.Length,
            item => ids.ToResult()
               .IfSuccessForEachAsync(
                    id => ToDoCache.GetToDoItem(id)
                       .IfSuccessAsync(toDoItem => List.UpdateItemAsync(toDoItem, null), cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(() => RefreshToDoItemListsAsync(ids, autoOrder, item, cancellationToken), cancellationToken), cancellationToken);
    }
}
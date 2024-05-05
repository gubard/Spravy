namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoSubItemsViewModel : ViewModelBase, IToDoItemOrderChanger, ITaskProgressServiceProperty
{
    private readonly MultiToDoItemsViewModel list;
    private IRefresh? refreshToDoItem;
    
    [Inject]
    public required MultiToDoItemsViewModel List
    {
        get => list;
        [MemberNotNull(nameof(list))]
        init
        {
            list?.Dispose();
            list = value;
            Disposables.Add(list);
        }
    }
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
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
                ids => this.RunProgressAsync(ids.Length,
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
                var result = await List.UpdateFavoriteItemAsync(item);
                
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
        Guid[] ids,
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
        Guid[] ids,
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
                if (autoOrder)
                {
                    var result = await List.UpdateItemAsync(item.WithOrderIndex(orderIndex));
                    
                    if (result.IsHasError)
                    {
                        return result;
                    }
                }
                else
                {
                    var result = await List.UpdateItemAsync(item);
                    
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
        Guid[] ids,
        IRefresh refresh,
        bool autoOrder,
        CancellationToken cancellationToken
    )
    {
        refreshToDoItem = refresh;
        
        return this.RunProgressAsync(ids.Length,
            item => RefreshToDoItemListsAsync(ids, autoOrder, item, cancellationToken), cancellationToken);
    }
}
using Spravy.Ui.Features.ToDo.Interfaces;

namespace Spravy.Ui.Features.ToDo.Services;

public class ToDoCache : IToDoCache
{
    private readonly Dictionary<Guid, ToDoItemEntityNotify> cache;
    private readonly IToDoService toDoService;
    private readonly IUiApplicationService uiApplicationService;
    private readonly IErrorHandler errorHandler;
    
    public ToDoCache(IToDoService toDoService, IUiApplicationService uiApplicationService, IErrorHandler errorHandler)
    {
        this.toDoService = toDoService;
        this.uiApplicationService = uiApplicationService;
        this.errorHandler = errorHandler;
        cache = new();
    }
    
    public Result<ToDoItemEntityNotify> GetToDoItem(Guid id)
    {
        if (cache.TryGetValue(id, out var value))
        {
            return value.ToResult();
        }
        
        var result = new ToDoItemEntityNotify(id, toDoService, uiApplicationService, errorHandler);
        cache.Add(id, result);
        
        return result.ToResult();
    }
    
    public ConfiguredValueTaskAwaitable<Result<ToDoItemEntityNotify>> UpdateAsync(
        ToDoItem toDoItem,
        CancellationToken cancellationToken
    )
    {
        return GetToDoItem(toDoItem.Id)
           .IfSuccessAsync(item =>
            {
                if (toDoItem.Active.HasValue)
                {
                    return UpdateAsync(toDoItem.Active.Value, cancellationToken)
                       .IfSuccessAsync(
                            () => GetToDoItem(toDoItem.Active.Value.Id)
                               .IfSuccessAsync(i => this.InvokeUIBackgroundAsync(() => item.Active = i),
                                    cancellationToken),
                            cancellationToken)
                       .IfSuccessAsync(() => UpdatePropertiesAsync(item, toDoItem, cancellationToken), cancellationToken);
                }
                
                return UpdatePropertiesAsync(item, toDoItem, cancellationToken);
            }, cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result<ToDoItemEntityNotify>> UpdatePropertiesAsync(
        ToDoItemEntityNotify item,
        ToDoItem toDoItem,
        CancellationToken cancellationToken
    )
    {
        return this.InvokeUIBackgroundAsync(() =>
            {
                item.Description = toDoItem.Description;
                item.DescriptionType = toDoItem.DescriptionType;
                item.Type = toDoItem.Type;
                item.Name = toDoItem.Name;
                item.Link = toDoItem.Link?.AbsoluteUri ?? string.Empty;
                item.Status = toDoItem.Status;
                item.IsCan = toDoItem.IsCan;
                item.IsFavorite = toDoItem.IsFavorite;
                item.OrderIndex = toDoItem.OrderIndex;
                item.ParentId = toDoItem.ParentId;
            })
           .IfSuccessAsync(item.ToResult, cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateAsync(
        Guid id,
        ReadOnlyMemory<ToDoShortItem> parents,
        CancellationToken cancellationToken
    )
    {
        return GetToDoItem(id)
           .IfSuccessAsync(
                item => parents.ToResult()
                   .IfSuccessForEachAsync(
                        p => UpdateAsync(p, cancellationToken)
                           .IfSuccessAsync(() => GetToDoItem(p.Id), cancellationToken), cancellationToken)
                   .IfSuccessAsync(
                        ps => this.InvokeUIBackgroundAsync(() =>
                            item.Path = RootItem.Default.As<object>().ToEnumerable().Concat(ps.ToArray()).ToArray()!),
                        cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateAsync(
        ToDoShortItem shortItem,
        CancellationToken cancellationToken
    )
    {
        return GetToDoItem(shortItem.Id)
           .IfSuccessAsync(item => this.InvokeUIBackgroundAsync(() => item.Name = shortItem.Name), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateAsync(ActiveToDoItem active, CancellationToken cancellationToken)
    {
        return GetToDoItem(active.Id)
           .IfSuccessAsync(item => this.InvokeUIBackgroundAsync(() => item.Name = active.Name), cancellationToken);
    }
}
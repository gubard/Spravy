using Spravy.Ui.Features.ToDo.Interfaces;

namespace Spravy.Ui.Features.ToDo.Services;

public class ToDoCache : IToDoCache
{
    private readonly Dictionary<Guid, ToDoItemEntityNotify> cache;
    private readonly Dictionary<Guid, ActiveToDoItemNotify> activeCache;
    private readonly IToDoService toDoService;
    private readonly IUiApplicationService uiApplicationService;
    private readonly IErrorHandler errorHandler;
    private readonly IClipboardService clipboardService;
    private readonly IConverter converter;
    private readonly INavigator navigator;
    private readonly IDialogViewer dialogViewer;
    private readonly IOpenerLink openerLink;
    private ReadOnlyMemory<ToDoItemEntityNotify> rootItems = ReadOnlyMemory<ToDoItemEntityNotify>.Empty;
    
    public ToDoCache(
        IToDoService toDoService,
        IUiApplicationService uiApplicationService,
        IErrorHandler errorHandler,
        IClipboardService clipboardService,
        IConverter converter,
        INavigator navigator,
        IOpenerLink openerLink,
        IDialogViewer dialogViewer
    )
    {
        activeCache = new();
        this.toDoService = toDoService;
        this.uiApplicationService = uiApplicationService;
        this.errorHandler = errorHandler;
        this.clipboardService = clipboardService;
        this.converter = converter;
        this.navigator = navigator;
        this.openerLink = openerLink;
        this.dialogViewer = dialogViewer;
        cache = new();
    }
    
    public Result<ActiveToDoItemNotify> GetActive(Guid id)
    {
        if (activeCache.TryGetValue(id, out var value))
        {
            return value.ToResult();
        }
        
        var result = new ActiveToDoItemNotify(id, navigator, errorHandler);
        activeCache.Add(id, result);
        
        return result.ToResult();
    }
    
    public Result<ToDoItemEntityNotify> GetToDoItem(Guid id)
    {
        if (cache.TryGetValue(id, out var value))
        {
            return value.ToResult();
        }
        
        var result = new ToDoItemEntityNotify(id, toDoService, navigator, uiApplicationService, dialogViewer,
            converter, clipboardService, openerLink, errorHandler);
        
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
                       .IfSuccessAsync(i => this.InvokeUiBackgroundAsync(() => item.Active = i), cancellationToken)
                       .IfSuccessAsync(() => UpdatePropertiesAsync(item, toDoItem, cancellationToken),
                            cancellationToken);
                }
                
                return this.InvokeUiBackgroundAsync(() => item.Active = null)
                   .IfSuccessAsync(() => UpdatePropertiesAsync(item, toDoItem, cancellationToken), cancellationToken);
            }, cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result<ToDoItemEntityNotify>> UpdatePropertiesAsync(
        ToDoItemEntityNotify item,
        ToDoItem toDoItem,
        CancellationToken cancellationToken
    )
    {
        return this.InvokeUiBackgroundAsync(() =>
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
            })
           .IfSuccessAsync(() =>
            {
                if (toDoItem.ParentId.HasValue)
                {
                    return GetToDoItem(toDoItem.ParentId.Value);
                }
                
                return new((ToDoItemEntityNotify)null);
            }, cancellationToken)
           .IfSuccessAsync(parent => this.InvokeUiBackgroundAsync(() => item.Parent = parent), cancellationToken)
           .IfSuccessAsync(item.UpdateCommandsAsync, cancellationToken)
           .IfSuccessAsync(item.ToResult, cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateParentsAsync(
        Guid id,
        ReadOnlyMemory<ToDoShortItem> parents,
        CancellationToken cancellationToken
    )
    {
        return GetToDoItem(id)
           .IfSuccessAsync(
                item => parents.ToResult()
                   .IfSuccessForEachAsync(p => UpdateAsync(p, cancellationToken), cancellationToken)
                   .IfSuccessAsync(
                        ps => this.InvokeUiBackgroundAsync(() =>
                            item.Path = RootItem.Default.As<object>().ToEnumerable().Concat(ps.ToArray()).ToArray()!),
                        cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoItemEntityNotify>>> UpdateAsync(
        ReadOnlyMemory<ToDoSelectorItem> items,
        CancellationToken cancellationToken
    )
    {
        return UpdateRootItems(items.ToArray().Select(x => x.Id).ToArray())
           .IfSuccessAsync(
                _ => items.ToResult()
                   .IfSuccessForEachAsync(item => UpdateAsync(item, cancellationToken), cancellationToken),
                cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<ToDoItemEntityNotify>> UpdateAsync(
        ToDoSelectorItem item,
        CancellationToken cancellationToken
    )
    {
        return GetToDoItem(item.Id)
           .IfSuccessAsync(
                x => this.InvokeUiBackgroundAsync(() => x.Name = item.Name)
                   .IfSuccessAsync(
                        () => item.Children
                           .ToResult()
                           .IfSuccessForEachAsync(y => UpdateAsync(y, cancellationToken), cancellationToken)
                           .IfSuccessForEachAsync(y => y.Id.ToResult(), cancellationToken)
                           .IfSuccessAsync(children => UpdateChildrenItemsAsync(x.Id, children, cancellationToken),
                                cancellationToken)
                           .IfSuccessAsync(_ => x.ToResult(), cancellationToken), cancellationToken),
                cancellationToken);
    }
    
    public Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateRootItems(ReadOnlyMemory<Guid> roots)
    {
        return roots.ToResult()
           .IfSuccessForEach(GetToDoItem)
           .IfSuccess(items =>
            {
                rootItems = items.ToArray().OrderBy(x => x.OrderIndex).ToArray();
                
                return rootItems.ToResult();
            });
    }
    
    public Result<ReadOnlyMemory<ToDoItemEntityNotify>> GetRootItems()
    {
        return rootItems.ToResult();
    }
    
    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoItemEntityNotify>>> UpdateChildrenItemsAsync(
        Guid id,
        ReadOnlyMemory<Guid> items,
        CancellationToken cancellationToken
    )
    {
        return items.ToResult()
           .IfSuccessForEach(GetToDoItem)
           .IfSuccessAsync(children => GetToDoItem(id)
               .IfSuccessAsync(item => this.InvokeUiBackgroundAsync(() =>
                {
                    item.Children.Clear();
                    item.Children.AddRange(children.ToArray().OrderBy(x => x.OrderIndex));
                }), cancellationToken)
               .IfSuccessAsync(() => children.ToResult(), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<ToDoItemEntityNotify>> UpdateAsync(
        ToDoShortItem shortItem,
        CancellationToken cancellationToken
    )
    {
        return GetToDoItem(shortItem.Id)
           .IfSuccessAsync(item => this.InvokeUiBackgroundAsync(() =>
                {
                    item.IsExpanded = false;
                    item.IsIgnore = false;
                    item.Name = shortItem.Name;
                })
               .IfSuccessAsync(item.ToResult, cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<ActiveToDoItemNotify>> UpdateAsync(
        ActiveToDoItem active,
        CancellationToken cancellationToken
    )
    {
        return GetActive(active.Id)
           .IfSuccessAsync(
                item => this.InvokeUiBackgroundAsync(() => item.Name = active.Name)
                   .IfSuccessAsync(item.ToResult, cancellationToken), cancellationToken);
    }
}
using Spravy.Ui.Features.ToDo.Interfaces;

namespace Spravy.Ui.Features.ToDo.Services;

public class ToDoCache : IToDoCache
{
    private readonly Dictionary<Guid, ToDoItemNotify> toDoItemCache;
    private readonly Dictionary<Guid, ActiveToDoItemNotify> activeToDoItemCache;
    private readonly Dictionary<Guid, ToDoItemParentNotify> toDoItemParentCache;
    private readonly Dictionary<Guid, ToDoSelectorItemNotify> toDoSelectorItemCache;
    private readonly Dictionary<Guid, ToDoShortItemNotify> toDoShortItemCache;
    private readonly Dictionary<Guid, ReadOnlyMemory<object>> toDoItemParentsCache;
    
    public ToDoCache()
    {
        toDoItemCache = new();
        activeToDoItemCache = new();
        toDoItemParentCache = new();
        toDoSelectorItemCache = new();
        toDoShortItemCache = new();
        toDoItemParentsCache = new();
    }
    
    public ToDoShortItemNotify GetToDoShortItem(Guid id)
    {
        if (toDoShortItemCache.TryGetValue(id, out var value))
        {
            return value;
        }
        
        var result = new ToDoShortItemNotify
        {
            Id = id,
            Name = "Loading...",
        };
        
        toDoShortItemCache.Add(id, result);
        
        return result;
    }
    
    public ToDoSelectorItemNotify GetToDoSelectorItem(Guid id)
    {
        if (toDoSelectorItemCache.TryGetValue(id, out var value))
        {
            value.IsExpanded = false;
            
            return value;
        }
        
        var result = new ToDoSelectorItemNotify
        {
            Id = id,
            Name = "Loading...",
        };
        
        toDoSelectorItemCache.Add(id, result);
        
        return result;
    }
    
    public ToDoItemParentNotify GetToDoItemParent(Guid id)
    {
        if (toDoItemParentCache.TryGetValue(id, out var value))
        {
            return value;
        }
        
        var result = new ToDoItemParentNotify
        {
            Id = id,
            Name = "Loading...",
        };
        
        toDoItemParentCache.Add(id, result);
        
        return result;
    }
    
    public ActiveToDoItemNotify GetActiveToDoItem(Guid id)
    {
        if (activeToDoItemCache.TryGetValue(id, out var value))
        {
            return value;
        }
        
        var result = new ActiveToDoItemNotify
        {
            Id = id,
            Name = "Loading...",
        };
        
        activeToDoItemCache.Add(id, result);
        
        return result;
    }
    
    public Result<ReadOnlyMemory<object>> GetToDoItemParents(Guid id)
    {
        if (toDoItemParentsCache.TryGetValue(id, out var value))
        {
            return value.ToResult();
        }
        
        var result = new ReadOnlyMemory<object>([RootItem.Default, GetToDoItemParent(id),]);
        toDoItemParentsCache.Add(id, result);
        
        return result.ToResult();
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateParentsAsync(
        Guid id,
        ReadOnlyMemory<ToDoShortItem> parents,
        CancellationToken cancellationToken
    )
    {
        toDoItemParentsCache[id] = new(RootItem.Default
           .As<object>()
           .ToEnumerable()
           .Concat(parents.ToArray().Select(x => GetToDoItemParent(x.Id)))
           .ToArray()!);
        
        return parents.ToResult().IfSuccessForEachAsync(UpdateAsync, cancellationToken);
    }
    
    public ToDoItemNotify GetToDoItem(Guid id)
    {
        if (toDoItemCache.TryGetValue(id, out var value))
        {
            return value;
        }
        
        var result = new ToDoItemNotify
        {
            Id = id,
            Name = "Loading...",
            OrderIndex = uint.MaxValue,
            Status = ToDoItemStatus.ReadyForComplete,
        };
        
        toDoItemCache.Add(id, result);
        
        return result;
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateAsync(ToDoShortItem toDoItem)
    {
        var shortItem = GetToDoShortItem(toDoItem.Id);
        var selector = GetToDoSelectorItem(toDoItem.Id);
        var parent = GetToDoItemParent(toDoItem.Id);
        var active = GetActiveToDoItem(toDoItem.Id);
        var item = GetToDoItem(toDoItem.Id);
        
        return this.InvokeUIBackgroundAsync(() =>
        {
            shortItem.Name = toDoItem.Name;
            selector.Name = toDoItem.Name;
            parent.Name = toDoItem.Name;
            active.Name = toDoItem.Name;
            item.Name = toDoItem.Name;
        });
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateAsync(ToDoItem toDoItem)
    {
        var shortItem = GetToDoShortItem(toDoItem.Id);
        var selector = GetToDoSelectorItem(toDoItem.Id);
        var parent = GetToDoItemParent(toDoItem.Id);
        var active = GetActiveToDoItem(toDoItem.Id);
        var item = GetToDoItem(toDoItem.Id);
        
        return this.InvokeUIBackgroundAsync(() =>
        {
            ActiveToDoItemNotify? itemActive = null;
            
            if (toDoItem.Active.HasValue)
            {
                itemActive = GetActiveToDoItem(toDoItem.Active.Value.Id);
                itemActive.Name = toDoItem.Active.Value.Name;
            }
            
            shortItem.Name = toDoItem.Name;
            selector.Name = toDoItem.Name;
            parent.Name = toDoItem.Name;
            active.Name = toDoItem.Name;
            item.Name = toDoItem.Name;
            item.OrderIndex = toDoItem.OrderIndex;
            item.Description = toDoItem.Description;
            item.Link = toDoItem.Link?.AbsoluteUri ?? string.Empty;
            item.IsCan = toDoItem.IsCan;
            item.IsFavorite = toDoItem.IsFavorite;
            item.Status = toDoItem.Status;
            item.ParentId = toDoItem.ParentId;
            item.Type = toDoItem.Type;
            item.Active = itemActive;
        });
    }
}
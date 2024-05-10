using Spravy.Ui.Features.ToDo.Interfaces;

namespace Spravy.Ui.Features.ToDo.Services;

public class ToDoCache : IToDoCache
{
    private readonly Dictionary<Guid, ToDoItemNotify> toDoItemCache;
    private readonly Dictionary<Guid, ActiveToDoItemNotify> activeToDoItemCache;
    private readonly Dictionary<Guid, ToDoItemParentNotify> toDoItemParentCache;
    private readonly Dictionary<Guid, ToDoSelectorItemNotify> toDoSelectorItemCache;
    private readonly Dictionary<Guid, ToDoShortItemNotify> toDoShortItemCache;
    
    public ToDoCache()
    {
        toDoItemCache = new();
        activeToDoItemCache = new();
        toDoItemParentCache = new();
        toDoSelectorItemCache = new();
        toDoShortItemCache = new();
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
    
    public void Update(ToDoItem toDoItem)
    {
        ActiveToDoItemNotify? itemActive = null;
        
        if (toDoItem.Active.HasValue)
        {
            itemActive = GetActiveToDoItem(toDoItem.Active.Value.Id);
            itemActive.Name = toDoItem.Active.Value.Name;
        }
        
        var shortItem = GetToDoShortItem(toDoItem.Id);
        shortItem.Name = toDoItem.Name;
        var selector = GetToDoSelectorItem(toDoItem.Id);
        selector.Name = toDoItem.Name;
        var parent = GetToDoItemParent(toDoItem.Id);
        parent.Name = toDoItem.Name;
        var active = GetActiveToDoItem(toDoItem.Id);
        active.Name = toDoItem.Name;
        var item = GetToDoItem(toDoItem.Id);
        item.Name = toDoItem.Name;
        item.OrderIndex = toDoItem.OrderIndex;
        item.Description = item.Description;
        item.Link = item.Link;
        item.IsCan = item.IsCan;
        item.IsFavorite = item.IsFavorite;
        item.Status = toDoItem.Status;
        item.ParentId = toDoItem.ParentId;
        item.Type = toDoItem.Type;
        item.Active = itemActive;
    }
}
namespace Spravy.Ui.Features.ToDo.Services;

public class ToDoCache : IToDoCache
{
    private readonly Dictionary<Guid, ToDoItemEntityNotify> cache;
    private readonly SpravyCommandNotifyService spravyCommandNotifyService;
    private ReadOnlyMemory<ToDoItemEntityNotify> rootItems = ReadOnlyMemory<ToDoItemEntityNotify>.Empty;

    public ToDoCache(SpravyCommandNotifyService spravyCommandNotifyService)
    {
        this.spravyCommandNotifyService = spravyCommandNotifyService;
        cache = new();
    }

    public Result<ToDoItemEntityNotify> GetToDoItem(Guid id)
    {
        if (cache.TryGetValue(id, out var value))
        {
            return value.ToResult();
        }

        var result = new ToDoItemEntityNotify(id, spravyCommandNotifyService);

        if (cache.TryAdd(id, result))
        {
            return result.ToResult();
        }

        return cache[id].ToResult();
    }

    public Result<ToDoItemEntityNotify> UpdateUi(ToDoItem toDoItem)
    {
        return GetToDoItem(toDoItem.Id)
           .IfSuccess(item =>
            {
                if (toDoItem.Active.TryGetValue(out var value))
                {
                    return UpdateUi(value)
                       .IfSuccess(i =>
                        {
                            item.Active = i;

                            return Result.Success;
                        })
                       .IfSuccess(() => UpdatePropertiesUi(item, toDoItem));
                }

                item.Active = null;

                return UpdatePropertiesUi(item, toDoItem);
            });
    }

    public Result<ToDoItemEntityNotify> UpdateUi(ActiveToDoItem activeToDoItem)
    {
        return GetToDoItem(activeToDoItem.Id)
           .IfSuccess(item =>
            {
                item.Name = activeToDoItem.Name;

                if (activeToDoItem.ParentId.TryGetValue(out var parentId))
                {
                    return GetToDoItem(parentId)
                       .IfSuccess(i =>
                        {
                            item.Parent = i;

                            return Result.Success;
                        })
                       .IfSuccess(item.ToResult);
                }

                return item.ToResult();
            });
    }

    private Result<ToDoItemEntityNotify> UpdatePropertiesUi(ToDoItemEntityNotify item, ToDoItem toDoItem)
    {
        item.Description = toDoItem.Description;
        item.DescriptionType = toDoItem.DescriptionType;
        item.Type = toDoItem.Type;
        item.Name = toDoItem.Name;
        item.Link = toDoItem.Link.TryGetValue(out var uri) ? uri.AbsoluteUri : string.Empty;
        item.Status = toDoItem.Status;
        item.IsCan = toDoItem.IsCan;
        item.IsFavorite = toDoItem.IsFavorite;
        item.OrderIndex = toDoItem.OrderIndex;

        if (toDoItem.ParentId.TryGetValue(out var value))
        {
            var parent = GetToDoItem(value);

            if (parent.IsHasError)
            {
                return parent;
            }

            item.Parent = parent.Value;
        }
        else
        {
            item.Parent = null;
        }

        return item.UpdateCommandsUi();
    }

    public Result UpdateParentsUi(Guid id, ReadOnlyMemory<ToDoShortItem> parents)
    {
        return GetToDoItem(id)
           .IfSuccess(item => parents.ToResult()
               .IfSuccessForEach(UpdateUi)
               .IfSuccess(ps =>
                {
                    item.Path = RootItem.Default.As<object>().ToEnumerable().Concat(ps.ToArray()).ToArray()!;

                    return Result.Success;
                }));
    }

    public Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateUi(ReadOnlyMemory<ToDoSelectorItem> items)
    {
        return UpdateRootItems(items.ToArray().Select(x => x.Id).ToArray())
           .IfSuccess(_ => items.ToResult().IfSuccessForEach(UpdateUi));
    }

    public Result<ToDoItemEntityNotify> UpdateUi(ToDoSelectorItem item)
    {
        return GetToDoItem(item.Id)
           .IfSuccess(x =>
            {
                x.Name = item.Name;

                return item.Children
                   .ToResult()
                   .IfSuccessForEach(UpdateUi)
                   .IfSuccessForEach(y => y.Id.ToResult())
                   .IfSuccess(children => UpdateChildrenItemsUi(x.Id, children))
                   .IfSuccess(_ => x.ToResult());
            });
    }

    public Result ResetItemsUi()
    {
        foreach (var value in cache.Values)
        {
            value.IsExpanded = false;
            value.IsIgnore = false;
        }

        return Result.Success;
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

    public Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateChildrenItemsUi(Guid id, ReadOnlyMemory<Guid> items)
    {
        return items.ToResult()
           .IfSuccessForEach(GetToDoItem)
           .IfSuccess(children => GetToDoItem(id)
               .IfSuccess(item =>
                {
                    item.Children.Clear();
                    item.Children.AddRange(children.ToArray().OrderBy(x => x.OrderIndex));

                    return Result.Success;
                })
               .IfSuccess(() => children.ToResult()));
    }

    public Result<ToDoItemEntityNotify> UpdateUi(ToDoShortItem shortItem)
    {
        return GetToDoItem(shortItem.Id)
           .IfSuccess(item =>
            {
                item.Name = shortItem.Name;

                return item.ToResult();
            });
    }
}
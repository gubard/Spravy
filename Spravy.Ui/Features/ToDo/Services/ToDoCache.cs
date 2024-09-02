namespace Spravy.Ui.Features.ToDo.Services;

public class ToDoCache : IToDoCache
{
    private readonly Dictionary<Guid, ToDoItemEntityNotify> cache;
    private readonly IServiceFactory serviceFactory;

    private ReadOnlyMemory<ToDoItemEntityNotify> rootItems =
        ReadOnlyMemory<ToDoItemEntityNotify>.Empty;

    public ToDoCache(IServiceFactory serviceFactory)
    {
        this.serviceFactory = serviceFactory;
        cache = new();
    }

    public Result<ToDoItemEntityNotify> GetToDoItem(Guid id)
    {
        if (cache.TryGetValue(id, out var value))
        {
            return value.ToResult();
        }

        var result = new ToDoItemEntityNotify(
            id,
            serviceFactory.CreateService<SpravyCommandNotifyService>()
        );

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

    public Result<ToDoItemEntityNotify> UpdateUi(FullToDoItem toDoItem)
    {
        return GetToDoItem(toDoItem.Id)
            .IfSuccess(item =>
            {
                item.ChildrenType = toDoItem.ChildrenType;
                item.DueDate = toDoItem.DueDate;
                item.MonthsOffset = toDoItem.MonthsOffset;
                item.YearsOffset = toDoItem.YearsOffset;
                item.DaysOffset = toDoItem.DaysOffset;
                item.WeeksOffset = toDoItem.WeeksOffset;
                item.IsRequiredCompleteInDueDate = toDoItem.IsRequiredCompleteInDueDate;
                item.TypeOfPeriodicity = toDoItem.TypeOfPeriodicity;
                item.DaysOfWeek.UpdateUi(toDoItem.WeeklyDays);
                item.DaysOfYear.UpdateUi(toDoItem.AnnuallyDays);
                item.DaysOfMonth.UpdateUi(toDoItem.MonthlyDays.Select(x => (int)x));
                item.Description = toDoItem.Description;
                item.DescriptionType = toDoItem.DescriptionType;
                item.Type = toDoItem.Type;
                item.Name = toDoItem.Name;
                item.Link = toDoItem.Link.TryGetValue(out var uri) ? uri.AbsoluteUri : string.Empty;
                item.Status = toDoItem.Status;
                item.IsCan = toDoItem.IsCan;
                item.IsFavorite = toDoItem.IsFavorite;
                item.OrderIndex = toDoItem.OrderIndex;
                item.ReferenceId = toDoItem.ReferenceId.GetValueOrNull();
                item.IsIgnore = item.Type == ToDoItemType.Reference;

                if (toDoItem.Active.TryGetValue(out var v))
                {
                    var result = UpdateUi(v)
                        .IfSuccess(i =>
                        {
                            item.Active = i;

                            return Result.Success;
                        });

                    if (result.IsHasError)
                    {
                        return new(result.Errors);
                    }
                }
                else
                {
                    item.Active = null;
                }

                if (toDoItem.ParentId.TryGetValue(out var value))
                {
                    var parent = GetToDoItem(value);

                    if (parent.TryGetValue(out var p))
                    {
                        item.Parent = p;
                    }
                    else
                    {
                        return parent;
                    }
                }
                else
                {
                    item.Parent = null;
                }

                return item.UpdateCommandsUi();
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

    private Result<ToDoItemEntityNotify> UpdatePropertiesUi(
        ToDoItemEntityNotify item,
        ToDoItem toDoItem
    )
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
        item.ReferenceId = toDoItem.ReferenceId.GetValueOrNull();
        item.IsIgnore = item.Type == ToDoItemType.Reference;

        if (toDoItem.ParentId.TryGetValue(out var value))
        {
            var parent = GetToDoItem(value);

            if (!parent.TryGetValue(out var p))
            {
                return parent;
            }

            item.Parent = p;
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
            .IfSuccess(item =>
                parents
                    .ToResult()
                    .IfSuccessForEach(UpdateUi)
                    .IfSuccess(ps =>
                    {
                        item.Path = RootItem
                            .DefaultObject.ToReadOnlyMemory()
                            .Combine(ps.Select(x => (object)x))
                            .ToArray();

                        return Result.Success;
                    })
            );
    }

    public Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateUi(
        ReadOnlyMemory<ToDoSelectorItem> items
    )
    {
        return UpdateRootItems(items.Select(x => x.Id))
            .IfSuccess(_ => items.ToResult().IfSuccessForEach(UpdateUi));
    }

    public Result<ToDoItemEntityNotify> UpdateUi(ToDoSelectorItem item)
    {
        return GetToDoItem(item.Id)
            .IfSuccess(x =>
            {
                x.Name = item.Name;
                x.OrderIndex = item.OrderIndex;

                return item
                    .Children.ToResult()
                    .IfSuccessForEach(UpdateUi)
                    .IfSuccessForEach(y =>
                    {
                        y.Parent = x;

                        return y.ToResult();
                    })
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
            value.IsIgnore = value.Type == ToDoItemType.Reference;
        }

        return Result.Success;
    }

    public Result IgnoreItemsUi(ReadOnlyMemory<Guid> ids)
    {
        foreach (var value in cache.Values)
        {
            if (ids.Contains(value.Id))
            {
                value.IsIgnore = true;
            }
        }

        return Result.Success;
    }

    public Result ExpandItemUi(Guid id)
    {
        return GetToDoItem(id)
            .IfSuccess(item =>
            {
                item.IsExpanded = true;

                if (item.Parent is null)
                {
                    return Result.Success;
                }

                return ExpandItemUi(item.Parent.Id);
            });
    }

    public Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateRootItems(ReadOnlyMemory<Guid> roots)
    {
        return roots
            .ToResult()
            .IfSuccessForEach(GetToDoItem)
            .IfSuccess(items =>
            {
                rootItems = items.OrderBy(x => x.OrderIndex);

                return rootItems.ToResult();
            });
    }

    public Result<ReadOnlyMemory<ToDoItemEntityNotify>> GetRootItems()
    {
        return rootItems.ToResult();
    }

    public Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateChildrenItemsUi(
        Guid id,
        ReadOnlyMemory<Guid> items
    )
    {
        return items
            .ToResult()
            .IfSuccessForEach(GetToDoItem)
            .IfSuccess(children =>
                GetToDoItem(id)
                    .IfSuccess(item =>
                    {
                        item.Children.RemoveAll(
                            item.Children.Where(x => !items.Span.Contains(x.Id))
                        );

                        var currentChildrenIds = item
                            .Children.Select(x => x.Id)
                            .ToArray()
                            .ToReadOnlyMemory();

                        item.Children.AddRange(
                            children.Where(x => !currentChildrenIds.Span.Contains(x.Id)).ToArray()
                        );

                        item.Children.BinarySortByOrderIndex();

                        return Result.Success;
                    })
                    .IfSuccess(() => children.ToResult())
            );
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

using System.Collections.Concurrent;

namespace Spravy.Ui.Features.ToDo.Services;

public class ToDoCache : IToDoCache
{
    private readonly ConcurrentDictionary<Guid, ToDoItemEntityNotify> cache;
    private readonly IServiceFactory serviceFactory;

    private ReadOnlyMemory<ToDoItemEntityNotify> rootItems = ReadOnlyMemory<ToDoItemEntityNotify>.Empty;

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

        var result = new ToDoItemEntityNotify(id, serviceFactory.CreateService<SpravyCommandNotifyService>());

        if (cache.TryAdd(id, result))
        {
            return result.ToResult();
        }

        return cache[id].ToResult();
    }

    public Result<ToDoItemEntityNotify> UpdateUi(FullToDoItem toDoItem)
    {
        return GetToDoItem(toDoItem.Item.Id)
           .IfSuccess(
                item =>
                {
                    item.ChildrenType = toDoItem.Item.ChildrenType;
                    item.DueDate = toDoItem.Item.DueDate;
                    item.MonthsOffset = toDoItem.Item.MonthsOffset;
                    item.YearsOffset = toDoItem.Item.YearsOffset;
                    item.DaysOffset = toDoItem.Item.DaysOffset;
                    item.WeeksOffset = toDoItem.Item.WeeksOffset;
                    item.IsRequiredCompleteInDueDate = toDoItem.Item.IsRequiredCompleteInDueDate;
                    item.TypeOfPeriodicity = toDoItem.Item.TypeOfPeriodicity;
                    item.WeeklyDays.UpdateUi(toDoItem.Item.WeeklyDays);
                    item.AnnuallyDays.UpdateUi(toDoItem.Item.AnnuallyDays);
                    item.MonthlyDays.UpdateUi(toDoItem.Item.MonthlyDays.Select(x => (int)x));
                    item.Description = toDoItem.Item.Description;
                    item.DescriptionType = toDoItem.Item.DescriptionType;
                    item.Type = toDoItem.Item.Type;
                    item.Name = toDoItem.Item.Name;
                    item.Link = toDoItem.Item.Link.TryGetValue(out var uri) ? uri.AbsoluteUri : string.Empty;
                    item.Status = toDoItem.Status;
                    item.IsCan = toDoItem.IsCan;
                    item.IsFavorite = toDoItem.Item.IsFavorite;
                    item.OrderIndex = toDoItem.Item.OrderIndex;
                    item.IsBookmark = toDoItem.Item.IsBookmark;
                    item.Icon = toDoItem.Item.Icon;
                    item.IsUpdated = true;

                    item.Color = toDoItem.Item.Color.IsNullOrWhiteSpace()
                        ? Colors.Transparent
                        : Color.Parse(toDoItem.Item.Color);

                    if (toDoItem.Active.TryGetValue(out var v))
                    {
                        var result = UpdateUi(v)
                           .IfSuccess(
                                i =>
                                {
                                    item.Active = i;

                                    return Result.Success;
                                }
                            );

                        if (result.IsHasError)
                        {
                            return new(result.Errors);
                        }
                    }
                    else
                    {
                        item.Active = null;
                    }

                    if (toDoItem.Item.ReferenceId.TryGetValue(out var referenceId))
                    {
                        var reference = GetToDoItem(referenceId);

                        if (reference.TryGetValue(out var r))
                        {
                            item.Reference = r;
                        }
                        else
                        {
                            return reference;
                        }
                    }
                    else
                    {
                        item.Reference = null;
                    }

                    if (toDoItem.Item.ParentId.TryGetValue(out var parentId))
                    {
                        var parent = GetToDoItem(parentId);

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
                }
            );
    }

    public Result UpdateParentsUi(Guid id, ReadOnlyMemory<ToDoShortItem> parents)
    {
        return GetToDoItem(id)
           .IfSuccess(
                item => parents.ToResult()
                   .IfSuccessForEach(UpdateUi)
                   .IfSuccess(
                        ps =>
                        {
                            item.Path = RootItem.DefaultObject
                               .ToReadOnlyMemory()
                               .Combine(ps.Select(x => (object)x))
                               .ToArray();

                            return Result.Success;
                        }
                    )
            );
    }

    public Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateUi(ReadOnlyMemory<ToDoSelectorItem> items)
    {
        return UpdateRootItems(items.Select(x => x.Item.Id))
           .IfSuccess(_ => items.ToResult().IfSuccessForEach(UpdateUi));
    }

    public Result SetIgnoreItemsUi(ReadOnlyMemory<Guid> ids)
    {
        foreach (var value in cache.Values)
        {
            value.IsIgnore = value.Type == ToDoItemType.Reference || ids.Contains(value.Id);
        }

        return Result.Success;
    }

    public Result ExpandItemUi(Guid id)
    {
        return GetToDoItem(id)
           .IfSuccess(
                item =>
                {
                    item.IsExpanded = true;

                    if (item.Parent is null)
                    {
                        return Result.Success;
                    }

                    return ExpandItemUi(item.Parent.Id);
                }
            );
    }

    public Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateRootItems(ReadOnlyMemory<Guid> roots)
    {
        return roots.ToResult()
           .IfSuccessForEach(GetToDoItem)
           .IfSuccess(
                items =>
                {
                    rootItems = items.OrderBy(x => x.OrderIndex);

                    return rootItems.ToResult();
                }
            );
    }

    public Result<ReadOnlyMemory<ToDoItemEntityNotify>> GetRootItems()
    {
        return rootItems.ToResult();
    }

    public Result<ReadOnlyMemory<ToDoItemEntityNotify>> UpdateChildrenItemsUi(Guid id, ReadOnlyMemory<Guid> items)
    {
        return items.ToResult()
           .IfSuccessForEach(GetToDoItem)
           .IfSuccess(
                children => GetToDoItem(id)
                   .IfSuccess(
                        item =>
                        {
                            item.Children.RemoveAll(item.Children.Where(x => !items.Span.Contains(x.Id)));
                            var currentChildrenIds = item.Children.Select(x => x.Id).ToArray().ToReadOnlyMemory();

                            item.Children.AddRange(
                                children.Where(x => !currentChildrenIds.Span.Contains(x.Id)).ToArray()
                            );

                            item.Children.BinarySortByOrderIndex();

                            return Result.Success;
                        }
                    )
                   .IfSuccess(() => children.ToResult())
            );
    }

    public Result<ReadOnlyMemory<ToDoItemEntityNotify>> GetFavoriteItems()
    {
        return cache.Values.Where(x => x.IsFavorite).OrderBy(x => x.OrderIndex).ToArray().ToReadOnlyMemory().ToResult();
    }

    public Result SetFavoriteItems(ReadOnlyMemory<Guid> ids)
    {
        foreach (var item in cache)
        {
            item.Value.IsFavorite = ids.Contains(item.Key);
        }
        
        return Result.Success;
    }

    public Result<ReadOnlyMemory<ToDoItemEntityNotify>> GetBookmarkItems()
    {
        return cache.Values.Where(x => x.IsBookmark).OrderBy(x => x.OrderIndex).ToArray().ToReadOnlyMemory().ToResult();
    }

    public Result<ToDoItemEntityNotify> UpdateUi(ToDoShortItem shortItem)
    {
        return GetToDoItem(shortItem.Id)
           .IfSuccess(
                item =>
                {
                    item.ChildrenType = shortItem.ChildrenType;
                    item.DueDate = shortItem.DueDate;
                    item.MonthsOffset = shortItem.MonthsOffset;
                    item.YearsOffset = shortItem.YearsOffset;
                    item.DaysOffset = shortItem.DaysOffset;
                    item.WeeksOffset = shortItem.WeeksOffset;
                    item.IsRequiredCompleteInDueDate = shortItem.IsRequiredCompleteInDueDate;
                    item.TypeOfPeriodicity = shortItem.TypeOfPeriodicity;
                    item.WeeklyDays.UpdateUi(shortItem.WeeklyDays);
                    item.AnnuallyDays.UpdateUi(shortItem.AnnuallyDays);
                    item.MonthlyDays.UpdateUi(shortItem.MonthlyDays.Select(x => (int)x));
                    item.Description = shortItem.Description;
                    item.DescriptionType = shortItem.DescriptionType;
                    item.Type = shortItem.Type;
                    item.Name = shortItem.Name;
                    item.IsFavorite = shortItem.IsFavorite;
                    item.OrderIndex = shortItem.OrderIndex;
                    item.IsBookmark = shortItem.IsBookmark;
                    item.Icon = shortItem.Icon;
                    item.RemindDaysBefore = shortItem.RemindDaysBefore;
                    item.Link = shortItem.Link.TryGetValue(out var uri) ? uri.AbsoluteUri : string.Empty;
                    item.IsUpdated = true;

                    item.Color = shortItem.Color.IsNullOrWhiteSpace()
                        ? Colors.Transparent
                        : Color.Parse(shortItem.Color);

                    if (shortItem.ReferenceId.TryGetValue(out var referenceId))
                    {
                        var reference = GetToDoItem(referenceId);

                        if (reference.TryGetValue(out var r))
                        {
                            item.Reference = r;
                        }
                        else
                        {
                            return reference;
                        }
                    }
                    else
                    {
                        item.Reference = null;
                    }

                    if (shortItem.ParentId.TryGetValue(out var parentId))
                    {
                        var parent = GetToDoItem(parentId);

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
                }
            );
    }

    public Result<ToDoItemEntityNotify> UpdateUi(ToDoSelectorItem item)
    {
        return GetToDoItem(item.Item.Id)
           .IfSuccess(
                x => UpdateUi(item.Item)
                   .IfSuccess(_ => item.Children.ToResult())
                   .IfSuccessForEach(UpdateUi)
                   .IfSuccessForEach(
                        y =>
                        {
                            y.Parent = x;

                            return y.ToResult();
                        }
                    )
                   .IfSuccessForEach(y => y.Id.ToResult())
                   .IfSuccess(children => UpdateChildrenItemsUi(x.Id, children))
                   .IfSuccess(_ => x.ToResult())
            );
    }
}
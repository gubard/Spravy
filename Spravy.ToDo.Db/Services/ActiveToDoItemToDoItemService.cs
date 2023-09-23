using Microsoft.EntityFrameworkCore;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Db.Services;

public class ActiveToDoItemToDoItemService
{
    public async Task<ActiveToDoItem?> GetActiveItemAsync(SpravyToDoDbContext context, ToDoItemEntity entity)
    {
        var result = await GetActiveToDoItemAsync(context, entity);

        return ToActiveToDoItem(entity, result);
    }

    private Task<ActiveToDoItem?> GetActiveToDoItemAsync(SpravyToDoDbContext context, ToDoItemEntity entity)
    {
        switch (entity.Type)
        {
            case ToDoItemType.Value:
            {
                return GetValueActiveItemAsync(context, entity);
            }
            case ToDoItemType.Group:
            {
                return GetGroupActiveItemAsync(context, entity);
            }
            case ToDoItemType.Planned:
            {
                return GetActiveItemByDueDateAsync(context, entity);
            }
            case ToDoItemType.Periodicity:
            {
                return GetActiveItemByDueDateAsync(context, entity);
            }
            case ToDoItemType.PeriodicityOffset:
            {
                return GetActiveItemByDueDateAsync(context, entity);
            }
            default:
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    private async Task<ActiveToDoItem?> GetActiveItemByDueDateAsync(SpravyToDoDbContext context, ToDoItemEntity entity)
    {
        var items = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == entity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        foreach (var item in items)
        {
            var activeItem = await GetActiveToDoItemAsync(context, item);

            if (activeItem is not null)
            {
                return activeItem;
            }
        }

        return GetActiveItemByDueDate(entity);
    }

    private ActiveToDoItem? GetActiveItemByDueDate(ToDoItemEntity entity)
    {
        if (entity.DueDate == DateTimeOffset.Now.ToCurrentDay())
        {
            return ToActiveToDoItem(entity);
        }

        if (entity.DueDate < DateTimeOffset.Now.ToCurrentDay())
        {
            return ToActiveToDoItem(entity);
        }

        if (entity.DueDate > DateTimeOffset.Now.ToCurrentDay())
        {
            return null;
        }

        throw new ArgumentOutOfRangeException();
    }

    private async Task<ActiveToDoItem?> GetGroupActiveItemAsync(SpravyToDoDbContext context, ToDoItemEntity entity)
    {
        var items = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == entity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        foreach (var item in items)
        {
            var activeItem = await GetActiveToDoItemAsync(context, item);

            if (activeItem is not null)
            {
                return activeItem;
            }
        }

        return null;
    }

    private async Task<ActiveToDoItem?> GetValueActiveItemAsync(SpravyToDoDbContext context, ToDoItemEntity entity)
    {
        if (entity.IsCompleted)
        {
            return null;
        }

        var items = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == entity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        foreach (var item in items)
        {
            var activeItem = await GetActiveToDoItemAsync(context, item);

            if (activeItem is not null)
            {
                return activeItem;
            }
        }

        return ToActiveToDoItem(entity);
    }

    private ActiveToDoItem? ToActiveToDoItem(ToDoItemEntity entity)
    {
        if (entity.ParentId is null)
        {
            return null;
        }

        return new ActiveToDoItem(entity.ParentId.Value, entity.Name);
    }

    private ActiveToDoItem? ToActiveToDoItem(ToDoItemEntity entity, ActiveToDoItem? item)
    {
        if (item is null)
        {
            return null;
        }

        if (entity.ParentId == item.Value.Id)
        {
            return null;
        }

        return item;
    }
}
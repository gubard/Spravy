using Microsoft.EntityFrameworkCore;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Db.Services;

public class ActiveToDoItemToDoItemService
{
    public Task<ActiveToDoItem?> GetActiveItemAsync(SpravyToDoDbContext context, ToDoItemEntity entity)
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
            var activeItem = await GetActiveItemAsync(context, item);

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
            return new ActiveToDoItem(entity.Id, entity.Name);
        }

        if (entity.DueDate < DateTimeOffset.Now.ToCurrentDay())
        {
            return new ActiveToDoItem(entity.Id, entity.Name);
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
            var activeItem = await GetActiveItemAsync(context, item);

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
            var activeItem = await GetActiveItemAsync(context, item);

            if (activeItem is not null)
            {
                return activeItem;
            }
        }

        return new ActiveToDoItem(entity.Id, entity.Name);
    }
}
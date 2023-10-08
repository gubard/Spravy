using Microsoft.EntityFrameworkCore;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Db.Services;

public class ActiveToDoItemToDoItemService
{
    private readonly StatusToDoItemService statusToDoItemService;

    public ActiveToDoItemToDoItemService(StatusToDoItemService statusToDoItemService)
    {
        this.statusToDoItemService = statusToDoItemService;
    }

    public async Task<ActiveToDoItem?> GetActiveItemAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
    {
        var result = await GetActiveToDoItemAsync(context, entity);

        return ToActiveToDoItem(entity, result);
    }

    private Task<ActiveToDoItem?> GetActiveToDoItemAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
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
                return GetPlannedActiveItemAsync(context, entity);
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

    private Task<ActiveToDoItem?> GetPlannedActiveItemAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
    {
        if (entity.IsCompleted)
        {
            return Task.FromResult<ActiveToDoItem?>(null);
        }

        return GetActiveItemByDueDateAsync(context, entity);
    }

    private Task<ActiveToDoItem?> GetActiveItemByDueDateAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
    {
        if (entity.DueDate > DateTimeOffset.Now.ToCurrentDay())
        {
            return Task.FromResult<ActiveToDoItem?>(null);
        }

        return GetChildrenActiveToDoItemAsync(context, entity, GetActiveItemByDueDate(entity));
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

    private Task<ActiveToDoItem?> GetGroupActiveItemAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
    {
        return GetChildrenActiveToDoItemAsync(context, entity, null);
    }

    private Task<ActiveToDoItem?> GetValueActiveItemAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
    {
        if (entity.IsCompleted)
        {
            return Task.FromResult<ActiveToDoItem?>(null);
        }

        return GetChildrenActiveToDoItemAsync(context, entity, ToActiveToDoItem(entity));
    }

    private async Task<ActiveToDoItem?> GetChildrenActiveToDoItemAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        ActiveToDoItem? def
    )
    {
        var items = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == entity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        ActiveToDoItem? result = null;

        foreach (var item in items)
        {
            var status = await statusToDoItemService.GetStatusAsync(context, item);
            var activeItem = await GetActiveToDoItemAsync(context, item);

            switch (status)
            {
                case ToDoItemStatus.Miss:
                    if (activeItem is not null)
                    {
                        return activeItem;
                    }

                    break;
                case ToDoItemStatus.ReadyForComplete:
                    result ??= activeItem;

                    break;
                case ToDoItemStatus.Completed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return result ?? def;
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
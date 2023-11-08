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

    public async Task<ActiveToDoItem?> GetActiveItemAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset
    )
    {
        var result = await GetActiveToDoItemAsync(context, entity, offset);

        return ToActiveToDoItem(entity, result);
    }

    private Task<ActiveToDoItem?> GetActiveToDoItemAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset
    )
    {
        return entity.Type switch
        {
            ToDoItemType.Value => GetValueActiveItemAsync(context, entity, offset),
            ToDoItemType.Group => GetGroupActiveItemAsync(context, entity, offset),
            ToDoItemType.Planned => GetPlannedActiveItemAsync(context, entity, offset),
            ToDoItemType.Periodicity => GetActiveItemByDueDateAsync(context, entity, offset),
            ToDoItemType.PeriodicityOffset => GetActiveItemByDueDateAsync(context, entity, offset),
            ToDoItemType.Circle => GetCircleActiveItemAsync(context, entity, offset),
            ToDoItemType.Step => GetStepActiveItemAsync(context, entity, offset),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private Task<ActiveToDoItem?> GetPlannedActiveItemAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset
    )
    {
        return entity.IsCompleted
            ? Task.FromResult<ActiveToDoItem?>(null)
            : GetActiveItemByDueDateAsync(context, entity, offset);
    }

    private Task<ActiveToDoItem?> GetActiveItemByDueDateAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset
    )
    {
        return entity.DueDate > DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly()
            ? Task.FromResult<ActiveToDoItem?>(null)
            : GetChildrenActiveToDoItemAsync(context, entity, GetActiveItemByDueDate(entity, offset), offset);
    }

    private ActiveToDoItem? GetActiveItemByDueDate(ToDoItemEntity entity, TimeSpan offset)
    {
        if (entity.DueDate == DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
        {
            return ToActiveToDoItem(entity);
        }

        if (entity.DueDate < DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
        {
            return ToActiveToDoItem(entity);
        }

        if (entity.DueDate > DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
        {
            return null;
        }

        throw new ArgumentOutOfRangeException();
    }

    private Task<ActiveToDoItem?> GetGroupActiveItemAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset
    )
    {
        return GetChildrenActiveToDoItemAsync(context, entity, null, offset);
    }

    private Task<ActiveToDoItem?> GetValueActiveItemAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset
    )
    {
        return entity.IsCompleted
            ? Task.FromResult<ActiveToDoItem?>(null)
            : GetChildrenActiveToDoItemAsync(context, entity, ToActiveToDoItem(entity), offset);
    }

    private Task<ActiveToDoItem?> GetCircleActiveItemAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset
    )
    {
        return entity.IsCompleted
            ? Task.FromResult<ActiveToDoItem?>(null)
            : GetChildrenActiveToDoItemAsync(context, entity, ToActiveToDoItem(entity), offset);
    }

    private Task<ActiveToDoItem?> GetStepActiveItemAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset
    )
    {
        return entity.IsCompleted
            ? Task.FromResult<ActiveToDoItem?>(null)
            : GetChildrenActiveToDoItemAsync(context, entity, ToActiveToDoItem(entity), offset);
    }

    private async Task<ActiveToDoItem?> GetChildrenActiveToDoItemAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        ActiveToDoItem? def,
        TimeSpan offset
    )
    {
        var items = await context.Set<ToDoItemEntity>()
            .Where(x => x.ParentId == entity.Id)
            .OrderBy(x => x.OrderIndex)
            .ToArrayAsync();

        ActiveToDoItem? result = null;

        foreach (var item in items)
        {
            var status = await statusToDoItemService.GetStatusAsync(context, item, offset);
            var activeItem = await GetActiveToDoItemAsync(context, item, offset);

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
                case ToDoItemStatus.Planned: break;
                case ToDoItemStatus.Completed: break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        return result ?? def;
    }

    private ActiveToDoItem? ToActiveToDoItem(ToDoItemEntity entity)
    {
        return entity.ParentId is null ? null : new ActiveToDoItem(entity.ParentId.Value, entity.Name);
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